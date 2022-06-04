namespace MoodleApi.Models;

public enum MoodleMethod
{
    CoreWebserviceGetSiteInfo,
    CoreUserGetUsers,
    CoreUserGetUsersByField,
    CoreEnrolGetUsersCourses,
    CoreUserCreateUsers,
    CoreUserUpdateUsers,
    CoreUserDeleteUsers,
    CoreRoleAssignRoles,
    CoreRoleUnassignRoles,
    //Course Enrollment Actions
    EnrolManualEnrolUsers,
    CoreGroupAddGroupMembers,
    CoreGroupDeleteGroupMembers,
    //Course Actions
    CoreCourseGetCategories,
    CoreCourseGetCourses,
    CoreCourseGetContents,
    CoreGroupGetGroups,
    CoreGroupGetCourseGroups,
    CoreGroupCreateGroups,
    CoreEnrolGetEnrolledUsers,
    CoreCourseCreateCourses,
    CoreCourseUpdateCourses,
    //Grade Actions
    CoreGradesGetGrades,
    CoreGradesUpdateGrades,
    CoreGradingGetDefinitions,
    //Calendar Actions
    CoreCalendarGetCalendarEvents,
    CoreCalendarCreateCalendarEvents,
    CoreCalendarDeleteCalendarEvents,
    Default
}