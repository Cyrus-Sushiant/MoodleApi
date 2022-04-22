using MoodleApi.Extensions;
using System.Net;
using System.Text;

namespace MoodleApi
{
    public class MoodleApi
    {
        #region Properties

        /// <summary>
        /// field that holds your api token
        /// </summary>
        private string? _token;

        /// <summary>
        /// This property sets you Api token.
        /// </summary>
        public string? Token
        {
            get { return _token; }
            set { _token = value; }
        }

        /// <summary>
        /// Repressents if the token is set.
        /// </summary>
        private bool TokenIsSet => string.IsNullOrEmpty(_token) is false;

        private Uri? _host;

        public Uri? Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Represents if the host address is set.
        /// </summary>
        private bool HostIsSet => Host is not null;

        #endregion

        public MoodleApi()
        {
        }

        public MoodleApi(string uri, string token)
        {
            _token = token;
            _host = new Uri(uri);
        }

        #region Methods

        #region Helper

        private int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return Convert.ToInt32((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
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
                case MoodleMethod.core_user_get_users:
                    return "core_user_get_users";
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
        /// <param names="username"></param>
        /// <param names="password"></param>
        /// <param names="serviceHostName"></param>
        /// <returns></returns>
        public Task<AuthentiactionResponse<AuthToken>> GetApiToken(string username, string password, string serviceHostName)
        {
            if (HostIsSet is false)
                throw new Exception("Host is not set");

            string query = $"login/token.php?username={username}&password={password}&service={serviceHostName}";

            return GetAuth<AuthToken>(query);
        }
        #endregion

        #region System actions
        /// <summary>
        /// This API will return information about the site, web services users, and authorized API actions. This call is useful for getting site information and the capabilities of the web service user. 
        /// </summary>
        /// <param names="serviceHostNames">Returns information about a particular service.</param>
        /// <returns></returns>
        public Task<ApiResponse<Site_info>> GetSiteInfo(string serviceHostName = "")
        {
            var query = GetBaseQuery(MoodleMethod.core_webservice_get_site_info);

            query.AppendFilterQueryIfHasValue("&serviceshortnames[0]=", serviceHostName);

            return Get<Site_info>(query);
        }

        #endregion

        #region User Actions 
        /// <summary>
        /// Search for users matching the parameters of the call. This call will return matching user accounts with profile fields.
        ///  The key/value pairs to be considered in user search. Values can not be empty. Specify different keys only once
        ///  (fullname =&gt; 'user1', auth =&gt; 'manual', ...) - key occurences are forbidden. The search is executed with AND operator on the criterias.
        ///  Invalid criterias (keys) are ignored, the search is still executed on the valid criterias. You can search without criteria,
        ///  but the function is not designed for it. It could very slow or timeout. The function is designed to search some specific users.
        /// <para />
        /// "id" (int) matching user id<para />
        ///"lastname" (string) user last names (Note: you can use % for searching but it may be considerably slower!)<para />
        ///"firstname" (string) user first names (Note: you can use % for searching but it may be considerably slower!)<para />
        ///"idnumber" (string) matching user idnumber<para />
        ///"username" (string) matching user username<para />
        ///"email" (string) user email (Note: you can use % for searching but it may be considerably slower!)<para />
        ///"auth" (string) matching user auth plugin<para />
        /// </summary>
        /// <param names="criteria_key0">Key of the first search parameter.</param>
        /// <param names="criteria_value0">Value of the first search term.</param>
        /// <param names="criteria_key1">Key of the second search parameter.</param>
        /// <param names="criteria_value1">Value of the second search term.</param>
        /// <returns></returns>
        public Task<ApiResponse<Users>> GetUsers(string criteria_key0, string criteria_value0, string? criteria_key1 = null, string? criteria_value1 = null)
        {
            var query = GetBaseQuery(MoodleMethod.core_user_get_users);

            query.Append("&criteria[0][key]=").Append(criteria_key0)
                .Append("&criteria[0][value]=").Append(criteria_value0);

            if (criteria_key1.HasValue() && criteria_value1.HasValue())
            {
                query.AppendFilterQuery("&criteria[1][key]=", criteria_key1!)
                    .AppendFilterQuery("&criteria[1][value]=", criteria_value1!);
            }

            return Get<Users>(query);
        }

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
        /// <param names="criteria_key">Key of the first search parameter.</param>
        /// <param names="criteria_value">Value of the first search term.</param>
        /// <returns></returns>
        public Task<ApiResponse<Users>> GetUsersByField(string criteria_key, string criteria_value)
        {
            var query = GetBaseQuery(MoodleMethod.core_user_get_users_by_field);
            query.AppendFilterQuery("&criteria[0][key]=", criteria_key)
                .AppendFilterQuery("&criteria[0][value]=", criteria_value);

            return Get<Users>(query);
        }

        /// <summary>
        /// Get the list of courses where a user is enrolled in 
        /// </summary>
        /// <param names="userid"></param>
        /// <returns></returns>
        public Task<ApiResponse<Cources>> GetUserCourses(int userId)
        {
            var query = GetBaseQuery(MoodleMethod.core_enrol_get_users_courses);
            query.AppendFilterQuery("&userid=", userId);

            return Get<Cources>(query);
        }

        /// <summary>
        /// Create a User.
        /// </summary>
        /// <param names="username"></param>
        /// <param names="firstname"></param>
        /// <param names="lastname"></param>
        /// <param names="email"></param>
        /// <param names="password"></param>
        /// <param names="auth"></param>
        /// <param names="idnumber"></param>
        /// <param names="lang"></param>
        /// <param names="calendartye"></param>
        /// <param names="theme"></param>
        /// <param names="timezone"></param>
        /// <param names="mailformat"></param>
        /// <param names="description"></param>
        /// <param names="city"></param>
        /// <param names="country"></param>
        /// <param names="firstnamephonetic"></param>
        /// <param names="lastnamephonetic"></param>
        /// <param names="middlename"></param>
        /// <param names="alternatename"></param>
        /// <param names="preferences_type"></param>
        /// <param names="preferences_value"></param>
        /// <param names="customfields_type"></param>
        /// <param names="customfields_value"></param>
        /// <returns></returns>
        public Task<ApiResponse<NewUser>> CreateUser(string username, string firstname, string lastname,
            string email, string password,
            string auth = "", string idnumber = "", string lang = "", string calendartye = "", string theme = "",
            string timezone = "",
            string mailformat = "", string description = "", string city = "", string country = "",
            string firstnamephonetic = "",
            string lastnamephonetic = "", string middlename = "", string alternatename = "",
            string preferences_type = "", string preferences_value = "",
            string customfields_type = "", string customfields_value = "")
        {
            var query = GetBaseQuery(MoodleMethod.core_user_create_users);
            query.AppendFilterQuery("&users[0][username]=", username)
                .AppendFilterQuery("&users[0][password]=", password)
                .AppendFilterQuery("&users[0][firstname]=", firstname)
                .AppendFilterQuery("&users[0][lastname]=", lastname)
                .AppendFilterQuery("&users[0][email]=", email)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", auth)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", idnumber)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", lang)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", calendartye)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", theme)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", timezone)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", mailformat)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", description)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", city)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", country)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", firstnamephonetic)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", lastnamephonetic)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", middlename)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", alternatename)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", preferences_type)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", preferences_value)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", customfields_type)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", customfields_value);

            return Get<NewUser>(query);
        }


        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param names="id"></param>
        /// <param names="username"></param>
        /// <param names="firstname"></param>
        /// <param names="lastname"></param>
        /// <param names="email"></param>
        /// <param names="password"></param>
        /// <param names="auth"></param>
        /// <param names="idnumber"></param>
        /// <param names="lang"></param>
        /// <param names="calendartye"></param>
        /// <param names="theme"></param>
        /// <param names="timezone"></param>
        /// <param names="mailformat"></param>
        /// <param names="description"></param>
        /// <param names="city"></param>
        /// <param names="country"></param>
        /// <param names="firstnamephonetic"></param>
        /// <param names="lastnamephonetic"></param>
        /// <param names="middlename"></param>
        /// <param names="alternatename"></param>
        /// <param names="preferences_type"></param>
        /// <param names="preferences_value"></param>
        /// <param names="customfields_type"></param>
        /// <param names="customfields_value"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> UpdateUser(int id, string username = "", string firstname = "",
            string lastname = "",
            string email = "", string password = "", string auth = "", string idnumber = "", string lang = "",
            string calendartye = "", string theme = "",
            string timezone = "", string mailformat = "", string description = "", string city = "", string country = "",
            string firstnamephonetic = "",
            string lastnamephonetic = "", string middlename = "", string alternatename = "",
            string preferences_type = "", string preferences_value = "",
            string customfields_type = "", string customfields_value = "")
        {
            var query = GetBaseQuery(MoodleMethod.core_user_update_users);
            query.AppendFilterQuery("&users[0][id]=", id)
                .AppendFilterQueryIfHasValue("&users[0][username]=", username)
                .AppendFilterQueryIfHasValue("&users[0][password]=", password)
                .AppendFilterQueryIfHasValue("&users[0][firstname]=", firstname)
                .AppendFilterQueryIfHasValue("&users[0][lastname]=", lastname)
                .AppendFilterQueryIfHasValue("&users[0][email]=", email)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", auth)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", idnumber)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", lang)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", calendartye)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", theme)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", timezone)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", mailformat)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", description)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", city)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", country)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", firstnamephonetic)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", lastnamephonetic)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", middlename)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", alternatename)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", preferences_type)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", preferences_value)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", customfields_type)
                .AppendFilterQueryIfHasValue("&users[0][auth]=", customfields_value);

            return Get<Success>(query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param names="id"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> DeleteUser(int id)
        {
            var query = GetBaseQuery(MoodleMethod.core_user_update_users);
            query.AppendFilterQuery("&userids[0]=", id);

            return Get<Success>(query);
        }

        /// <summary>
        /// Manual role assignments. This call should be made in an array. 
        /// </summary>
        /// <param names="role_id">
        /// <summary>Role to assign to the user</summary>
        /// </param>
        /// <param names="user_id"></param>
        /// <param names="context_id"></param>
        /// <param names="context_level"></param>
        /// <param names="instance_id"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> AssignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
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
        /// 
        /// </summary>
        /// <param names="role_id"></param>
        /// <param names="user_id"></param>
        /// <param names="context_id"></param>
        /// <param names="context_level"></param>
        /// <param names="instance_id"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> UnassignRoles(int roleId, int userId, string contextId = "", string contextLevel = "", int? instanceId = null)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param names="role_id"></param>
        /// <param names="user_id"></param>
        /// <param names="cource_id"></param>
        /// <param names="timestart"></param>
        /// <param names="timeend"></param>
        /// <param names="suspend"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> EnrolUser(int roleId, int userId, int courceId, int? timeStart = null, int? timeEnd = null, int? suspend = null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param names="group_id"></param>
        /// <param names="user_id"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> AddGroupMember(int groupId, int userId)
        {
            var query = GetBaseQuery(MoodleMethod.core_group_add_group_members);
            query.AppendFilterQuery("&members[0][groupid]=", groupId)
                .AppendFilterQuery("&members[0][userid]=", userId);

            return Get<Success>(query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param names="group_id"></param>
        /// <param names="user_id"></param>
        /// <returns></returns>
        public Task<ApiResponse<Success>> DeleteGroupMember(int groupId, int userId)
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
        /// <param names="criteria_key">
        /// <summary>
        /// criteria[0][key] - The category column to search, expected keys (value format) are:"id" (int) the category id,"names" (string)
        ///  the category names,"parent" (int) the parent category id,"idnumber" (string) category idnumber - user must have 'moodle/category:manage'
        ///  to search on idnumber,"visible" (int) whether the returned categories must be visible or hidden.
        ///  If the key is not passed, then the function return all categories that the user can see. - user must have 'moodle/category:manage'
        ///  or 'moodle/category:viewhiddencategories' to search on visible,"theme" (string) only return the categories having this theme
        ///  - user must have 'moodle/category:manage' to search on theme
        /// </summary>
        /// </param>
        /// <param names="criteria_value"><summary>Criteria[0][value] - The value to match</summary></param>
        /// <param names="addSubCategories"><summary>Return the sub categories infos (1 - default) otherwise only the category info (0)</summary></param>
        /// <returns></returns>
        public Task<ApiResponse<Category>> GetCategories(string criteriaKey, string criteriaValue, int addSubCategories = 1)
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
        public Task<ApiResponse<Course>> GetCourses(int? options = null)
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
        public Task<ApiResponse<Content>> GetContents(int courseId)
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
        public Task<ApiResponse<Group>> GetGroup(int groupId)
        {
            var query = GetBaseQuery(MoodleMethod.core_group_get_groups);
            query.AppendFilterQuery("&groupids[0]=", groupId);

            return Get<Group>(query);
        }
        /// <summary>
        /// Returns group details. 
        /// </summary>
        /// <param names="group_ids"><summary>Group Ids</summary></param>
        /// <returns></returns>
        public Task<ApiResponse<Group>> GetGroups(int[] groupIds)
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
        public Task<ApiResponse<Group>> GetCourseGroups(int courseId)
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
        public Task<ApiResponse<EnrolledUser>> GetEnrolledUsersByCourse(int courseId)
        {
            var query = GetBaseQuery(MoodleMethod.core_enrol_get_enrolled_users);
            query.AppendFilterQuery("&courseid=", courseId);

            return Get<EnrolledUser>(query);
        }

        /// <summary>
        /// Create new course
        /// </summary>
        /// <param names="fullname"><summary>Full names of the course</summary></param>
        /// <param names="shortname"><summary>Shortname of the course</summary></param>
        /// <param names="category_id"><summary>Category ID of the course</summary></param>
        /// <param names="idnumber"><summary>Optional //id number</summary></param>
        /// <param names="summary"><summary>Optional //summary</summary></param>
        /// <param names="summaryformat"><summary>Default to "1" //summary format (1 = HTML, 0 = MOODLE, 2 = PLAIN or 4 = MARKDOWN)</summary></param>
        /// <param names="format"><summary>Default to "topics" //course format: weeks, topics, social, site,..</summary></param>
        /// <param names="showgrades"><summary>Default to "0" //1 if grades are shown, otherwise 0</summary></param>
        /// <param names="newsitems"><summary>Default to "0" //number of recent items appearing on the course page</summary></param>
        /// <param names="startdate"><summary>Optional //timestamp when the course start</summary></param>
        /// <param names="numsections"><summary>Optional //(deprecated, use courseformatoptions) number of weeks/topics</summary></param>
        /// <param names="maxbytes"><summary>Default to "104857600" //largest size of file that can be uploaded into the course</summary></param>
        /// <param names="showreports"><summary>Default to "1" //are activity report shown (yes = 1, no =0)</summary></param>
        /// <param names="visible"><summary>Optional //1: available to student, 0:not available</summary></param>
        /// <param names="hiddensections"><summary>Optional //(deprecated, use courseformatoptions) How the hidden sections in the course are displayed to students</summary></param>
        /// <param names="groupmode"><summary>Default to "0" //no group, separate, visible</summary></param>
        /// <param names="groupmodeforce"><summary>Default to "0" //1: yes, 0: no</summary></param>
        /// <param names="defaultgroupingid"><summary>Default to "0" //default grouping id</summary></param>
        /// <param names="enablecompletion"><summary>Optional //Enabled, control via completion and activity settings. Disabled, not shown in activity settings.</summary></param>
        /// <param names="completenotify"><summary>Optional //1: yes 0: no</summary></param>
        /// <param names="lang"><summary>//forced course language</summary></param>
        /// <param names="forcetheme"><summary>Optional //names of the force theme</summary></param>
        /// <param names="courcCourseformatoption"><summary>Optional //additional options for particular course format list of ( object { names string //course format option names
        ///value string //course format option value } )} )</summary></param>
        /// <returns></returns>
        public Task<ApiResponse<NewCourse>> CreateCourse(string fullname, string shortname, int category_id,
            string idnumber = "", string summary = "", int summaryformat = 1, string format = "", int showgrades = 0, int newsitems = 0,
            DateTime startdate = default, int numsections = int.MaxValue, int maxbytes = 104857600, int showreports = 1,
            int visible = 0, int hiddensections = int.MaxValue, int groupmode = 0,
            int groupmodeforce = 0, int defaultgroupingid = 0, int enablecompletion = int.MaxValue,
            int completenotify = 0, string lang = "", string forcetheme = "",
            string courcCourseformatoption = ""/*not implemented*/)
        {
            var query = GetBaseQuery(MoodleMethod.core_course_create_courses);
            query.AppendFilterQuery("&courses[0][fullname]=", fullname)
                    .AppendFilterQuery("&courses[0][shortname]=", shortname)
                    .AppendFilterQuery("&courses[0][categoryid]=", category_id)
                    .AppendFilterQueryIfHasValue("&courses[0][idnumber]=", idnumber)
                    .AppendFilterQueryIfHasValue("&courses[0][summary]=", summary)
                    .AppendFilterQueryIf(summaryformat != 1, "&courses[0][summaryformat]=", summaryformat)
                    .AppendFilterQueryIfHasValue("&courses[0][format]=", format)
                    .AppendFilterQueryIf(showgrades != 0, "&courses[0][showgrades]=", showgrades)
                    .AppendFilterQueryIf(startdate.Equals(default(DateTime)) is false, "&courses[0][startdate]=", DateTimeToUnixTimestamp(startdate))
                    .AppendFilterQueryIf(newsitems != 0, "&courses[0][newsitems]=", newsitems)
                    .AppendFilterQueryIf(numsections != int.MaxValue, "&courses[0][numsections]=", numsections)
                    .AppendFilterQueryIf(maxbytes != 104857600, "&courses[0][maxbytes]=", maxbytes)
                    .AppendFilterQueryIf(showreports != 1, "&courses[0][showreports]=", showreports)
                    .AppendFilterQueryIf(visible != 0, "&courses[0][visible]=", visible)
                    .AppendFilterQueryIf(hiddensections != int.MaxValue, "&courses[0][hiddensections]=", hiddensections)
                    .AppendFilterQueryIf(groupmode != 0, "&courses[0][groupmode]=", groupmode)
                    .AppendFilterQueryIf(groupmodeforce != 0, "&courses[0][groupmodeforce]=", groupmodeforce)
                    .AppendFilterQueryIf(defaultgroupingid != 0, "&courses[0][defaultgroupingid]=", defaultgroupingid)
                    .AppendFilterQueryIf(enablecompletion != int.MaxValue, "&courses[0][enablecompletion]=", enablecompletion)
                    .AppendFilterQueryIf(completenotify != 0, "&courses[0][completenotify]=", completenotify)
                    .AppendFilterQueryIfHasValue("&courses[0][lang]=", lang)
                    .AppendFilterQueryIfHasValue("&courses[0][forcetheme]=", forcetheme);

            return Get<NewCourse>(query);
        }
        /// <summary>
        /// Create new courses
        /// </summary>
        /// <param names="fullname"></param>
        /// <param names="shortname"></param>
        /// <param names="categoryIds"></param>
        /// <returns></returns>
        public Task<ApiResponse<NewCourse>> CreateCourses(string[] fullname, string[] shortname, int[] categoryIds)
        {
            var query = GetBaseQuery(MoodleMethod.core_course_create_courses);
            for (int i = 0; i < fullname.Count(); i++)
            {
                query.Append("&courses[").Append(i).Append("][fullname]=").Append(fullname[i])
                    .Append("&courses[").Append(i).Append("][shortname]=").Append(shortname[i])
                    .Append("&courses[").Append(i).Append("][categoryid]=").Append(categoryIds[i]);
            }
            return Get<NewCourse>(query);
        }

        public Task<ApiResponse<UpdateCourseRoot>> UpdateCourse(int id, string fullname = "", string shortname = "", int category_id = int.MaxValue,
            string idnumber = "", string summary = "", int summaryformat = 1, string format = "", int showgrades = 0, int newsitems = 0,
            DateTime startdate = default, int numsections = int.MaxValue, int maxbytes = 104857600, int showreports = 1,
            int visible = 0, int hiddensections = int.MaxValue, int groupmode = 0,
            int groupmodeforce = 0, int defaultgroupingid = 0, int enablecompletion = int.MaxValue,
            int completenotify = 0, string lang = "", string forcetheme = "",
            string courcCourseformatoption = ""/*not implemented*/)
        {
            var query = GetBaseQuery(MoodleMethod.core_course_update_courses);
            query.AppendFilterQuery("&courses[0][id]=", id)
                    .AppendFilterQueryIfHasValue("&courses[0][fullname]=", fullname)
                    .AppendFilterQueryIfHasValue("&courses[0][shortname]=", shortname)
                    .AppendFilterQueryIf(category_id != int.MaxValue, "&courses[0][categoryid]=", category_id)
                    .AppendFilterQueryIfHasValue("&courses[0][idnumber]=", idnumber)
                    .AppendFilterQueryIfHasValue("&courses[0][summary]=", summary)
                    .AppendFilterQueryIf(summaryformat != 1, "&courses[0][summaryformat]=", summaryformat)
                    .AppendFilterQueryIfHasValue("&courses[0][format]=", format)
                    .AppendFilterQueryIf(showgrades != 0, "&courses[0][showgrades]=", showgrades)
                    .AppendFilterQueryIf(startdate.Equals(default(DateTime)) is false, "&courses[0][startdate]=", DateTimeToUnixTimestamp(startdate))
                    .AppendFilterQueryIf(newsitems != 0, "&courses[0][newsitems]=", newsitems)
                    .AppendFilterQueryIf(numsections != int.MaxValue, "&courses[0][numsections]=", numsections)
                    .AppendFilterQueryIf(maxbytes != 104857600, "&courses[0][maxbytes]=", maxbytes)
                    .AppendFilterQueryIf(showreports != 1, "&courses[0][showreports]=", showreports)
                    .AppendFilterQueryIf(visible != 0, "&courses[0][visible]=", visible)
                    .AppendFilterQueryIf(hiddensections != int.MaxValue, "&courses[0][hiddensections]=", hiddensections)
                    .AppendFilterQueryIf(groupmode != 0, "&courses[0][groupmode]=", groupmode)
                    .AppendFilterQueryIf(groupmodeforce != 0, "&courses[0][groupmodeforce]=", groupmodeforce)
                    .AppendFilterQueryIf(defaultgroupingid != 0, "&courses[0][defaultgroupingid]=", defaultgroupingid)
                    .AppendFilterQueryIf(enablecompletion != int.MaxValue, "&courses[0][enablecompletion]=", enablecompletion)
                    .AppendFilterQueryIf(completenotify != 0, "&courses[0][completenotify]=", completenotify)
                    .AppendFilterQueryIfHasValue("&courses[0][lang]=", lang)
                    .AppendFilterQueryIfHasValue("&courses[0][forcetheme]=", forcetheme);

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
        public Task<ApiResponse<Category>> GetGrades(int courseId, string component = "", int activityid = int.MaxValue, string[]? userIds = null)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupids"></param>
        /// <param name="courseids"></param>
        /// <param name="eventids"></param>
        /// <returns></returns>
        public Task<ApiResponse<Events>> GetCalanderEvents(int[]? groupids = null, int[]? courseids = null, int[]? eventids = null)
        {
            var query = GetBaseQuery(MoodleMethod.core_calendar_get_calendar_events);

            if (groupids is not null)
                for (int i = 0; i < groupids.Length; i++)
                    query.Append("&events[groupids][").Append(i).Append("]=").Append(groupids[i]);

            if (courseids is not null)
                for (int i = 0; i < courseids.Length; i++)
                    query.Append("&events[courseids][").Append(i).Append("]=").Append(courseids[i]);

            if (eventids is not null)
                for (int i = 0; i < eventids.Length; i++)
                    query.Append("&events[eventids][").Append(i).Append("]=").Append(eventids[i]);

            return Get<Events>(query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <param name="descriptions"></param>
        /// <param name="formats"></param>
        /// <param name="groupids"></param>
        /// <param name="courseids"></param>
        /// <param name="repeats"></param>
        /// <param name="eventtypes"></param>
        /// <param name="timestarts"></param>
        /// <param name="timedurations"></param>
        /// <param name="visible"></param>
        /// <param name="sequences"></param>
        /// <returns></returns>
        public Task<ApiResponse<Events>> CreateCalanderEvents(string[] names, string[]? descriptions = null,
             int[]? formats = null, int[]? groupids = null, int[]? courseids = null, int[]? repeats = null,
             string[]? eventtypes = null, DateTime[]? timestarts = null, TimeSpan[]? timedurations = null,
             int[]? visible = null, int[]? sequences = null)
        {
            var query = GetBaseQuery(MoodleMethod.core_calendar_create_calendar_events);

            for (int i = 0; i < names.Length; i++)
                query.Append("&events[").Append(i).Append("][name]=").Append(names[i]);

            if (groupids is not null)
                for (int i = 0; i < groupids.Length; i++)
                    query.Append("&events[").Append(i).Append("][groupid]=").Append(groupids[i]);

            if (courseids is not null)
                for (int i = 0; i < courseids.Length; i++)
                    query.Append("&events[").Append(i).Append("][courseid]=").Append(courseids[i]);

            if (descriptions is not null)
                for (int i = 0; i < descriptions.Length; i++)
                    query.Append("&events[").Append(i).Append("][description]=").Append(descriptions[i]);

            if (formats is not null)
                for (int i = 0; i < formats.Length; i++)
                    query.Append("&events[").Append(i).Append("][format]=").Append(formats[i]);

            if (repeats is not null)
                for (int i = 0; i < repeats.Length; i++)
                    query.Append("&events[").Append(i).Append("][repeats]=").Append(repeats[i]);

            if (eventtypes is not null)
                for (int i = 0; i < eventtypes.Length; i++)
                    query.Append("&events[").Append(i).Append("][eventtypes]=").Append(eventtypes[i]);

            if (timestarts is not null)
                for (int i = 0; i < timestarts.Length; i++)
                    query.Append("&events[").Append(i).Append("][timestart]=").Append(DateTimeToUnixTimestamp(timestarts[i]));

            if (timedurations != null)
                for (int i = 0; i < timedurations.Length; i++)
                    query.Append("&events[").Append(i).Append("][timeduration]=").Append(timedurations[i].TotalSeconds);

            if (visible is not null)
                for (int i = 0; i < visible.Length; i++)
                    query.Append("&events[").Append(i).Append("][visible]=").Append(visible[i]);

            if (sequences is not null)
                for (int i = 0; i < sequences.Length; i++)
                    query.Append("&events[").Append(i).Append("][sequence]=").Append(sequences[i]);

            return Get<Events>(query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventids"></param>
        /// <param name="repeats"></param>
        /// <param name="descriptions"></param>
        /// <returns></returns>
        public Task<ApiResponse<Events>> DeleteCalanderEvents(int[]? eventids, int[]? repeats, string[]? descriptions = null)
        {
            var query = GetBaseQuery(MoodleMethod.core_calendar_delete_calendar_events);

            if (repeats is not null)
                for (int i = 0; i < repeats.Length; i++)
                    query.Append("&events[").Append(i).Append("][repeat]=").Append(repeats[i]);

            if (eventids is not null)
                for (int i = 0; i < eventids.Length; i++)
                    query.Append("&events[").Append(i).Append("][eventid]=").Append(eventids[i]);


            if (descriptions is not null)
                for (int i = 0; i < descriptions.Length; i++)
                    query.Append("&events[").Append(i).Append("][description]=").Append(descriptions[i]);

            return Get<Events>(query);
        }

        #endregion

        #region Group Actions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <param name="courseids"></param>
        /// <param name="descriptions"></param>
        /// <param name="descriptionformats"></param>
        /// <param name="enrolmentkeys"></param>
        /// <param name="idnumbers"></param>
        /// <returns></returns>
        public Task<ApiResponse<Group>> CreateGroups(string[]? names = null, int[]? courseids = null, string[]? descriptions = null,
            int[]? descriptionformats = null, string[]? enrolmentkeys = null, string[]? idnumbers = null)
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

            if (descriptionformats is not null)
                for (int i = 0; i < descriptionformats.Length; i++)
                    query.Append("&groups[").Append(i).Append("][descriptionformat]=").Append(descriptionformats[i]);

            if (enrolmentkeys is not null)
                for (int i = 0; i < enrolmentkeys.Length; i++)
                    query.Append("&groups[").Append(i).Append("][enrolmentkey]=").Append(enrolmentkeys[i]);

            if (idnumbers is not null)
                for (int i = 0; i < idnumbers.Length; i++)
                    query.Append("&groups[").Append(i).Append("][idnumber]=").Append(idnumbers[i]);


            return Get<Group>(query);
        }

        #endregion

        #region Getters

        private async Task<AuthentiactionResponse<T>> GetAuth<T>(string query) where T : IDataModel
        {
            if (HostIsSet is false)
                throw new Exception("Host is not set");

            try
            {
                string uri = Host!.AbsoluteUri + query;
                if (Host.Scheme == "https")
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var request = WebRequest.Create(Uri.EscapeUriString(uri));
                using (var response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var data = JObject.Parse(await reader.ReadToEndAsync());
                        return new AuthentiactionResponse<T>(new AuthentiactionResponseRaw(data));
                    }
                }
            }
            catch (WebException)
            {
                // No internet connection
                throw new WebException("No internet connection.");
            }
        }


        private async Task<ApiResponse<T>> Get<T>(string query) where T : IDataModel
        {
            if (HostIsSet is false)
                throw new Exception("Host is not set");

            try
            {
                string uri = Host!.AbsoluteUri + query;
                if (Host.Scheme == "https")
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var request = WebRequest.Create(Uri.EscapeUriString(uri));
                using (var response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = await reader.ReadToEndAsync();
                        if (result.ToLower() == "null")
                            result = "{IsSuccessful: true,}";
                        try
                        {
                            var data = JArray.Parse(result);
                            return new ApiResponse<T>(new ApiResponseRaw(data));
                        }
                        catch (Exception ex)
                        {
                            var data = JObject.Parse(result);
                            return new ApiResponse<T>(new ApiResponseRaw(data));
                        }
                    }
                }
            }
            catch (WebException)
            {
                // No internet connection
                throw new WebException("No internet connection.");
            }
        }

        private async Task<ApiResponse<T>> Get<T>(StringBuilder query) where T : IDataModel
        {
            return await Get<T>(query.ToString());
        }
        #endregion

        #endregion
    }
}