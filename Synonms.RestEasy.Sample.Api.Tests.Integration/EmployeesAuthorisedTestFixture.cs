using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Sample.Api.Infrastructure;
using Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;
using Synonms.RestEasy.Testing;
using Synonms.RestEasy.Testing.Data;
using Microsoft.Extensions.DependencyInjection;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Extensions;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.Core.Schema.Forms;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration;

public class EmployeesAuthorisedTestFixture : SampleApiAuthorisedTestFixture<Employee, EmployeeResource>
{
    public EmployeesAuthorisedTestFixture() : base(5)
    {
    }

    public override ValueTask DisposeAsync() =>
        ValueTask.CompletedTask;

    protected override Employee GenerateUniqueAggregate()
    {
        EmployeeResource resource = GenerateValidResource(null); 
        
        return Employee.Create(resource)
            .Match(
                aggregateRoot => aggregateRoot,
                fault =>
                {
                    Assert.Fail($"Failed to create Employee: [{fault}]");
                    return default(Employee);
                });
    }

    protected override EmployeeResource GenerateInvalidResource(Employee? existingEmployee) => 
        new()
        {
            Id = existingEmployee?.Id.Value ?? Guid.NewGuid()
        };

    protected override EmployeeResource GenerateValidResource(Employee? existingEmployee) =>
        new()
        {
            Id = existingEmployee?.Id.Value ?? Guid.NewGuid(),
            Forename = RandomDataGenerator.GenerateAlphaNumeric(40),
            Surname = RandomDataGenerator.GenerateAlphaNumeric(40),
            Sex = "Male",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
            EmploymentDetails = new EmploymentDetailsResource()
            {
                IsTeachingStaff = true,
                EmploymentStartDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                IsQualifiedTeacher = true
            },
            HomeAddressId = SeedData.Addresses.FirstOrDefault()?.Id ?? EntityId<Address>.Uninitialised
        };

    protected override async Task<Employee> PersistAggregateAsync(ArrangeAggregateInfo<Employee> arrangeAggregateInfo)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        IAggregateRepository<Employee> repository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Employee>>();

        await repository.AddAsync(arrangeAggregateInfo.AggregateRoot, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        return arrangeAggregateInfo.AggregateRoot;
    }

    protected override Task PersistPrerequisitesAsync(ArrangeEntitiesInfo arrangeEntitiesInfo) =>
        Task.CompletedTask;

    protected override async Task RemoveAggregateAsync(EntityId<Employee> id)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        IAggregateRepository<Employee> repository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Employee>>();

        await repository.DeleteAsync(id, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);
    }

    protected override async Task<Employee?> RetrieveAggregateAsync(EntityId<Employee> id)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        IAggregateRepository<Employee> repository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<Employee>>();

        Employee? employee = (await repository.FindAsync(id, CancellationToken.None))
            .Match<Employee?>(employee => employee, () => null);

        return employee;
    }

    protected override void ValidateAggregate(Employee aggregateRoot, EmployeeResource resource)
    {
        Assert.Equal(resource.Id, aggregateRoot.Id.Value);
        // TODO: Remaining properties
    }

    protected override void ValidateResource(Employee aggregateRoot, EmployeeResource resource)
    {
        Assert.Equal(aggregateRoot.Id.Value, resource.Id);
        // TODO: Remaining properties
    }

    protected override void ValidateEditForm(Form form, Employee aggregateRoot)
    {
        FormField? forenameField = form.Fields.SingleOrDefault(formField =>
            formField.Name.Equals(nameof(EmployeeResource.Forename).ToCamelCase()));
        
        Assert.NotNull(forenameField);
        Assert.True(forenameField.IsRequired);
        Assert.Equal(Employee.ForenameMaxLength, forenameField.MaxLength);
        Assert.Equal(aggregateRoot.Forename.Value, forenameField.Value);
        
        // TODO: Remaining fields;
    }

    protected override void ValidateCreateForm(Form form)
    {
        FormField? forenameField = form.Fields.SingleOrDefault(formField =>
            formField.Name.Equals(nameof(EmployeeResource.Forename).ToCamelCase()));
        
        Assert.NotNull(forenameField);
        Assert.True(forenameField.IsRequired);
        Assert.Equal(Employee.ForenameMaxLength, forenameField.MaxLength);
        
        // TODO: Remaining fields;
    }
}