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
// Oh, it also uses my Synonms.Functional library, but again you don't have to.

[RestEasyResource("people")]
public class Person : AggregateRoot<Person>
{
    public const int ForenameMaxLength = 30;
    public const int SurnameMaxLength = 30;
    public const int ColourMaxLength = 10;
    
    private Person(EntityId<Person> id, Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId)
        : this(forename, surname, dateOfBirth, favouriteColour, homeAddressId)
    {
        Id = id;
    }
    
    private Person(Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        FavouriteColour = favouriteColour;
        HomeAddressId = homeAddressId;
    }
    
    public Moniker Forename { get; private set; }
    
    public Moniker Surname { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public Colour? FavouriteColour { get; private set; }
    
    public EntityId<Address> HomeAddressId { get; private set; }

    // TODO: Present Pets as a link (without a navigation property?)
    
    public static Result<Person> Create(PersonResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithOptionalValueObject(resource.FavouriteColour, x => Colour.CreateOptional(x, ColourMaxLength), out Colour? favouriteColourValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                )
            .Build()
            .ToResult(new Person(forenameValueObject, surnameValueObject, dateOfBirthValueObject, favouriteColourValueObject, resource.HomeAddressId));

    public Maybe<Fault> Update(PersonResource resource) =>
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

                return Maybe<Fault>.None;
            });
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
}
```

### Hypermedia

Resources are presented using Ion Hypermedia type (https://ionspec.org/).

Here is an example GET response for a Person resource as defined above:

```json
{
  "value": {
    "id": "00000000-0000-0000-0000-000000000002",
    "forename": "Michael",
    "surname": "Archer",
    "dateOfBirth": "1984-06-06",
    "favouriteColour": "Brown",
    "homeAddressId": "00000000-0000-0000-0001-000000000002",
    "self": {
      "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002",
      "rel": "self",
      "method": "GET"
    },
    "edit-form": {
      "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002/edit-form",
      "rel": "edit-form",
      "method": "GET"
    },
    "delete": {
      "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002",
      "rel": "self",
      "method": "DELETE"
    },
    "homeAddress": {
      "href": "https://localhost:7235/addresses/00000000-0000-0000-0001-000000000002",
      "rel": "related",
      "method": "GET"
    }
  },
  "self": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002",
    "rel": "self",
    "method": "GET"
  }
}
```

All of the links in the above example have been automatically constructed by the framework.  One of the advantages of this RESTful approach over "vanilla" JSON over HTTP is that it enables client UIs to be link driven rather than having to hardcode URLs in to the application.

### Forms

Create and Update requests are facilitated via *Forms*.  Resource collections have an associated `create-form` link, while existing resources have an `edit-form` link.

Forms provide an array of all of the fields available for submission, with information around data types, validation rules etc.  Resource models can be decorated with custom attributes to provide the additional information where required, for example `RestEasyMaxLength` attribute generates the `maxlength` property on the form.  A target URI is also presented so clients know where to POST or PUT the completed form.

```json
{
  "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002",
  "rel": "edit-form",
  "method": "PUT",
  "value": [
    {
      "name": "forename",
      "type": "string",
      "required": true,
      "maxlength": 30,
      "value": "Michael"
    },
    {
      "name": "surname",
      "type": "string",
      "required": true,
      "maxlength": 30,
      "value": "Archer"
    },
    {
      "name": "dateOfBirth",
      "type": "date",
      "required": true,
      "pattern": "^\\d{4}-\\d{2}-\\d{2}$",
      "placeholder": "yyyy-MM-dd",
      "value": "1984-06-06"
    },
    {
      "name": "favouriteColour",
      "type": "string",
      "required": false,
      "value": "Brown",
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
      "value": "00000000-0000-0000-0001-000000000002"
    },
    {
      "name": "id",
      "type": "string",
      "required": false,
      "value": "00000000-0000-0000-0000-000000000002"
    }
  ],
  "self": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000002/edit-form",
    "rel": "edit-form",
    "method": "GET"
  }
}
```

Nested forms are supported in situations where a resource has child resources, including resource collections.  Lookups are also available where you can provide an array of options for clients to constrain acceptable values and populate dropdown lists etc.  Provide an implementation of the `ILookupOptionsProvider` to get the data from your database or wherever you hold it.

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

- Nested child resource example
- Nested child resource collection example
- Related resource collection example
- Query parameters in all requests (filtering, sorting...)
- Entry point endpoint (service discovery - present all top level uris)
- Entity versioning/conflict detection (support ETag/If-Match)
- Auth
