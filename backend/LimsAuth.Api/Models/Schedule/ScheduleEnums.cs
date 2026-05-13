namespace LimsAuth.Api.Models;

public enum ScheduleSource
{
    CentralScheduling,
    Reservation,
    TeachingRequest,
    PendingReservation,
    PendingTeachingRequest,
    Cancelled
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}

public enum RegistrationStatus
{
    Pending,
    Registered,
    Overdue
}

public enum ProjectCategory
{
    CourseTeaching,
    TeacherResearch,
    StudentResearch,
    InnovationEntrepreneurship,
    GraduationThesis,
    StudentActivity,
    InstitutionActivity,
    Other
}
