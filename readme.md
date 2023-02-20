# RESTEasy

*_Work In Progress_*

RESTEasy is a framework to help facilitate the implementation of RESTful CRUD API endpoints for your domain.

When building a simple API you write a bit of boilerplate stuff and it's no big deal but as you add more capabilities to your API you end up having to write more and more boilerplate stuff and before you know it adding a simple domain model and some associated CRUD endpoints becomes a tedious nightmare. 

With RESTEasy you just create a handful of objects:

- An *Aggregate Root* (the internal domain model)
- A *Resource* (the external presentation of your domain model)
- An *Aggregate Creator* (given a Resource from the outside world, how do you validate it and create a new Aggregate Root)
- An *Aggregate Updater* (given a Resource from the outside world, how do you validate it and update an existing Aggregate Root)

You focus on your business domain and the framework takes care of the API endpoints, serialisation, pagination, Hypermedia etc.

```csharp
// The internal model.
// This example goes for a DDD-Lite approach with ValueObjects and factory methods that prevent invalid state but you don't have to.
// It's a contrived model for demonstration purposes and not necessarily greate aggregate design ;)
// Oh, it also uses my Synonms.Functional library, but again you don't have to.

[RestEasyResource("people")]
public class Person : AggregateRoot<Person>
{
    public const int ForenameMaxLength = 30;
    public const int SurnameMaxLength = 30;
    public const int ColourMaxLength = 10;
    
    private Person(EntityId<Person> id, Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId, PersonalAchievement greatestAchievement, ICollection<PersonalAchievement> achievements)
        : this(forename, surname, dateOfBirth, favouriteColour, homeAddressId, greatestAchievement, achievements)
    {
        Id = id;
    }
    
    private Person(Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId, PersonalAchievement greatestAchievement, ICollection<PersonalAchievement> achievements)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        FavouriteColour = favouriteColour;
        HomeAddressId = homeAddressId;
        GreatestAchievement = greatestAchievement;
        Achievements = achievements;
    }
    
    public Moniker Forename { get; private set; }
    
    public Moniker Surname { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public Colour? FavouriteColour { get; private set; }

    // Related resource (presents as a link)
    public EntityId<Address> HomeAddressId { get; private set; }

    // Nested resource (presents as a populated child resource object)
    public PersonalAchievement GreatestAchievement { get; private set; }

    // Nested resource collection (presents as a populated array of child resources)
    public ICollection<PersonalAchievement> Achievements { get; private set; }

    public static Result<Person> Create(PersonResource resource) =>
        resource.Achievements
            .Select(PersonalAchievement.Create)
            .Reduce(achievements => achievements)
            .Bind(
                achievements =>
                    PersonalAchievement.Create(resource.GreatestAchievement)
                        .Bind(greatestAchievement =>
                            AggregateRules.CreateBuilder()
                                .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
                                .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
                                .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
                                .WithOptionalValueObject(resource.FavouriteColour, x => Colour.CreateOptional(x, ColourMaxLength), out Colour? favouriteColourValueObject)
                                .WithDomainRules(
                                    RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                                    )
                                .Build()
                                .ToResult(new Person(forenameValueObject, surnameValueObject, dateOfBirthValueObject, favouriteColourValueObject, resource.HomeAddressId, greatestAchievement, achievements.ToList()))));

    public Maybe<Fault> Update(PersonResource resource) =>
        MergeAchievements(resource)
            .BiBind(() => 
                PersonalAchievement.Create(resource.GreatestAchievement)
                    .Bind(greatestAchievement =>
                        AggregateRules.CreateBuilder()
                            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
                            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
                            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
                            .WithOptionalValueObject(resource.FavouriteColour, x => Colour.CreateOptional(x, ColourMaxLength), out Colour? favouriteColourValueObject)
                            .WithDomainRules(
                                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                            )
                            .Build()
                            .BiBind(() =>
                            {
                                Forename = forenameValueObject;
                                Surname = surnameValueObject;
                                DateOfBirth = dateOfBirthValueObject;
                                FavouriteColour = favouriteColourValueObject;
                                HomeAddressId = resource.HomeAddressId;
                                GreatestAchievement = greatestAchievement;

                                return Maybe<Fault>.None;
                            })));
    
    private Maybe<Fault> MergeAchievements(PersonResource resource) =>
        Achievements
            .Merge<PersonalAchievement, PersonalAchievementResource>(
                resource.Achievements,
                (am, r) => am.Id == r.Id,
                PersonalAchievement.Create,
                (am, r) => am.Update(r));
}
```

```csharp
// The external model.
// This is the thing that gets serialised in and out of the API.
// Convention based auto-mapping happens out of the box but if your internal and external models are different you can provide your own mappers. 
public class PersonResource : Resource<Person>
{
    public PersonResource()
    {
    }
    
    public PersonResource(EntityId<Person> id, Link selfLink) 
        : base(id, selfLink)
    {
    }

    [RestEasyRequired]
    [RestEasyMaxLength(Person.ForenameMaxLength)]
    public string Forename { get; set; } = string.Empty;
    
    [RestEasyRequired]
    [RestEasyMaxLength(Person.SurnameMaxLength)]
    public string Surname { get; set; } = string.Empty;

    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.DateOnly)]
    [RestEasyDescriptor(placeholder: Placeholders.DateOnly)]
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;

    [RestEasyLookup("Colour")]
    public string? FavouriteColour { get; set; }
    
    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.Guid)]
    [RestEasyDescriptor(placeholder: Placeholders.Guid)]
    public EntityId<Address> HomeAddressId { get; set; } = EntityId<Address>.Uninitialised;
    
    public PersonalAchievementResource GreatestAchievement { get; set; }

    public IEnumerable<PersonalAchievementResource> Achievements { get; set; } = Enumerable.Empty<PersonalAchievementResource>();
}
```

### Hypermedia

Resources are presented using Ion Hypermedia type (https://ionspec.org/).

Here is an example GET response for a Person resource as defined above:

```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "forename": "Kendrick",
  "surname": "Lamar",
  "dateOfBirth": "1994-05-05",
  "favouriteColour": null,
  "homeAddressId": "00000000-0000-0000-0001-000000000001",
  "greatestAchievement": {
    "id": "cda40291-a83a-4317-aea5-4bd0407d9541",
    "description": "To Pimp A Butterfly",
    "dateOfAchievement": "2015-03-16"
  },
  "achievements": [
    {
      "id": "9b1cc13a-8f26-4200-b1a8-4bd0407d9543",
      "description": "Section.80",
      "dateOfAchievement": "2011-07-02"
    },
    {
      "id": "c426e8b4-29ab-4fd3-9d5c-4bd0407d9543",
      "description": "Good Kid, M.A.A.D City",
      "dateOfAchievement": "2012-10-22"
    },
    {
      "id": "b03de7dd-d57c-4781-9152-4bd0407d9543",
      "description": "DAMN",
      "dateOfAchievement": "2017-04-14"
    },
    {
      "id": "2b625df3-286d-4a69-bcc3-4bd0407d9543",
      "description": "Mr Morale & the Big Steppers",
      "dateOfAchievement": "2022-05-13"
    }
  ],
  "self": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001",
    "rel": "self",
    "method": "GET"
  },
  "edit-form": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001/edit-form",
    "rel": "edit-form",
    "method": "GET"
  },
  "delete": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001",
    "rel": "self",
    "method": "DELETE"
  },
  "homeAddress": {
    "href": "https://localhost:7235/addresses/00000000-0000-0000-0001-000000000001",
    "rel": "related",
    "method": "GET"
  }
}
```

All of the links in the above example have been automatically constructed by the framework.  One of the advantages of this RESTful approach over "vanilla" JSON over HTTP is that it enables client UIs to be link driven rather than having to hardcode URLs in to the application.

Note also how the aggregate members (achievements) have been presented inline as child resources, whereas the related aggregate root (homeAddress) has been presented as a link.

### Forms

Create and Update requests are facilitated via *Forms*.  Resource collections have an associated `create-form` link, while existing resources have an `edit-form` link.

Forms provide an array of all of the fields available for submission, with information around data types, validation rules etc.  Resource models can be decorated with custom attributes to provide the additional information where required, for example `RestEasyMaxLength` attribute generates the `maxlength` property on the form.  A target URI is also presented so clients know where to POST or PUT the completed form.

```json
{
  "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001",
  "rel": "edit-form",
  "method": "PUT",
  "value": [
    {
      "name": "forename",
      "type": "string",
      "required": true,
      "maxlength": 30,
      "value": "Kendrick"
    },
    {
      "name": "surname",
      "type": "string",
      "required": true,
      "maxlength": 30,
      "value": "Lamar"
    },
    {
      "name": "dateOfBirth",
      "type": "date",
      "required": true,
      "pattern": "^\\d{4}-\\d{2}-\\d{2}$",
      "placeholder": "yyyy-MM-dd",
      "value": "1994-05-05"
    },
    {
      "name": "favouriteColour",
      "type": "string",
      "required": false,
      "options": {
        "value": [
          {
            "isEnabled": true,
            "value": "Black"
          },
          {
            "isEnabled": true,
            "value": "Blue"
          },
          {
            "isEnabled": true,
            "value": "Brown"
          },
          {
            "isEnabled": true,
            "value": "Green"
          },
          {
            "isEnabled": true,
            "value": "Purple"
          },
          {
            "isEnabled": true,
            "value": "Orange"
          },
          {
            "isEnabled": true,
            "value": "Red"
          },
          {
            "isEnabled": true,
            "value": "White"
          },
          {
            "isEnabled": true,
            "value": "Yellow"
          }
        ]
      }
    },
    {
      "name": "homeAddressId",
      "type": "string",
      "required": true,
      "pattern": "^[0-9a-fA-F]{8}[-]([0-9a-fA-F]{4}[-]){3}[0-9a-fA-F]{12}$",
      "placeholder": "00000000-0000-0000-0000-000000000000",
      "value": "00000000-0000-0000-0001-000000000001"
    },
    {
      "name": "greatestAchievement",
      "type": "object",
      "form": {
        "value": [
          {
            "name": "description",
            "type": "string",
            "required": true,
            "maxlength": 250,
            "value": "To Pimp A Butterfly"
          },
          {
            "name": "dateOfAchievement",
            "type": "date",
            "required": true,
            "pattern": "^\\d{4}-\\d{2}-\\d{2}$",
            "placeholder": "yyyy-MM-dd",
            "value": "2015-03-16"
          },
          {
            "name": "id",
            "type": "string",
            "required": false,
            "value": "fd5a022b-2b85-4fad-8b20-4bd04080e58c"
          }
        ]
      },
      "required": false,
      "value": {
        "id": "fd5a022b-2b85-4fad-8b20-4bd04080e58c",
        "description": "To Pimp A Butterfly",
        "dateOfAchievement": "2015-03-16"
      }
    },
    {
      "name": "achievements",
      "type": "array",
      "etype": "object",
      "eform": {
        "value": [
          {
            "name": "description",
            "type": "string",
            "required": true,
            "maxlength": 250,
            "value": ""
          },
          {
            "name": "dateOfAchievement",
            "type": "date",
            "required": true,
            "pattern": "^\\d{4}-\\d{2}-\\d{2}$",
            "placeholder": "yyyy-MM-dd",
            "value": "0001-01-01"
          },
          {
            "name": "id",
            "type": "string",
            "required": false,
            "value": "00000000-0000-0000-0000-000000000000"
          }
        ]
      },
      "required": false,
      "value": [
        {
          "id": "2abf46ef-8d9a-4ce8-8d31-4bd04080e58e",
          "description": "Section.80",
          "dateOfAchievement": "2011-07-02"
        },
        {
          "id": "ef0b307c-7e2f-4cf7-a270-4bd04080e58e",
          "description": "Good Kid, M.A.A.D City",
          "dateOfAchievement": "2012-10-22"
        },
        {
          "id": "8bc4c648-9ac9-4079-adb0-4bd04080e58e",
          "description": "DAMN",
          "dateOfAchievement": "2017-04-14"
        },
        {
          "id": "f24fe6fa-8447-4269-a9f3-4bd04080e58e",
          "description": "Mr Morale & the Big Steppers",
          "dateOfAchievement": "2022-05-13"
        }
      ]
    },
    {
      "name": "id",
      "type": "string",
      "required": false,
      "value": "00000000-0000-0000-0000-000000000001"
    }
  ],
  "self": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001/edit-form",
    "rel": "edit-form",
    "method": "GET"
  }
}
```

Nested forms are supported in situations where a resource has child resources, including resource collections (`greatestAchievement` and `achievements` properties in this example).  Lookups are also available where you can provide an array of options for clients to constrain acceptable values and populate dropdown lists etc. (see `favouriteColour`).  Provide an implementation of the `ILookupOptionsProvider` to get the data from your database or wherever you hold it.

## Pagination

Collections are paginated out of the box too:

```csharp
{
  "value": [
    // ...One page of 5 resources...
  ],
  "self": {
    "href": "https://localhost:7235/people",
    "rel": "self",
    "method": "GET"
  },
  "create-form": {
    "href": "https://localhost:7235/people/create-form",
    "rel": "create-form",
    "method": "GET"
  }
  "offset": 0,
  "limit": 5,
  "size": 9,
  "first": {
    "href": "https://localhost:7235/people",
    "rel": "collection",
    "method": "GET"
  },
  "last": {
    "href": "https://localhost:7235/people?offset=5",
    "rel": "collection",
    "method": "GET"
  },
  "next": {
    "href": "https://localhost:7235/people?offset=5",
    "rel": "collection",
    "method": "GET"
  }
}
```

In this example there are 9 resources in total (`size`), of which we are presenting the first 5 (`limit`, or page size), that is with an `offset` (the number of resources to skip) of 0.  Links to the `previous` and `next` page of results are dynamic and only presented if there is another page to navigate to.  This makes paging controls on the UI easier as they can be entirely link driven.


## TODO...

- Related resource collection example
- Query parameters in all requests (filtering, sorting...)
- Entry point endpoint (service discovery - present all top level uris)
- Entity versioning/conflict detection (support ETag/If-Match)
- Auth
