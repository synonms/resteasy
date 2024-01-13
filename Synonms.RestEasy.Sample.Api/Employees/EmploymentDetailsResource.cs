using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Text;
using Synonms.RestEasy.Sample.Api.Infrastructure;
using Synonms.RestEasy.WebApi.Application;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmploymentDetailsResource : ChildResource
{
    public EmploymentDetailsResource()
    {
    }
    
    public EmploymentDetailsResource(Guid id) 
        : base(id)
    {
    }

    [RestEasyRequired]
    public bool IsTeachingStaff { get; set; }
    
    [RestEasyMaxLength(EmploymentDetails.StaffCodeMaxLength)]
    public string? TeacherNumber { get; set; }

    [RestEasyMaxLength(EmploymentDetails.StaffCodeMaxLength)]
    public string? StaffCode { get; set; }
    
    [RestEasyMaxLength(EmploymentDetails.PrimaryLocationCodeMaxLength)]
    public string? PrimaryLocationCode { get; set; }
    
    [RestEasyRequired]
    public DateOnly EmploymentStartDate { get; set; }
    
    public DateOnly? EmploymentEndDate { get; set; }
    
    public DateOnly? LocalAuthorityStartDate { get; set; }
    
    [RestEasyMaxLength(EmploymentDetails.PayrollNumberMaxLength)]
    public string? PayrollNumber { get; set; }
    
    [RestEasyRequired]
    public bool IsQualifiedTeacher { get; set; }
    
    [RestEasyPattern(RegularExpressions.Guid)]
    [RestEasyLookup(nameof(LeavingReasonLookup))]
    public EntityId<Lookup>? LeavingReasonId { get; set; }
    
    [RestEasyHidden]
    public LookupResource? LeavingReason { get; set; }
}