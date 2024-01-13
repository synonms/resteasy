using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Contracts;
using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Sample.Api.Legislations;
using Synonms.RestEasy.Sample.Api.Lookups;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public static class SeedData
{
    public static readonly List<Employee> Employees = new()
    {
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource
                {
                    Title = "Mr",
                    Forename = "Kendrick", 
                    Surname = "Lamar", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1994, 5, 5), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA01",
                        StaffCode = "STA01",
                        PrimaryLocationCode = "COMP",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "LA01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000001")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource
                {
                    Title = "Mr",
                    Forename = "Michael", 
                    Surname = "Archer", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1984, 6, 6), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA02",
                        StaffCode = "STA02",
                        PrimaryLocationCode = "LA01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000002")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "LeBron",
                    Surname = "James", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1994, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA03",
                        StaffCode = "STA03",
                        PrimaryLocationCode = "LA01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000003")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "Heung-min",
                    Surname = "Son", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1994, 8, 8), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA04",
                        StaffCode = "STA04",
                        PrimaryLocationCode = "LDN01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000004")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "Jeff",
                    Surname = "Buckley", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1994, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA05",
                        StaffCode = "STA05",
                        PrimaryLocationCode = "HVN01",
                        EmploymentStartDate = new DateOnly(2010, 1, 1),
                        EmploymentEndDate = new DateOnly(2015, 6, 6),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true,
                        LeavingReasonId = LeavingReasonLookup.LeavingReasonLookupId1
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000005")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "David",
                    Surname = "Bowie", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1944, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA06",
                        StaffCode = "STA06",
                        PrimaryLocationCode = "HVN01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000006")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "Kobe",
                    Surname = "Bryant", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1984, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA07",
                        StaffCode = "STA07",
                        PrimaryLocationCode = "HVN01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000007")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "Michael",
                    Surname = "Jordan", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1964, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA08",
                        StaffCode = "STA08",
                        PrimaryLocationCode = "CHI01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000008")),
        FunctionalHelper.FromResult(
            Employee.Create(
                new EmployeeResource{ 
                    Title = "Mr",
                    Forename = "Jimmy",
                    Surname = "Greaves", 
                    Sex = "Male",
                    DateOfBirth = new DateOnly(1940, 2, 20), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    EmploymentDetails = new EmploymentDetailsResource
                    {
                        IsTeachingStaff = true,
                        TeacherNumber = "TEA09",
                        StaffCode = "STA09",
                        PrimaryLocationCode = "HVN01",
                        EmploymentStartDate = new DateOnly(2023, 1, 1),
                        PayrollNumber = "PAY01",
                        IsQualifiedTeacher = true
                    }
                }))
            .WithId(EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000009"))
    };

    public static readonly List<Contract> Contracts = new()
    {
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000001"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000001")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000002"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000002")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000003"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000003")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000004"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000004")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000005"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000005")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000006"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000006")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000007"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000007")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000008"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000008")),
        FunctionalHelper.FromResult(Contract.Create(new ContractResource
            {
                EmployeeId = EntityId<Employee>.Parse("00000000-0000-0000-0000-000000000009"), 
                StartDate = new DateOnly(2023, 1, 1), 
                EndDate = null,
                EmploymentType = "Permanent"
            }))
            .WithId(EntityId<Contract>.Parse("00000000-0000-0000-0002-000000000009"))
    };

    public static readonly List<Address> Addresses = new()
    {
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Some Street", Line2 = "Svartalfheim", PostCode = "SV1 1SS" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Awful Avenue", Line2 = "Alfheim", PostCode = "AL2 2AA" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Manky Mews", Line2 = "Midgard", PostCode = "MI3 3MM" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Beyond", Line2 = "The Ether", PostCode = "BE1 0ND" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"))
    };
    
    public static readonly List<Legislation> Legislations = new()
    {
        FunctionalHelper.FromResult(Legislation.Create(new LegislationResource { Name = "United Kingdom", CurrencyId = CurrencyLookup.CurrencyLookupId1 })).WithId(EntityId<Legislation>.Parse("00000000-0000-0000-0003-000000000001"))
    };
}