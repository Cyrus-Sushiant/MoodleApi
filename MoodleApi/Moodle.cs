using MoodleApi.Extensions;
using MoodleApi.Models;
using MoodleApi.Models.Responses;
using System.Net;
using System.Text;

namespace MoodleApi;

public class Moodle
{
    private readonly HttpClient _httpClient;
    #region Properties

    /// <summary>
    /// This property sets you Api token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Repressents if the token is set.
    /// </summary>
    private bool TokenIsSet => string.IsNullOrEmpty(Token) is false;

    public Uri? Host { get; set; }

    /// <summary>
    /// Represents if the host address is set.
    /// </summary>
    private bool HostIsSet => Host is not null;

    #endregion

    #region Constructors

    public Moodle(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #endregion

    #region Methods

    #region Helper

    private int DateTimeToUnixTimestamp(DateTime dateTime)
    {
        return Convert.ToInt32((TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
    }

    private string ParseFormat(MoodleFormat format)
    {
        switch (format)
        {
            case MoodleFormat.JSON:
                return "json";
            case MoodleFormat.XML:
                return "xml";
        }
        throw new ArgumentOutOfRangeException("format");
    }

    private string ParseMethod(MoodleMethod method)
    {
        switch (method)
        {
            case MoodleMethod.core_webservice_get_site_info:
                return "core_webservice_get_site_info";
            case MoodleMethod.core_user_get_users_by_field:
                return "core_user_get_users_by_field";
            case MoodleMethod.core_enrol_get_users_courses:
                return "core_enrol_get_users_courses";
            case MoodleMethod.core_user_create_users:
                return "core_user_create_users";
            case MoodleMethod.core_user_update_users:
                return "core_user_update_users";
            case MoodleMethod.core_user_delete_users:
                return "core_user_delete_users";
            case MoodleMethod.core_role_assign_roles:
                return "core_role_assign_roles";
            case MoodleMethod.core_role_unassign_roles:
                return "core_role_unassign_roles";
            case MoodleMethod.enrol_manual_enrol_users:
                return "enrol_manual_enrol_users";
            case MoodleMethod.core_group_add_group_members:
                return "core_group_add_group_members";
            case MoodleMethod.core_group_delete_group_members:
                return "core_group_delete_group_members";
            case MoodleMethod.core_course_get_categories:
                return "core_course_get_categories";
            case MoodleMethod.core_course_get_courses:
                return "core_course_get_courses";
            case MoodleMethod.core_course_get_contents:
                return "core_course_get_contents";
            case MoodleMethod.core_group_get_groups:
                return "core_group_get_groups";
            case MoodleMethod.core_group_get_course_groups:
                return "core_group_get_course_groups";
            case MoodleMethod.core_enrol_get_enrolled_users:
                return "core_enrol_get_enrolled_users";
            case MoodleMethod.core_course_create_courses:
                return "core_course_create_courses";
            case MoodleMethod.core_course_update_courses:
                return "core_course_update_courses";
            case MoodleMethod.core_grades_get_grades:
                return "core_grades_get_grades";
            case MoodleMethod.core_grades_update_grades:
                return "core_grades_update_grades";
            case MoodleMethod.core_grading_get_definitions:
                return "core_grading_get_definitions";
            case MoodleMethod.core_calendar_get_calendar_events:
                return "core_calendar_get_calendar_events";
            case MoodleMethod.core_calendar_create_calendar_events:
                return "core_calendar_create_calendar_events";
            case MoodleMethod.core_calendar_delete_calendar_events:
                return "core_calendar_delete_calendar_events";
            case MoodleMethod.default_:
                return "";
        }
        throw new ArgumentOutOfRangeException("method");
    }

    private StringBuilder GetBaseQuery(MoodleMethod moodleMethod, MoodleFormat moodleFormat = MoodleFormat.JSON)
    {
        if (TokenIsSet is false)
            throw new Exception("Token is not set");

        StringBuilder query = new StringBuilder("webservice/rest/server.php?wstoken=");
        query.Append(Token)
            .AppendFilterQuery("&wsfunction=", ParseMethod(moodleMethod))
            .AppendFilterQuery("&moodlewsrestformat=", ParseFormat(moodleFormat));

        return query;
    }

    #endregion

    #region Authentications

    /// <summary>
    /// Returns your Api Token needed to make any calls
    /// <para />
    /// service shortname - The service shortname is usually hardcoded in the pre-build service (db/service.php files).
    /// Moodle administrator will be able to edit shortnames for service created on the fly: MDL-29807.
    /// If you want to use the Mobile service, its shortname is moodle_mobile_app. Also useful to know,
    /// the database shortname field can be found in the table named external_services.
    /// </summary>
    /// <param names="userName"></param>
    /// <param names="password"></param>
    /// <param names="serviceHostName"></param>
    /// <returns></returns>
    public async Task<AuthentiactionResult> Login(string userName, string password, string serviceHostName = "moodle_mobile_app")
    {
        string query = $"login/token.php?username={userName}&password={password}&service={serviceHostName}";

        var result = await GetAuth<AuthToken>(query);

        if (result.Data?.Token.HasNoValue() ?? true)
        {
            return new AuthentiactionResult(result.Error);
        }
        else
        {
            Token = result.Data?.Token;
            return new AuthentiactionResult();
        }
    }

    #endregion

    #region System actions
    /// <summary>
    /// This API will return information about the site, web services users, and authorized API actions. This call is useful for getting site information and the capabilities of the web service user. 
    /// </summary>
    /// <param names="serviceHostNames">Returns information about a particular service.</param>
    /// <returns></returns>
    public Task<MoodleResponse<SiteInfo>> GetSiteInfo(string serviceHostName = "")
    {
        var query = GetBaseQuery(MoodleMethod.core_webservice_get_site_info);

        query.AppendFilterQueryIfHasValue("&serviceshortnames[0]=", serviceHostName);

        return Get<SiteInfo>(query);
    }

    #endregion

    #region User Actions 
    
    /// <summary>
    /// Retrieve users information for a specified unique field - If you want to do a user search, use GetUsers()
    /// 
    /// Avaiable Criteria:
    ///"id" (int) matching user id
    ///"lastname" (string) user last names (Note: you can use % for searching but it may be considerably slower!)
    ///"firstname" (string) user first names (Note: you can use % for searching but it may be considerably slower!)
    ///"idnumber" (string) matching user idnumber
    ///"username" (string) matching user username
    ///"email" (string) user email (Note: you can use % for searching but it may be considerably slower!)
    ///"auth" (string) matching user auth plugin
    /// </summary>
    /// <param names="field">Field of the search parameter.</param>
    /// <param names="criteriaValue">Values of the search term.</param>
    /// <returns></returns>
    public Task<MoodleResponse<User>> GetUsersByField(string field, params string[] values)
    {
        var query = GetBaseQuery(MoodleMethod.core_user_get_users_by_field);
        query.AppendFilterQuery("&field=", field);

        for (int i = 0; i < values.Length; i++)
        {
            query.Append("&values[").Append(i).Append("]=").Append(values[i]);
        }

        return Get<User>(query);
    }

    /// <summary>
    /// Get the list of courses where a user is enrolled in 
    /// </summary>
    /// <param names="userId"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Cources>> GetUserCourses(int userId)
    {
        var query = GetBaseQuery(MoodleMethod.core_enrol_get_users_courses);
        query.AppendFilterQuery("&userid=", userId);

        return Get<Cources>(query);
    }

    /// <summary>
    /// Create a User.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="auth"></param>
    /// <param name="idNumber"></param>
    /// <param name="language"></param>
    /// <param name="calendartye"></param>
    /// <param name="theme"></param>
    /// <param name="timezone"></param>
    /// <param name="mailFormat"></param>
    /// <param name="description"></param>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="firstNamePhonetic"></param>
    /// <param name="lastNamePhonetic"></param>
    /// <param name="middleName"></param>
    /// <param name="alternateName"></param>
    /// <param name="preferencesType"></param>
    /// <param name="preferencesValue"></param>
    /// <param name="customFieldsType"></param>
    /// <param name="customFieldsValue"></param>
    /// <returns></returns>
    public Task<MoodleResponse<NewUser>> CreateUser(string userName, string firstName, string lastName, string email, string password,
        string auth = "", string idNumber = "", string language = "", string calendartye = "", string theme = "",
        string timezone = "", string mailFormat = "", string description = "", string city = "", string country = "",
        string firstNamePhonetic = "", string lastNamePhonetic = "", string middleName = "", string alternateName = "",
        string preferencesType = "", string preferencesValue = "",
        string customFieldsType = "", string customFieldsValue = "")
    {
        var query = GetBaseQuery(MoodleMethod.core_user_create_users);
        query.AppendFilterQuery("&users[0][username]=", userName)
            .AppendFilterQuery("&users[0][password]=", password)
            .AppendFilterQuery("&users[0][firstname]=", firstName)
            .AppendFilterQuery("&users[0][lastname]=", lastName)
            .AppendFilterQuery("&users[0][email]=", email)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", auth)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", idNumber)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", language)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", calendartye)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", theme)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", timezone)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", mailFormat)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", description)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", city)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", country)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", firstNamePhonetic)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", lastNamePhonetic)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", middleName)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", alternateName)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", preferencesType)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", preferencesValue)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", customFieldsType)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", customFieldsValue);

        return Get<NewUser>(query);
    }


    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="auth"></param>
    /// <param name="idNumber"></param>
    /// <param name="language"></param>
    /// <param name="calendartye"></param>
    /// <param name="theme"></param>
    /// <param name="timezone"></param>
    /// <param name="mailFormat"></param>
    /// <param name="description"></param>
    /// <param name="city"></param>
    /// <param name="country"></param>
    /// <param name="firstNamePhonetic"></param>
    /// <param name="lastNamePhonetic"></param>
    /// <param name="middleName"></param>
    /// <param name="alternateName"></param>
    /// <param name="preferencesType"></param>
    /// <param name="preferencesValue"></param>
    /// <param name="customfieldsType"></param>
    /// <param name="customfieldsValue"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Success>> UpdateUser(int id, string userName = "", string firstName = "", string lastName = "",
        string email = "", string password = "", string auth = "", string idNumber = "", string language = "",
        string calendartye = "", string theme = "", string timezone = "", string mailFormat = "", string description = "", string city = "", string country = "",
        string firstNamePhonetic = "", string lastNamePhonetic = "", string middleName = "", string alternateName = "",
        string preferencesType = "", string preferencesValue = "",
        string customfieldsType = "", string customfieldsValue = "")
    {
        var query = GetBaseQuery(MoodleMethod.core_user_update_users);
        query.AppendFilterQuery("&users[0][id]=", id)
            .AppendFilterQueryIfHasValue("&users[0][username]=", userName)
            .AppendFilterQueryIfHasValue("&users[0][password]=", password)
            .AppendFilterQueryIfHasValue("&users[0][firstname]=", firstName)
            .AppendFilterQueryIfHasValue("&users[0][lastname]=", lastName)
            .AppendFilterQueryIfHasValue("&users[0][email]=", email)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", auth)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", idNumber)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", language)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", calendartye)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", theme)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", timezone)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", mailFormat)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", description)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", city)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", country)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", firstNamePhonetic)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", lastNamePhonetic)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", middleName)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", alternateName)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", preferencesType)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", preferencesValue)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", customfieldsType)
            .AppendFilterQueryIfHasValue("&users[0][auth]=", customfieldsValue);

        return Get<Success>(query);
    }

    /// <summary>
    /// elete a user
    /// </summary>
    /// <param names="id"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Success>> DeleteUser(int id)
    {
        var query = GetBaseQuery(MoodleMethod.core_user_update_users);
        query.AppendFilterQuery("&userids[0]=", id);

        return Get<Success>(query);
    }

    /// <summary>
    /// Manual role assignments. This call should be made in an array.
    /// </summary>
    /// <param name="roleId">
    /// <summary>Role to assign to the user</summary>
    /// </param>
    /// <param name="userId"></param>
    /// <param name="contextId"></param>
    /// <param name="contextLevel"></param>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Success>> AssignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_role_assign_roles);
        query.AppendFilterQuery("&assignments[0][roleid]=", roleId)
            .AppendFilterQuery("&assignments[0][userid]=", userId)
            .AppendFilterQueryIfHasValue("&assignments[0][contextid]=", contextId)
            .AppendFilterQueryIfHasValue("&assignments[0][contextlevel]=", contextLevel)
            .AppendFilterQueryIfHasValue("&assignments[0][instanceId]=", instanceId);

        return Get<Success>(query);
    }

    /// <summary>
    /// Manual role unassignments.
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    /// <param name="contextId"></param>
    /// <param name="contextLevel"></param>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Success>> UnassignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_role_unassign_roles);
        query.AppendFilterQuery("&unassignments[0][roleid]=", roleId)
            .AppendFilterQuery("&unassignments[0][userid]=", userId)
            .AppendFilterQueryIfHasValue("&unassignments[0][contextid]=", contextId)
            .AppendFilterQueryIfHasValue("&unassignments[0][contextlevel]=", contextLevel)
            .AppendFilterQueryIfHasValue("&unassignments[0][instanceId]=", instanceId);

        return Get<Success>(query);
    }

    #endregion

    #region Course Enrollment Actions

    public Task<MoodleResponse<Success>> EnrolUser(int roleId, int userId, int courceId, int? timeStart = null, int? timeEnd = null, int? suspend = null)
    {
        var query = GetBaseQuery(MoodleMethod.enrol_manual_enrol_users);
        query.AppendFilterQuery("&enrolments[0][roleid]=", roleId)
            .AppendFilterQuery("&enrolments[0][userid]=", userId)
            .AppendFilterQuery("&enrolments[0][courceid]=", courceId)
            .AppendFilterQueryIfHasValue("&enrolments[0][timestart]=", timeStart)
            .AppendFilterQueryIfHasValue("&enrolments[0][timeend]=", timeEnd)
            .AppendFilterQueryIfHasValue("&enrolments[0][suspend]=", suspend);

        return Get<Success>(query);
    }

    public Task<MoodleResponse<Success>> AddGroupMember(int groupId, int userId)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_add_group_members);
        query.AppendFilterQuery("&members[0][groupid]=", groupId)
            .AppendFilterQuery("&members[0][userid]=", userId);

        return Get<Success>(query);
    }

    public Task<MoodleResponse<Success>> DeleteGroupMember(int groupId, int userId)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_delete_group_members);
        query.AppendFilterQuery("&members[0][groupid]=", groupId)
            .AppendFilterQuery("&members[0][userid]=", userId);

        return Get<Success>(query);
    }


    #endregion

    #region Course Actions

    /// <summary>
    /// Get a listing of categories in the system.
    /// </summary>
    /// <param names="criteriaKey">
    /// <summary>
    /// criteria[0][key] - The category column to search, expected keys (value format) are:"id" (int) the category id,"names" (string)
    ///  the category names,"parent" (int) the parent category id,"idnumber" (string) category idnumber - user must have 'moodle/category:manage'
    ///  to search on idnumber,"visible" (int) whether the returned categories must be visible or hidden.
    ///  If the key is not passed, then the function return all categories that the user can see. - user must have 'moodle/category:manage'
    ///  or 'moodle/category:viewhiddencategories' to search on visible,"theme" (string) only return the categories having this theme
    ///  - user must have 'moodle/category:manage' to search on theme
    /// </summary>
    /// </param>
    /// <param names="criteriaValue"><summary>Criteria[0][value] - The value to match</summary></param>
    /// <param names="addSubCategories"><summary>Return the sub categories infos (1 - default) otherwise only the category info (0)</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<Category>> GetCategories(string criteriaKey, string criteriaValue, int addSubCategories = 1)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_get_categories);
        query.AppendFilterQuery("&criteria[0][key]=", criteriaKey)
            .AppendFilterQuery("&criteria[0][value]=", criteriaValue)
            .AppendFilterQueryIf(addSubCategories != 1, "&addsubcategories=", addSubCategories);

        return Get<Category>(query);
    }

    /// <summary>
    /// Get a listing of courses in the system.
    /// </summary>
    /// <param names="options"><summary>List of course id.If empty return all courses except front page course.</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<Course>> GetCourses(int? options = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_get_courses);
        query.AppendFilterQueryIfHasValue("&addsubcategories=", options);

        return Get<Course>(query);
    }

    /// <summary>
    /// Get course contents
    /// </summary>
    /// <param names="course_id"><summary>Course Id</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<Content>> GetContents(int courseId)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_get_contents);
        query.AppendFilterQuery("&courseid=", courseId);

        return Get<Content>(query);
    }

    /// <summary>
    /// Returns group details. 
    /// </summary>
    /// <param names="groupId">Group Id</param>
    /// <returns></returns>
    public Task<MoodleResponse<Group>> GetGroup(int groupId)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_get_groups);
        query.AppendFilterQuery("&groupids[0]=", groupId);

        return Get<Group>(query);
    }
    /// <summary>
    /// Returns group details. 
    /// </summary>
    /// <param names="groupIds"><summary>Group Ids</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<Group>> GetGroups(int[] groupIds)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_get_groups);

        for (int i = 0; i < groupIds.Length; i++)
        {
            query.Append("&groupids[").Append(i).Append("]=").Append(groupIds[i]);
        }

        return Get<Group>(query);
    }

    /// <summary>
    /// Returns all groups in specified course.
    /// </summary>
    /// <param names="courseId"><summary>Course Id</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<Group>> GetCourseGroups(int courseId)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_get_course_groups);
        query.AppendFilterQuery("&courseid=", courseId);

        return Get<Group>(query);
    }

    /// <summary>
    /// Get enrolled users by course id. 
    /// </summary>
    /// <param names="courseId"></param>
    /// <returns></returns>
    public Task<MoodleResponse<EnrolledUser>> GetEnrolledUsersByCourse(int courseId)
    {
        var query = GetBaseQuery(MoodleMethod.core_enrol_get_enrolled_users);
        query.AppendFilterQuery("&courseid=", courseId);

        return Get<EnrolledUser>(query);
    }

    /// <summary>
    /// Create new course
    /// </summary>
    /// <param names="fullName"><summary>Full names of the course</summary></param>
    /// <param names="shortName"><summary>Shortname of the course</summary></param>
    /// <param names="categoryId"><summary>Category ID of the course</summary></param>
    /// <param names="idNumber"><summary>Optional //id number</summary></param>
    /// <param names="summary"><summary>Optional //summary</summary></param>
    /// <param names="summaryFormat"><summary>Default to "1" //summary format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)</summary></param>
    /// <param names="format"><summary>Default to "topics" //course format: weeks, topics, social, site,..</summary></param>
    /// <param names="showGrades"><summary>Default to "0" //1 if grades are shown, otherwise 0</summary></param>
    /// <param names="newsItems"><summary>Default to "0" //number of recent items appearing on the course page</summary></param>
    /// <param names="startDate"><summary>Optional //timestamp when the course start</summary></param>
    /// <param names="numSections"><summary>Optional //(deprecated, use courseformatoptions) number of weeks/topics</summary></param>
    /// <param names="maxBytes"><summary>Default to "104857600" //largest size of file that can be uploaded into the course</summary></param>
    /// <param names="showReports"><summary>Default to "1" //are activity report shown (yes = 1, no =0)</summary></param>
    /// <param names="visible"><summary>Optional //1: available to student, 0:not available</summary></param>
    /// <param names="hiddenSections"><summary>Optional //(deprecated, use courseformatoptions) How the hidden sections in the course are displayed to students</summary></param>
    /// <param names="groupMode"><summary>Default to "0" //no group, separate, visible</summary></param>
    /// <param names="groupModeForce"><summary>Default to "0" //1: yes, 0: no</summary></param>
    /// <param names="defaultGroupingId"><summary>Default to "0" //default grouping id</summary></param>
    /// <param names="enableCompletion"><summary>Optional //Enabled, control via completion and activity settings. Disabled, not shown in activity settings.</summary></param>
    /// <param names="completeNotify"><summary>Optional //1: yes 0: no</summary></param>
    /// <param names="language"><summary>//forced course language</summary></param>
    /// <param names="forceTheme"><summary>Optional //names of the force theme</summary></param>
    /// <param names="courcCourseformatoption"><summary>Optional //additional options for particular course format list of ( object { names string //course format option names
    ///value string //course format option value } )} )</summary></param>
    /// <returns></returns>
    public Task<MoodleResponse<NewCourse>> CreateCourse(string fullName, string shortName, int categoryId,
        string idNumber = "", string summary = "", int summaryFormat = 1, string format = "", int showGrades = 0, int newsItems = 0,
        DateTime startdate = default, int numSections = int.MaxValue, int maxBytes = 104857600, int showReports = 1,
        int visible = 0, int hiddenSections = int.MaxValue, int groupMode = 0,
        int groupModeForce = 0, int defaultGroupingId = 0, int enableCompletion = int.MaxValue,
        int completeNotify = 0, string language = "", string forceTheme = "",
        string courcCourseformatoption = ""/*not implemented*/)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_create_courses);
        query.AppendFilterQuery("&courses[0][fullname]=", fullName)
                .AppendFilterQuery("&courses[0][shortname]=", shortName)
                .AppendFilterQuery("&courses[0][categoryid]=", categoryId)
                .AppendFilterQueryIfHasValue("&courses[0][idnumber]=", idNumber)
                .AppendFilterQueryIfHasValue("&courses[0][summary]=", summary)
                .AppendFilterQueryIf(summaryFormat != 1, "&courses[0][summaryformat]=", summaryFormat)
                .AppendFilterQueryIfHasValue("&courses[0][format]=", format)
                .AppendFilterQueryIf(showGrades != 0, "&courses[0][showgrades]=", showGrades)
                .AppendFilterQueryIf(startdate.Equals(default(DateTime)) is false, "&courses[0][startdate]=", DateTimeToUnixTimestamp(startdate))
                .AppendFilterQueryIf(newsItems != 0, "&courses[0][newsitems]=", newsItems)
                .AppendFilterQueryIf(numSections != int.MaxValue, "&courses[0][numsections]=", numSections)
                .AppendFilterQueryIf(maxBytes != 104857600, "&courses[0][maxbytes]=", maxBytes)
                .AppendFilterQueryIf(showReports != 1, "&courses[0][showreports]=", showReports)
                .AppendFilterQueryIf(visible != 0, "&courses[0][visible]=", visible)
                .AppendFilterQueryIf(hiddenSections != int.MaxValue, "&courses[0][hiddensections]=", hiddenSections)
                .AppendFilterQueryIf(groupMode != 0, "&courses[0][groupmode]=", groupMode)
                .AppendFilterQueryIf(groupModeForce != 0, "&courses[0][groupmodeforce]=", groupModeForce)
                .AppendFilterQueryIf(defaultGroupingId != 0, "&courses[0][defaultgroupingid]=", defaultGroupingId)
                .AppendFilterQueryIf(enableCompletion != int.MaxValue, "&courses[0][enablecompletion]=", enableCompletion)
                .AppendFilterQueryIf(completeNotify != 0, "&courses[0][completenotify]=", completeNotify)
                .AppendFilterQueryIfHasValue("&courses[0][lang]=", language)
                .AppendFilterQueryIfHasValue("&courses[0][forcetheme]=", forceTheme);

        return Get<NewCourse>(query);
    }

    /// <summary>
    /// Create new courses
    /// </summary>
    /// <param name="courses"></param>
    /// <returns></returns>
    public Task<MoodleResponse<NewCourse>> CreateCourses((string FullName, string ShortName, int CategoryId)[] courses)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_create_courses);
        for (int i = 0; i < courses.Length; i++)
        {
            query.Append("&courses[").Append(i).Append("][fullname]=").Append(courses[i].FullName)
                .Append("&courses[").Append(i).Append("][shortname]=").Append(courses[i].ShortName)
                .Append("&courses[").Append(i).Append("][categoryid]=").Append(courses[i].CategoryId);
        }
        return Get<NewCourse>(query);
    }

    public Task<MoodleResponse<UpdateCourseRoot>> UpdateCourse(int id, string fullName = "", string shortName = "", int categoryId = int.MaxValue,
        string idNumber = "", string summary = "", int summaryFormat = 1, string format = "", int showGrades = 0, int newsItems = 0,
        DateTime startdate = default, int numsections = int.MaxValue, int maxbytes = 104857600, int showreports = 1,
        int visible = 0, int hiddenSections = int.MaxValue, int groupMode = 0,
        int groupModeForce = 0, int defaultGroupingId = 0, int enableCompletion = int.MaxValue,
        int completenotify = 0, string language = "", string forceTheme = "",
        string courcCourseformatoption = ""/*not implemented*/)
    {
        var query = GetBaseQuery(MoodleMethod.core_course_update_courses);
        query.AppendFilterQuery("&courses[0][id]=", id)
                .AppendFilterQueryIfHasValue("&courses[0][fullname]=", fullName)
                .AppendFilterQueryIfHasValue("&courses[0][shortname]=", shortName)
                .AppendFilterQueryIf(categoryId != int.MaxValue, "&courses[0][categoryid]=", categoryId)
                .AppendFilterQueryIfHasValue("&courses[0][idnumber]=", idNumber)
                .AppendFilterQueryIfHasValue("&courses[0][summary]=", summary)
                .AppendFilterQueryIf(summaryFormat != 1, "&courses[0][summaryformat]=", summaryFormat)
                .AppendFilterQueryIfHasValue("&courses[0][format]=", format)
                .AppendFilterQueryIf(showGrades != 0, "&courses[0][showgrades]=", showGrades)
                .AppendFilterQueryIf(startdate.Equals(default(DateTime)) is false, "&courses[0][startdate]=", DateTimeToUnixTimestamp(startdate))
                .AppendFilterQueryIf(newsItems != 0, "&courses[0][newsitems]=", newsItems)
                .AppendFilterQueryIf(numsections != int.MaxValue, "&courses[0][numsections]=", numsections)
                .AppendFilterQueryIf(maxbytes != 104857600, "&courses[0][maxbytes]=", maxbytes)
                .AppendFilterQueryIf(showreports != 1, "&courses[0][showreports]=", showreports)
                .AppendFilterQueryIf(visible != 0, "&courses[0][visible]=", visible)
                .AppendFilterQueryIf(hiddenSections != int.MaxValue, "&courses[0][hiddensections]=", hiddenSections)
                .AppendFilterQueryIf(groupMode != 0, "&courses[0][groupmode]=", groupMode)
                .AppendFilterQueryIf(groupModeForce != 0, "&courses[0][groupmodeforce]=", groupModeForce)
                .AppendFilterQueryIf(defaultGroupingId != 0, "&courses[0][defaultgroupingid]=", defaultGroupingId)
                .AppendFilterQueryIf(enableCompletion != int.MaxValue, "&courses[0][enablecompletion]=", enableCompletion)
                .AppendFilterQueryIf(completenotify != 0, "&courses[0][completenotify]=", completenotify)
                .AppendFilterQueryIfHasValue("&courses[0][lang]=", language)
                .AppendFilterQueryIfHasValue("&courses[0][forcetheme]=", forceTheme);

        return Get<UpdateCourseRoot>(query);
    }

    #endregion

    #region Grade Actions

    /// <summary>
    /// Returns grade item details and optionally student grades. 
    /// </summary>
    /// <param names="criteria_key"></param>
    /// <param names="criteria_value"></param>
    /// <param names="addSubCategories"></param>
    /// <returns></returns>
    public Task<MoodleResponse<Category>> GetGrades(int courseId, string component = "", int activityid = int.MaxValue, string[]? userIds = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_grades_get_grades);
        query.AppendFilterQuery("&courseid=", courseId)
                .AppendFilterQueryIfHasValue("&component=", component)
                .AppendFilterQueryIf(activityid != int.MaxValue, "&activityid=", activityid);

        if (userIds is not null && userIds.Length > 0)
        {
            for (int i = 0; i < userIds.Length; i++)
            {
                query.Append("&userids[").Append(i).Append("]=").Append(userIds[i]);
            }
        }

        return Get<Category>(query);
    }



    #endregion

    #region Calander Actions


    public Task<MoodleResponse<ListOfEvents>> GetCalanderEvents(int[]? groupids = null, int[]? courseIds = null, int[]? eventIds = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_calendar_get_calendar_events);

        if (groupids is not null)
            for (int i = 0; i < groupids.Length; i++)
                query.Append("&events[groupids][").Append(i).Append("]=").Append(groupids[i]);

        if (courseIds is not null)
            for (int i = 0; i < courseIds.Length; i++)
                query.Append("&events[courseids][").Append(i).Append("]=").Append(courseIds[i]);

        if (eventIds is not null)
            for (int i = 0; i < eventIds.Length; i++)
                query.Append("&events[eventids][").Append(i).Append("]=").Append(eventIds[i]);

        return Get<ListOfEvents>(query);
    }


    public Task<MoodleResponse<ListOfEvents>> CreateCalanderEvents(string[] names, string[]? descriptions = null,
         int[]? formats = null, int[]? groupIds = null, int[]? courseIds = null, int[]? repeats = null,
         string[]? eventTypes = null, DateTime[]? timeStarts = null, TimeSpan[]? timeDurations = null,
         int[]? visible = null, int[]? sequences = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_calendar_create_calendar_events);

        for (int i = 0; i < names.Length; i++)
            query.Append("&events[").Append(i).Append("][name]=").Append(names[i]);

        if (groupIds is not null)
            for (int i = 0; i < groupIds.Length; i++)
                query.Append("&events[").Append(i).Append("][groupid]=").Append(groupIds[i]);

        if (courseIds is not null)
            for (int i = 0; i < courseIds.Length; i++)
                query.Append("&events[").Append(i).Append("][courseid]=").Append(courseIds[i]);

        if (descriptions is not null)
            for (int i = 0; i < descriptions.Length; i++)
                query.Append("&events[").Append(i).Append("][description]=").Append(descriptions[i]);

        if (formats is not null)
            for (int i = 0; i < formats.Length; i++)
                query.Append("&events[").Append(i).Append("][format]=").Append(formats[i]);

        if (repeats is not null)
            for (int i = 0; i < repeats.Length; i++)
                query.Append("&events[").Append(i).Append("][repeats]=").Append(repeats[i]);

        if (eventTypes is not null)
            for (int i = 0; i < eventTypes.Length; i++)
                query.Append("&events[").Append(i).Append("][eventtypes]=").Append(eventTypes[i]);

        if (timeStarts is not null)
            for (int i = 0; i < timeStarts.Length; i++)
                query.Append("&events[").Append(i).Append("][timestart]=").Append(DateTimeToUnixTimestamp(timeStarts[i]));

        if (timeDurations != null)
            for (int i = 0; i < timeDurations.Length; i++)
                query.Append("&events[").Append(i).Append("][timeduration]=").Append(timeDurations[i].TotalSeconds);

        if (visible is not null)
            for (int i = 0; i < visible.Length; i++)
                query.Append("&events[").Append(i).Append("][visible]=").Append(visible[i]);

        if (sequences is not null)
            for (int i = 0; i < sequences.Length; i++)
                query.Append("&events[").Append(i).Append("][sequence]=").Append(sequences[i]);

        return Get<ListOfEvents>(query);
    }


    public Task<MoodleResponse<ListOfEvents>> DeleteCalanderEvents(int[]? eventIds, int[]? repeats, string[]? descriptions = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_calendar_delete_calendar_events);

        if (repeats is not null)
            for (int i = 0; i < repeats.Length; i++)
                query.Append("&events[").Append(i).Append("][repeat]=").Append(repeats[i]);

        if (eventIds is not null)
            for (int i = 0; i < eventIds.Length; i++)
                query.Append("&events[").Append(i).Append("][eventid]=").Append(eventIds[i]);


        if (descriptions is not null)
            for (int i = 0; i < descriptions.Length; i++)
                query.Append("&events[").Append(i).Append("][description]=").Append(descriptions[i]);

        return Get<ListOfEvents>(query);
    }

    #endregion

    #region Group Actions

    public Task<MoodleResponse<Group>> CreateGroups(string[]? names = null, int[]? courseids = null, string[]? descriptions = null,
        int[]? descriptionFormats = null, string[]? enrolmentKeys = null, string[]? idNumbers = null)
    {
        var query = GetBaseQuery(MoodleMethod.core_group_create_groups);

        if (names is not null)
            for (int i = 0; i < names.Length; i++)
                query.Append("&groups[").Append(i).Append("][name]=").Append(names[i]);

        if (courseids is not null)
            for (int i = 0; i < courseids.Length; i++)
                query.Append("&groups[").Append(i).Append("][courseid]=").Append(courseids[i]);

        if (descriptions is not null)
            for (int i = 0; i < descriptions.Length; i++)
                query.Append("&groups[").Append(i).Append("][description]=").Append(descriptions[i]);

        if (descriptionFormats is not null)
            for (int i = 0; i < descriptionFormats.Length; i++)
                query.Append("&groups[").Append(i).Append("][descriptionformat]=").Append(descriptionFormats[i]);

        if (enrolmentKeys is not null)
            for (int i = 0; i < enrolmentKeys.Length; i++)
                query.Append("&groups[").Append(i).Append("][enrolmentkey]=").Append(enrolmentKeys[i]);

        if (idNumbers is not null)
            for (int i = 0; i < idNumbers.Length; i++)
                query.Append("&groups[").Append(i).Append("][idnumber]=").Append(idNumbers[i]);


        return Get<Group>(query);
    }

    #endregion

    #region Getters

    private async Task<AuthentiactionResponse<T>> GetAuth<T>(string query) where T : IDataModel
    {
        if (HostIsSet is false)
            throw new Exception("Host is not set");

        if (Host!.Scheme == "https")
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var requestUri = new Uri(Host, query);
        var data = await _httpClient.GetStringAsync(requestUri);
        return new AuthentiactionResponse<T>(data);
    }


    private async Task<MoodleResponse<T>> Get<T>(string query) where T : IDataModel
    {
        if (HostIsSet is false)
            throw new Exception("Host is not set");

        if (Host!.Scheme == "https")
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var requestUri = new Uri(Host, query);
        var data = await _httpClient.GetStringAsync(requestUri);
        return new MoodleResponse<T>(data);
    }

    private async Task<MoodleResponse<T>> Get<T>(StringBuilder query) where T : IDataModel
    {
        return await Get<T>(query.ToString());
    }
    #endregion

    #region Setters

    public void SetHost(string url)
    {
        Host = new Uri(url);
    }

    #endregion

    #endregion
}