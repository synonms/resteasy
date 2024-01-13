# RestEasy

RestEasy is primarily to help facilitate the implementation of RESTful CRUD API endpoints for your domain.
It uses conventions and attributes to automatically generate the boilerplate "plumbing" required for APIs.  
You focus on your business domain and the framework takes care of the API endpoints, serialisation etc.

- RESTful hypermedia
- Resource collections with pagination, filtering and sorting
- MultiTenancy
- Granular Authorisation
- Entity Framework integration
- OpenApi (Swagger) document generation
- Integration Testing infrastructure
- CQRS abstraction with MediatR enabling alternate non-HTTP inputs e.g. messaging, CLI apps
- Correlation
- Concurrency via ETag related headers

On top of that the library also provides functionality which can be used for general purpose .NET systems:

- Domain Driven Design objects - Entities, strongly typed Entity Ids, Aggregate Roots/Members, Value Objects etc.
- Simple bespoke database migration mechanism for Entity Framework allowing handcrafted SQL Scripts to be applied via an API endpoint (removing the need to rely on EF to script your database)
- JSON and EF Converters for DateOnly/TimeOnly types (missing from .NET Core at the time of writing)
- Extensions to the Synonms.Functional library for Railway Oriented Programming
- Paginated Lists, Date Providers, various extension methods...

## Web API

### Basic Structure

There are a few fundamental objects you will need to create at a minimum in order to achieve a functioning API:

- An _Aggregate Root_ (the internal domain model)
- A _Resource_ (the external presentation of your domain model)
- An _Aggregate Creator_ (given a new Resource from the outside world, how do you validate it and create a new Aggregate Root)
- An _Aggregate Updater_ (given an updated Resource from the outside world, how do you validate it and update the corresponding existing Aggregate Root)

Additionally, you will also likely have a _Repository_ per Aggregate (how does the framework retrieve and store the domain models).

### Aggregates

Aggregates are the internal representation of your domain entities.  They are hierarchical groupings of data.  The 'parent' object is the Aggregate Root and all 'child' objects are Aggregate Members.  All actions are performed via the Aggregate Root.  Repositories, for example are only available for Aggregate Roots.  API endpoints are only generated for Aggregate Roots.

Unlike a database schema where all of the tables could be related, Aggregates should be as small as possible.  As a rule of thumb, only that data which is necessary in order to create an Aggregate Root forms the Aggregate. 

As an example, consider Employees, Employment Details and Employment Contracts as per the Sample API.  
Employees have a 1-1 relationship with Employment Details which are essentially a horizontal partition of an Employee.  You can't create an Employee without knowing their Employment Details and Employment Details have limited value outside the scope of an Employee.  As such, we have an Aggregate with Employee as the Aggregate Root and Employment Details as an Aggregate Member.  
Although Contracts have a required relationship to an Employee, Employees can be created without entering their Contract details and there is value in being able to manage Contracts independently of the Employee.  As such we have another Aggregate with Contract as the Aggregate Root.  Other entities which are fundamental parts of a Contract would be Aggregate Members of that Aggregate (for example if we had a separate Salary entity). 
API endpoints (and Repositories etc.) will be available for Employees and Contracts, but not for Employment Details.  They will be updated via the Employee.

### Resources

Resources are the external representation of the entities. For the most part, resources should consist of properties with simple vanilla C# types (string, int, bool etc.).  Where the corresponding type on the Aggregate is a ValueObject then the Resource would present the underlying type. However, there are certain property types in Resources which will trigger special behaviour which allow for related data to be presented either as embedded data or as a link.

The default Resource mapper enumerates each public instance property on the Resource and reacts to the following special types:

| Type                                  | Example                                                                 | Interpretation                                                 | Action                                                                                                                                                                                                         |
|---------------------------------------|-------------------------------------------------------------------------|----------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| EntityId<TAggregateRoot>              | `public EntityId<Address> AddressId { get; set; }`                      | A link to a related resource                                   | Adds a `related` link to the Resource links unless there is a corresponding Navigation Property on the Aggregate with the same property name less the 'Id' suffix (i.e. `AddressId` AND `Address` properties). |    
| TResource                             | `public AddressResource Address { get; set; }`                          | An embedded resource (from Aggregate Root)                     | Related Aggregate Root mapped and set to property.                                                                                                                                                             |    
| TChildResource                        | `public EmployeeDetailsResource EmployeeDetails { get; set; }`          | An embedded child resource (from Aggregate Member)             | Related Aggregate Member mapped and set to property.                                                                                                                                                           |    
| LookupResource                        | `public LookupResource Currency { get; set; }`                          | An embedded lookup                                             | Related Lookup mapped and set to property.                                                                                                                                                                     |    
| IEnumerable<EntityId<TAggregateRoot>> | `public IEnumerable<EntityId<Contract>> Contracts { get; set; }`        | A link to a related resource collection                        | Adds a `related` link to the Resource links.                                                                                                                                                                   |    
| IEnumerable<TResource>                | `public IEnumerable<ContractResource> Contracts { get; set; }`          | An embedded resource collection (from Aggregate Roots)         | Related Aggregate Roots mapped to Resources and added to property.                                                                                                                                             |
| IEnumerable<TChildResource>           | `public IEnumerable<EmailAddressResource> EmailAddresses { get; set; }` | An embedded child resource collection (from Aggregate Members) | Related Aggregate Members mapped to Child Resources and added to property.                                                                                                                                     |

All other types are assumed to be vanilla properties and Reflection is used to set the property using the value from the Aggregate.  This will potentially fail if the types differ.

### Endpoints

By default the full range of CRUD endpoints are available:

| Endpoint       | Purpose                                                                | HTTP Method | Example route                                                |
|----------------|------------------------------------------------------------------------|-------------|--------------------------------------------------------------|
| **GetAll**     | The root collection endpoint                                           | GET         | /my-resources                                                |
| **CreateForm** | Form containing details of how to add a new resource to the collection | GET         | /my-resources/create-form                                    |
| **Post**       | Add a new resource to the collection                                   | POST        | /my-resources                                                |
| **GetById**    | Retrieve a specific resource by unique Id                              | GET         | /my-resources/00000000-0000-0000-0000-000000000001           |
| **EditForm**   | Form containing details of how to update an existing resource          | GET         | /my-resources/00000000-0000-0000-0000-000000000001/edit-form |
| **Put**        | Update an existing resource                                            | PUT         | /my-resources/00000000-0000-0000-0000-000000000001           |
| **Delete**     | Delete an existing resource                                            | DELETE      | /my-resources/00000000-0000-0000-0000-000000000001           |

Generation of the endpoints is triggered by the `RestEasyResourceAttribute` decorating a domain model:

```csharp
[RestEasyResource(typeof(EmployeeResource), "employees", requiresAuthentication: false, pageLimit: 5)]
public class Employee : AggregateRoot<Employee>
{
  ...
}
```

The parameters provided to the attribute influence how the endpoints are generated:

| Parameter               | Default                     | Purpose                                                                                                                                                                                                                                                            |
|-------------------------|-----------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| resourceType            |                             | The type of the corresponding Resource object to map to, e.g. `Employee` (AggregateRoot) <-> `EmployeeResource` (Resource).                                                                                                                                        |
| collectionPath          |                             | The path for the collection URL.  This must be unique across all aggregates otherwise there will be a routing collision.  It should generally just be pluralised version of the aggregate root name, e.g. `Employee` -> "employees", `PayPeriod` -> "pay-periods". |
| requiresAuthentication  |                             | If set to true then an authenticated user is required to access all endpoints for this resource.                                                                                                                                                                   |
| authorisationPolicyName | null                        | Prefix for policy name to be registered for Authorisation. You should generally use `nameof(MyAggregateRoot)` if authorisation is required, otherwise leave this as null.                                                                                          |
| pageLimit               | Pagination.DefaultPageLimit | Page limit for GetAll collection endpoint                                                                                                                                                                                                                          |
| isCreateDisabled        | false                       | If set to true then CreateForm and Post endpoints are not generated                                                                                                                                                                                                |
| isUpdateDisabled        | false                       | If set to true then EditForm and Put endpoints are not generated                                                                                                                                                                                                   | 
| isDeleteDisabled        | false                       | If set to true then Delete endpoint is not generated                                                                                                                                                                                                               |


### OpenAPI

RestEasy supports OpenApi documentation and documents the paths/operations for all generated endpoints.  No action is required other than enabling Swashbuckle at startup.

```csharp
builder.Services.AddRestEasyFramework(mvcOptions => mvcOptions.WithDefaultFormatters().WithIonFormatters(), SampleApiProject.Assembly)
    .WithApplicationDependenciesFrom(SampleApiProject.Assembly)
    .WithDomainDependenciesFrom(SampleApiProject.Assembly)
    .WithOpenApi(swaggerGenOptions =>
    {
        swaggerGenOptions.SwaggerDoc("v1.0", new OpenApiInfo { Title = "RestEasy Sample API", Version = "v1.0" });
    });
```

### Media Types

RestEasy initially supports JSON (`application/json`) and ION Hypermedia (`application/ion+json`) content types.

The default JSON format is lightly decorated to accommodate `self` links and pagination data.  Otherwise it is "vanilla" JSON.  It is recommended for clients which do not require RESTful Hypermedia, for example back-end data services.

The ION format is richly decorated with Hypermedia and is recommended for clients who want the self describing capabilities of a RESTful service (for example a dynamic, link driven UI).

Examples follow to help visualise the difference.

Default JSON format:
```json
{
  "value": [
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "title": "Mr",
      "forename": "Kendrick",
      "surname": "Lamar",
      // ...other properties...
    },
    // ...other resources...
  ],
  "self": {
    "href": "https://localhost:5001/employees",
    "rel": "self",
    "method": "GET"
  },
  "offset": 0,
  "limit": 5,
  "size": 9
}
```

ION Format:
```json
{
    "value": [
        {
            "id": "00000000-0000-0000-0000-000000000001",
            "title": "Mr",
            "forename": "Kendrick",
            "surname": "Lamar",
            //...other properties...,
            "self": {
                "href": "https://localhost:5001/employees/00000000-0000-0000-0000-000000000001",
                "rel": "self",
                "method": "GET"
            },
            "edit-form": {
                "href": "https://localhost:5001/employees/00000000-0000-0000-0000-000000000001/edit-form",
                "rel": "edit-form",
                "method": "GET"
            },
            "delete": {
                "href": "https://localhost:5001/employees/00000000-0000-0000-0000-000000000001",
                "rel": "self",
                "method": "DELETE"
            },
            "homeAddress": {
                "href": "https://localhost:5001/addresses/00000000-0000-0000-0001-000000000001",
                "rel": "related",
                "method": "GET"
            },
            "contracts": {
                "href": "https://localhost:5001/contracts?employeeId=00000000-0000-0000-0000-000000000001",
                "rel": "related",
                "method": "GET"
            }
        },
        // ...other resources...
    ],
    "self": {
        "href": "https://localhost:5001/employees",
        "rel": "self",
        "method": "GET"
    },
    "create-form": {
        "href": "https://localhost:5001/employees/create-form",
        "rel": "create-form",
        "method": "GET"
    },
    "offset": 0,
    "limit": 5,
    "size": 9,
    "first": {
        "href": "https://localhost:5001/employees?offset=0",
        "rel": "collection",
        "method": "GET"
    },
    "last": {
        "href": "https://localhost:5001/employees?offset=5",
        "rel": "collection",
        "method": "GET"
    },
    "next": {
        "href": "https://localhost:5001/employees?offset=5",
        "rel": "collection",
        "method": "GET"
    }
}
```

Additional formats can be supported by creating a suite of JsonConverters in `WebApi/Serialisation` and Formatters in `WebApi/Hypermedia`.  The OpenApi operations will also need to be tweaked in the `RestEasyDocumentFilter` class for the new media type to be reflected in the OpenApi documentation. 

### Faults

Client faults handled by the framework are presented back to the client as an array of errors:

```json
{
  "errors": [
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "code": "VAL01",
      "title": "Validation Rule",
      "detail": "Name must be 20 characters or less.",
      "source": {
        "pointer": "Name",
        "parameter": "Ienteredanunacceptablylongname"
      }
    }
  ],
  "self": {
    "href": "https://localhost:5001/employees",
    "rel": "self",
    "method": "POST"
  }
}
```

Client faults resolve to the following HTTP response codes:

| Fault                 | Response Code             |
|-----------------------|---------------------------|
| ApplicationRuleFault  | 400 Bad Request           |
| ApplicationRulesFault | 400 Bad Request           |
| DomainRuleFault       | 400 Bad Request           |
| DomainRulesFault      | 400 Bad Request           |
| EntityNotFoundFault   | 404 Not Found             |

Any other type of fault is considered a server fault and will present back to the client as a 500 Internal Server Error without a response body.

Anti-corruption layer validation failures (e.g. `Name` is not filled in) should be surfaced as an `ApplicationRulesFault`.
Business logic validation failures (e.g. `ContractStartDate` must be on or after the `EmploymentStartDate`) should be surfaced as a `DomainRulesFault`.
The `AggregateRules` class is designed to simplify the implementation of domain rules and generation of validation failures: 

```csharp
internal static Result<Legislation> Create(LegislationResource resource) =>
    AggregateRules.CreateBuilder()
        .WithMandatoryValueObject(resource.Name, x => Moniker.CreateMandatory(x, NameMaxLength), out Moniker nameValueObject)
        .WithDomainRules(
            RelatedEntityIdRuleset<Lookup>.Create(nameof(CurrencyId), resource.CurrencyId)
            )
        .Build()
        .ToResult(() => new Legislation((EntityId<Legislation>)resource.Id, nameValueObject, resource.CurrencyId));
```

In the above example the `WithMandatoryValueObject` call performs the conversion of plain `string` type to `Moniker` ValueObject (including validating it).
The `WithDomainRules` call allows you to pass a collection of `IDomainRuleset` implementations.  In this case we're just checking the CurrencyId is not an empty Guid but you can make whatever rulesets you want.
Any validation failures will be aggregated and passed back to the caller as a `DomainRulesFault`. 
Only if no faults occur will the `Legislation` be created.

`EntityNotFoundFault` should be used when one of the resources specified in the route is not found.  If some related required entity is not found which does not form part of the route then an alternative Fault type should be used.
