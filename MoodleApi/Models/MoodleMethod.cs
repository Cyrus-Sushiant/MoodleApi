namespace MoodleApi.Models;

public enum MoodleMethod
{
    core_webservice_get_site_info,
    core_user_get_users_by_field,
    core_enrol_get_users_courses,
    core_user_create_users,
    core_user_update_users,
    core_user_delete_users,
    core_role_assign_roles,
    core_role_unassign_roles,
    //Course Enrollment Actions
    enrol_manual_enrol_users,
    core_group_add_group_members,
    core_group_delete_group_members,
    //Course Actions
    core_course_get_categories,
    core_course_get_courses,
    core_course_get_contents,
    core_group_get_groups,
    core_group_get_course_groups,
    core_group_create_groups,
    core_enrol_get_enrolled_users,
    core_course_create_courses,
    core_course_update_courses,
    //Grade Actions
    core_grades_get_grades,
    core_grades_update_grades,
    core_grading_get_definitions,
    //Calendar Actions
    core_calendar_get_calendar_events,
    core_calendar_create_calendar_events,
    core_calendar_delete_calendar_events,
    default_
}