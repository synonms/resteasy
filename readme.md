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
    private const int ForenameMaxLength = 30;
    private const int SurnameMaxLength = 30;
    
    private Person(EntityId<Person> id, Moniker forename, Moniker surname, EventDate dateOfBirth, EntityId<Address> homeAddressId)
        : this(forename, surname, dateOfBirth, homeAddressId)
    {
        Id = id;
    }
    
    private Person(Moniker forename, Moniker surname, EventDate dateOfBirth, EntityId<Address> homeAddressId)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        HomeAddressId = homeAddressId;
    }
    
    public Moniker Forename { get; private set; }
    
    public Moniker Surname { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public EntityId<Address> HomeAddressId { get; private set; }

    public static Result<Person> Create(PersonResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                )
            .Build()
            .ToResult(new Person(forenameValueObject, surnameValueObject, dateOfBirthValueObject, resource.HomeAddressId));

    public Maybe<Fault> Update(PersonResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
            )
            .Build()
            .BiBind(() =>
            {
                Forename = forenameValueObject;
                Surname = surnameValueObject;
                DateOfBirth = dateOfBirthValueObject;
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

    public string Forename { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
    
    public EntityId<Address> HomeAddressId { get; set; } = EntityId<Address>.Uninitialised;
}
```

### Hypermedia

Resources are presented using Ion Hypermedia type (https://ionspec.org/).

Here is an example GET response for a Person resource as defined above:

```json
{
  "value": {
    "id": "00000000-0000-0000-0000-000000000001",
    "forename": "Kendrick",
    "surname": "Lamar",
    "dateOfBirth": "1984-05-05",
    "homeAddressId": "00000000-0000-0000-0001-000000000001",
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
  },
  "self": {
    "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001",
    "rel": "self",
    "method": "GET"
  }
}
```

All of the links in the above example have been automatically constructed by the framework.  Hypermedia is great because it enables client UIs to be link driven rather than having to hardcode URLs in to the application.

### Forms

Create and Update requests are facilitated via *Forms*.  Resource collections have an associated `create-form` link, while existing resources have an `edit-form` link.

Forms provide an array of all of the fields available for submission, with information around data types, validation rules etc.  Resource models can be decorated with custom attributes to provide additional information (*TODO: Examples*).  A target URI is also presented so clients know where to POST or PUT the completed form.

```json
{
  "href": "https://localhost:7235/people/00000000-0000-0000-0000-000000000001",
  "rel": "edit-form",
  "method": "PUT",
  "value": [
    {
      "name": "forename",
      "type": "string",
      "required": false,
      "value": "Kendrick"
    },
    {
      "name": "surname",
      "type": "string",
      "required": false,
      "value": "Lamar"
    },
    {
      "name": "dateOfBirth",
      "type": "date",
      "required": false,
      "value": "1984-05-05"
    },
    {
      "name": "homeAddressId",
      "type": "string",
      "required": false,
      "value": "00000000-0000-0000-0001-000000000001"
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

Nested forms are supported in situations where a resource has child resources, including resource collections.  Lookups will also be available soon where you can provide an array of key/value pairs for clients to constrain acceptable values and populate dropdown lists etc.

## TODO...

- Lookups for forms
- Nested child resource example
- Nested child resource collection example
- Related resource collection example
- Query parameters in all requests (filtering, sorting...)
- Entry point endpoint (service discovery - present all top level uris)
- Entity versioning/conflict detection (support ETag/If-Match)
- Auth
