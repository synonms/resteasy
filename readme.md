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

Documentation is available on my [Portfolio](https://victorious-river-03dbada03.5.azurestaticapps.net/rest-easy).

## Get Started

### Projects/NuGet Packages

[![NuGet version (Synonms.RestEasy.Core)](https://img.shields.io/nuget/v/Synonms.RestEasy.Core?label=Synonms.RestEasy.Core)](https://www.nuget.org/packages/Synonms.RestEasy.Core/)

Shared functionality not specific to web APIs, including DDD objects, the resource schema and serialisation.

[![NuGet version (Synonms.RestEasy.EntityFramework)](https://img.shields.io/nuget/v/Synonms.RestEasy.Core?label=Synonms.RestEasy.EntityFramework)](https://www.nuget.org/packages/Synonms.RestEasy.EntityFramework/)

Persistence implementations for Entity Framework including a base DbContext with all of the DDD objects and domain event plumbing configured.

[![NuGet version (Synonms.RestEasy.WebApi)](https://img.shields.io/nuget/v/Synonms.RestEasy.Core?label=Synonms.RestEasy.WebApi)](https://www.nuget.org/packages/Synonms.RestEasy.WebApi/)

Web API related functionality including endpoint generation, middleware, Hypermedia and MediatR/Swashbuckle integrations.

[![NuGet version (Synonms.RestEasy.Testing)](https://img.shields.io/nuget/v/Synonms.RestEasy.Core?label=Synonms.RestEasy.Testing)](https://www.nuget.org/packages/Synonms.RestEasy.Testing/)

Infrastructure to simplify Integration testing for RestEasy endpoints with test fixture base classes.

[![NuGet version (Synonms.RestEasy.Testing.EntityFrameworkCore)](https://img.shields.io/nuget/v/Synonms.RestEasy.Core?label=Synonms.RestEasy.Testing.EntityFrameworkCore)](https://www.nuget.org/packages/Synonms.RestEasy.Testing.EntityFrameworkCore/)

Additional test fixtures for Entity Framework integrations.

### ASP.NET Web API

Add a reference to the `Synonms.RestEasy.WebApi` NuGet package.

RESTEasy requires implementations of `Product`, `Tenant` and `User` models. There is Middleware plumbed into the pipeline which will try to resolve each of these entities if available. They can be empty implementations as demonstrated below if you have no need for them, or you can add whatever properties you want:

```csharp
public class SampleProduct : RestEasyProduct
{
}

public class SampleTenant : RestEasyTenant
{
}

public class SampleUser : RestEasyUser
{
}
```

RESTEasy also requires corresponding repositories for these models. Again, they can be empty implementations if you do not have any need for these items, otherwise grab them from your tenants database or call another service or whatever you want:

```csharp
public class SampleProductRepository : IProductRepository<SampleProduct>
{
    public Task<IEnumerable<SampleProduct>> FindAvailableProductsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Enumerable.Empty<SampleProduct>());

    public Task<Maybe<SampleProduct>> FindSelectedProductAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Maybe<SampleProduct>.None);
}

public class SampleTenantRepository : ITenantRepository<SampleTenant>
{
    public Task<IEnumerable<SampleTenant>> FindAvailableTenantsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Enumerable.Empty<SampleTenant>());

    public Task<Maybe<SampleTenant>> FindSelectedTenantAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Maybe<SampleTenant>.None);
}

public class SampleUserRepository : IUserRepository<SampleUser>
{
    public Task<Maybe<SampleUser>> FindAuthenticatedUserAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Maybe<SampleUser>.Some(new SampleUser()));
}
```

Finally, if you want to use lookups you need an implementation of `ILookupIOptionsProvider` to get lookup values (for example from a database):

```csharp
public class LookupOptionsProvider : ILookupOptionsProvider
{
    public IEnumerable<FormFieldOption> Get(string discriminator) =>
        Enumerable<FormFieldOption>.Empty();
}
```

In `Program.cs` (or wherever you manage your startup code) wire up RestEasy with the classes you created:

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ILoggerFactory loggerFactory = new LoggerFactory();

RestEasyOptions options = new()
{
    Assemblies = typeof(Program).Assembly,
    MvcOptionsConfigurationAction = mvcOptions => mvcOptions.ClearFormatters().WithDefaultFormatters(loggerFactory).WithIonFormatters(loggerFactory),
    SwaggerGenConfigurationAction = swaggerGenOptions => swaggerGenOptions.SwaggerDoc("v1.0", new OpenApiInfo { Title = "RestEasy Sample API", Version = "v1.0" }),
    SwaggerUiConfigurationAction = swaggerUiOptions => swaggerUiOptions.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1.0")
};

builder.Services.AddRestEasyFramework<SampleUser, SampleProduct, SampleTenant>(options);
builder.Services.AddSingleton<ILookupOptionsProvider, LookupOptionsProvider>();

WebApplication app = builder.Build();

app.UseRestEasyFramework<SampleUser, SampleProduct, SampleTenant>(options);

app.Run();
```

`RestEasyOptions` is the place to look if you want to customise the behaviour.

### Basic Structure

There are a few fundamental objects you will need to create at a minimum in order to achieve a functioning API:

- An _Aggregate Root_ (the internal domain model)
- A _Resource_ (the external presentation of your domain model)
- An _Aggregate Creator_ (given a new Resource from the outside world, how do you validate it and create a new Aggregate Root)
- An _Aggregate Updater_ (given an updated Resource from the outside world, how do you validate it and update the corresponding existing Aggregate Root)

Additionally, you will also likely have a _Repository_ per Aggregate (how does the framework retrieve and store the domain models).

## Aggregates

Aggregates are the internal representation of your domain entities.  They are hierarchical groupings of data.  The 'parent' object is the Aggregate Root and all 'child' objects are Aggregate Members.  All actions are performed via the Aggregate Root.  Repositories, for example are only available for Aggregate Roots.  API endpoints are only generated for Aggregate Roots.

Unlike a database schema where all of the tables could be related, Aggregates should be as small as possible.  As a rule of thumb, only that data which is necessary in order to create an Aggregate Root forms the Aggregate. 

As an example, consider Employees, Employment Details and Employment Contracts as per the Sample API.  
Employees have a 1-1 relationship with Employment Details which are essentially a horizontal partition of an Employee.  You can't create an Employee without knowing their Employment Details and Employment Details have limited value outside the scope of an Employee.  As such, we have an Aggregate with Employee as the Aggregate Root and Employment Details as an Aggregate Member.  
Although Contracts have a required relationship to an Employee, Employees can be created without entering their Contract details and there is value in being able to manage Contracts independently of the Employee.  As such we have another Aggregate with Contract as the Aggregate Root.  Other entities which are fundamental parts of a Contract would be Aggregate Members of that Aggregate (for example if we had a separate Salary entity). 
API endpoints (and Repositories etc.) will be available for Employees and Contracts, but not for Employment Details.  They will be updated via the Employee.

## Resources

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

## Aggregate Creators

POST methods are used to create new Aggregates and accept the corresponding Resource in the request body. The AggregateCreator is responsible for taking the incoming Resource, validating it and creating the new Aggregate.

Use the related `IAggregateRepository` to check for duplicates etc. Note that the framework will automatically perform the insert and save via the implementation of the repository.

```csharp
public class PermissionCreator : IAggregateCreator<Permission, PermissionResource>
{
    private readonly IAggregateRepository<Permission> _aggregateRepository;

    public PermissionCreator(IAggregateRepository<Permission> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    public async Task<Result<Permission>> CreateAsync(PermissionResource resource, CancellationToken cancellationToken)
    {
        Maybe<Permission> existingPermissionOutcome = await _aggregateRepository.FindAsync((EntityId<Permission>)resource.Id, cancellationToken);

        return existingPermissionOutcome.Match(
        existingPermission => new DomainRuleFault("{entityType} Id '{id}' already exists.", nameof(Permission), resource.Id),
            () => Permission.Create(resource));
    }
}
```

## Aggregate Updaters

PUT methods are used to update existing Aggregates and accept the corresponding Resource in the request body (the Id is obtained from the route). The AggregateUpdater is responsible for retrieving the existing Aggregate, validating the incoming Resource and updating the Aggregate.

Note that the framework will automatically find the existing aggregate and do the update and save via the implementation of the repository.

```csharp
public class PermissionUpdater : IAggregateUpdater<Permission, PermissionResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Permission aggregateRoot, PermissionResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}
```

## Repositories

Aggregates require a Repository in order for RESTEasy to be able to store and retrieve them. The `Synonms.RestEasy.EntityFramework` package provides a default implementation which will do all of the work for you in most cases. Simply create a Repository class for your Aggregate and inherit from `AggregateRepository`:

```csharp
public class PermissionRepository : AggregateRepository<Permission>
{
    public PermissionRepository(MyDbContext dbContext) : base(dbContext)
    {
    }
}
```

There will be cases when you need to specify `Include` clauses when surfacing your resources to pull in aggregate members etc. To do that, simply override the virtual members:

```csharp
public class RoleRepository : AggregateRepository<Role>
{
    public RoleRepository(MyDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Maybe<Role>> FindAsync(EntityId<Role> id, CancellationToken cancellationToken)
    {
        Role? aggregateRoot = await DbContext.Set<Role>()
            .Include(role => role.Product)
            .Include(role => role.RolePermissions).ThenInclude(rolePermission => rolePermission.Permission)
            .FirstOrDefaultAsync(role => role.Id == id, cancellationToken);

        return aggregateRoot ?? Maybe<Role>.None;
    }

    public override IQueryable<Role> Query(Expression<Func<Role, bool>> predicate) =>
        DbContext.Set<Role>()
            .Include(role => role.Product)
            .Include(role => role.RolePermissions).ThenInclude(rolePermission => rolePermission.Permission)
            .Where(predicate);

    public override Task<PaginatedList<Role>> ReadAllAsync(int offset, int limit, Func<IQueryable<Role>, IQueryable<Role>> sortFunc, CancellationToken cancellationToken)
    {
        IIncludableQueryable<Role, Permission> queryable = DbContext.Set<Role>()
            .Include(role => role.Product)
            .Include(role => role.RolePermissions).ThenInclude(rolePermission => rolePermission.Permission);

        return Task.FromResult(PaginatedList<Role>.Create(sortFunc.Invoke(queryable), offset, limit));
    }
}
```

For all of this to work you will of course need a `DbContext`. There is a base class for that too which wires in all the value converters you need and adds Domain Event capabilities too.

```csharp
public class MyDbContext : RestEasyDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options, IDomainEventDispatcher domainEventDispatcher)
        : base(options, domainEventDispatcher)
    {
    }

    protected MyDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher)
        : base(options, domainEventDispatcher)
    {
    }

    // Add whatever DbSets you want as normal
    public DbSet<Permission> PermissionDbSet { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        RegisterValueTypeValueConverters(configurationBuilder, MyWebApiProject.Assembly);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(EntityFrameworkCoreProject.Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(MyWebApiProject.Assembly);

        RegisterEntityIdValueConverters(modelBuilder);
    }
}
```

Add it to your DI container:

```csharp
serviceCollection.AddScoped(serviceProvider =>
{
    IDomainEventDispatcher domainEventDispatcher = serviceProvider.GetRequiredService<IDomainEventDispatcher>();

    string? connectionString = configuration.GetConnectionString("MyConnectionString");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new ApplicationException("Unable to determine valid connection string from configuration.");
    }

    DbContextOptions<MyDbContext> options = new DbContextOptionsBuilder<MyDbContext>()
        .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
        .Options;
    MyDbContext myDbContext = new(options, domainEventDispatcher);

    return myDbContext;
});

// RestEasyDbContext is used by the framework for Migrations
serviceCollection.AddScoped<RestEasyDbContext>(serviceProvider => serviceProvider.GetRequiredService<MyDbContext>());
```

If you are averse to Entity Framework you can by all means provide your own implementations of `IAggregateRepository`, it's just obviously more work.

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

| Parameter               | Default                     | Purpose                                                                                                                                                                                                                                                                                                                 |
|-------------------------|-----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| resourceType            |                             | The type of the corresponding Resource object to map to, e.g. `Employee` (AggregateRoot) <-> `EmployeeResource` (Resource).                                                                                                                                                                                             |
| collectionPath          |                             | The path for the collection URL.  This must be unique across all aggregates otherwise there will be a routing collision.  It should generally just be pluralised version of the aggregate root name, e.g. `Employee` -> "employees", `PayPeriod` -> "pay-periods", but you can add route prefixes e.g. "api/employees". |
| requiresAuthentication  |                             | If set to true then an authenticated user is required to access all endpoints for this resource.                                                                                                                                                                                                                        |
| authorisationPolicyName | null                        | Prefix for policy name to be registered for Authorisation. You should generally use `nameof(MyAggregateRoot)` if authorisation is required, otherwise leave this as null.                                                                                                                                               |
| pageLimit               | Pagination.DefaultPageLimit | Page limit for GetAll collection endpoint                                                                                                                                                                                                                                                                               |
| isCreateDisabled        | false                       | If set to true then CreateForm and Post endpoints are not generated                                                                                                                                                                                                                                                     |
| isUpdateDisabled        | false                       | If set to true then EditForm and Put endpoints are not generated                                                                                                                                                                                                                                                        | 
| isDeleteDisabled        | false                       | If set to true then Delete endpoint is not generated                                                                                                                                                                                                                                                                    |


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
