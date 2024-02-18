using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Employees;

[RestEasyChildResource(typeof(EmploymentDetailsResource))]
public class EmploymentDetails : AggregateMember<EmploymentDetails>
{
    public const int TeacherNumberMaxLength = Reference.DefaultMaxLength;
    public const int StaffCodeMaxLength = Code.DefaultMaxLength;
    public const int PrimaryLocationCodeMaxLength = Code.DefaultMaxLength;
    public const int PayrollNumberMaxLength = Reference.DefaultMaxLength;
    
    private EmploymentDetails(EntityId<EmploymentDetails> id, EntityId<Employee> employeeId, IsA isTeachingStaff, Reference? teacherNumber, Code? staffCode, Code? primaryLocationCode, EventDate employmentStartDate, EventDate? employmentEndDate, EventDate? localAuthorityStartDate, Reference? payrollNumber, IsA isQualifiedTeacher, EntityId<Lookup>? leavingReasonId)
        : this(employeeId, isTeachingStaff, teacherNumber, staffCode, primaryLocationCode, employmentStartDate, employmentEndDate, localAuthorityStartDate, payrollNumber, isQualifiedTeacher, leavingReasonId)
    {
        Id = id;
        
        // Bodge to cover for not having an ORM
        if (leavingReasonId == LeavingReasonLookup.LeavingReasonLookupId1) LeavingReason = LeavingReasonLookup.LeavingReasonLookup1;
        else if (leavingReasonId == LeavingReasonLookup.LeavingReasonLookupId2) LeavingReason = LeavingReasonLookup.LeavingReasonLookup2;
    }
    
    private EmploymentDetails(EntityId<Employee> employeeId, IsA isTeachingStaff, Reference? teacherNumber, Code? staffCode, Code? primaryLocationCode, EventDate employmentStartDate, EventDate? employmentEndDate, EventDate? localAuthorityStartDate, Reference? payrollNumber, IsA isQualifiedTeacher, EntityId<Lookup>? leavingReasonId)
    {
        EmployeeId = employeeId;
        IsTeachingStaff = isTeachingStaff;
        TeacherNumber = teacherNumber;
        StaffCode = staffCode;
        PrimaryLocationCode = primaryLocationCode;
        EmploymentStartDate = employmentStartDate;
        EmploymentEndDate = employmentEndDate;
        LocalAuthorityStartDate = localAuthorityStartDate;
        PayrollNumber = payrollNumber;
        IsQualifiedTeacher = isQualifiedTeacher;
        LeavingReasonId = leavingReasonId;
    }
    
    public EntityId<Employee> EmployeeId { get; private set; }

    public IsA IsTeachingStaff { get; private set; }
    
    public Reference? TeacherNumber { get; private set; }
    
    public Code? StaffCode { get; private set; }

    public Code? PrimaryLocationCode { get; private set; }
    
    public EventDate EmploymentStartDate { get; private set; }

    public EventDate? EmploymentEndDate { get; private set; }
    
    public EventDate? LocalAuthorityStartDate { get; private set; }

    public Reference? PayrollNumber { get; private set; }
    
    public IsA IsQualifiedTeacher { get; private set; }

    public EntityId<Lookup>? LeavingReasonId { get; private set; }
    public LeavingReasonLookup? LeavingReason { get; private set; }

    public static Result<EmploymentDetails> Create(EntityId<Employee> employeeId, EmploymentDetailsResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.IsTeachingStaff, IsA.CreateMandatory, out IsA isTeachingStaffValueObject)
            .WithOptionalValueObject(resource.TeacherNumber, x => Reference.CreateOptional(x, TeacherNumberMaxLength), out Reference? teacherNumberValueObject)
            .WithOptionalValueObject(resource.StaffCode, x => Code.CreateOptional(x, StaffCodeMaxLength), out Code? staffCodeValueObject)
            .WithOptionalValueObject(resource.PrimaryLocationCode, x => Code.CreateOptional(x, PrimaryLocationCodeMaxLength), out Code? primaryLocationCodeValueObject)
            .WithMandatoryValueObject(resource.EmploymentStartDate, EventDate.CreateMandatory, out EventDate employmentStartDateValueObject)
            .WithOptionalValueObject(resource.EmploymentEndDate, EventDate.CreateOptional, out EventDate? employmentEndDateValueObject)
            .WithOptionalValueObject(resource.LocalAuthorityStartDate, EventDate.CreateOptional, out EventDate? localAuthorityStartDateValueObject)
            .WithOptionalValueObject(resource.PayrollNumber, x => Reference.CreateOptional(x, PayrollNumberMaxLength), out Reference? payrollNumberValueObject)
            .WithMandatoryValueObject(resource.IsQualifiedTeacher, IsA.CreateMandatory, out IsA isQualifiedTeacherValueObject)
            .Build()
            .ToResult(new EmploymentDetails(
                (EntityId<EmploymentDetails>)resource.Id, 
                employeeId, 
                isTeachingStaffValueObject, 
                teacherNumberValueObject, 
                staffCodeValueObject, 
                primaryLocationCodeValueObject, 
                employmentStartDateValueObject, 
                employmentEndDateValueObject,
                localAuthorityStartDateValueObject,
                payrollNumberValueObject,
                isQualifiedTeacherValueObject,
                resource.LeavingReasonId
                ));

    public Maybe<Fault> Update(EmploymentDetailsResource resource, Action? updatedDelegate) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.IsTeachingStaff, IsA.CreateMandatory, out IsA isTeachingStaffValueObject)
            .WithOptionalValueObject(resource.TeacherNumber, x => Reference.CreateOptional(x, TeacherNumberMaxLength), out Reference? teacherNumberValueObject)
            .WithOptionalValueObject(resource.StaffCode, x => Code.CreateOptional(x, StaffCodeMaxLength), out Code? staffCodeValueObject)
            .WithOptionalValueObject(resource.PrimaryLocationCode, x => Code.CreateOptional(x, PrimaryLocationCodeMaxLength), out Code? primaryLocationCodeValueObject)
            .WithMandatoryValueObject(resource.EmploymentStartDate, EventDate.CreateMandatory, out EventDate employmentStartDateValueObject)
            .WithOptionalValueObject(resource.EmploymentEndDate, EventDate.CreateOptional, out EventDate? employmentEndDateValueObject)
            .WithOptionalValueObject(resource.LocalAuthorityStartDate, EventDate.CreateOptional, out EventDate? localAuthorityStartDateValueObject)
            .WithOptionalValueObject(resource.PayrollNumber, x => Reference.CreateOptional(x, PayrollNumberMaxLength), out Reference? payrollNumberValueObject)
            .WithMandatoryValueObject(resource.IsQualifiedTeacher, IsA.CreateMandatory, out IsA isQualifiedTeacherValueObject)
            .Build()
            .BiBind(() =>
            {
                DateTime? originalUpdatedAt = UpdatedAt;
                
                UpdateMandatoryValue(_ => _.IsTeachingStaff, isTeachingStaffValueObject);
                UpdateMandatoryValue(_ => _.EmploymentStartDate, employmentStartDateValueObject);
                UpdateMandatoryValue(_ => _.IsQualifiedTeacher, isQualifiedTeacherValueObject);
                UpdateOptionalValue(_ => _.TeacherNumber, teacherNumberValueObject);
                UpdateOptionalValue(_ => _.StaffCode, staffCodeValueObject);
                UpdateOptionalValue(_ => _.PrimaryLocationCode, primaryLocationCodeValueObject);
                UpdateOptionalValue(_ => _.EmploymentEndDate, employmentEndDateValueObject);
                UpdateOptionalValue(_ => _.LocalAuthorityStartDate, localAuthorityStartDateValueObject);
                UpdateOptionalValue(_ => _.PayrollNumber, payrollNumberValueObject);
                UpdateOptionalValue(_ => _.LeavingReasonId, resource.LeavingReasonId);

                // TODO: A nicer way of doing this
                if (UpdatedAt != originalUpdatedAt)
                {
                    updatedDelegate?.Invoke();
                }
                
                // Bodge to cover for not having an ORM
                if (LeavingReasonId == LeavingReasonLookup.LeavingReasonLookupId1) LeavingReason = LeavingReasonLookup.LeavingReasonLookup1;
                else if (LeavingReasonId == LeavingReasonLookup.LeavingReasonLookupId2) LeavingReason = LeavingReasonLookup.LeavingReasonLookup2;
                else LeavingReason = null;

                return Maybe<Fault>.None;
            });
}
