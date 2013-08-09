using System.Collections.Generic;
using MySql.Data.MySqlClient;
using IHI.Server.Permissions;

namespace IHI.Server.Database.Actions
{
    public static class PermissionActions
    {
        #region Table: permission_group_permissions

        #region Action: GetAllPermissionGroupPermissions

        /// <summary>
        ///	
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static ICollection<PermissionsGroupPermissionData> GetAllPermissionGroupPermissions(WrappedMySqlConnection connection = null)
        {
            List<PermissionsGroupPermissionData> groupPermissions = new List<PermissionsGroupPermissionData>();

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `group_name`, `permission_name`, `permission_state` FROM `permission_group_permissions`").ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groupPermissions.Add(new PermissionsGroupPermissionData((string)reader["group_name"], (string)reader["permission_name"], (PermissionState)reader["permission_state"])); // TODO: Test if a direct cast to PermissionState is possible.
                    }

                    return groupPermissions;
                }
            }
        }

        #endregion

        #endregion

        #region Table: permission_habbo_permissions

        #region Action: GetHabboPermissionsFromHabboId

        /// <summary>
        ///	
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static Dictionary<string, PermissionState> GetHabboPermissionsFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, PermissionState> permissions = new Dictionary<string, PermissionState>();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `permission_name`, `permission_state` FROM `permission_habbo_permissions` WHERE `habbo_id` = @habbo_id").ExecuteReader(parameters))
                {
                    while (reader.Read())
                    {
                        if((sbyte)reader["permission_state"] == 1)
                            permissions.Add((string)reader["permission_name"], PermissionState.Allow);
                        else
                            permissions.Add((string)reader["permission_name"], PermissionState.Deny);
                    }

                    return permissions;
                }
            }
            ;
        }

        #endregion

        #region Action: SetHabboPermissionsFromId

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetHabboPermissionsFromId(int habboId, string permissionName, PermissionState permissionState, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@permission_name", permissionName);

            if (permissionState == PermissionState.Undefined)
            {
                using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
                {
                    return connection.GetCachedCommand("DELETE FROM `permission_habbo_permissions` WHERE `habbo_id` = @habbo_id AND `permission_name`= @permission_name").ExecuteNonQuery(parameters) > 0;
                }
            }

            parameters.Add("@permission_state", (int)permissionState);
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("INSERT INTO `permission_habbo_permissions` SET `habbo_id` = @habbo_id, `permission_name` = @permission_name, `permission_state` = @permission_state ON DUPLICATE KEY UPDATE `permission_state` = @permission_state").ExecuteNonQuery(parameters) > 0;
            }
        }

        #endregion

        #endregion

        #region Table: permission_habbo_groups

        #region Action: GetHabboPermissionGroupsFromHabboId

        /// <summary>
        ///	
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static IEnumerable<string> GetHabboPermissionGroupsFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            List<string> groupNames = new List<string>();

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `group_name` FROM `permission_habbo_groups` WHERE `habbo_id` = @habbo_id").ExecuteReader(parameters))
                {
                    while (reader.Read())
                    {
                        groupNames.Add((string)reader["group_name"]);
                    }

                    return groupNames;
                }
            }
        }

        #endregion

        #region Action: AddHabboPermissionGroups

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool AddHabboPermissionGroups(int habboId, string groupName, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@group_name", groupName);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("INSERT INTO `permission_habbo_groups` SET `habbo_id` = @habbo_id, `group_name` = @group_name").ExecuteNonQuery(parameters) > 0;
            }
        }

        #endregion

        #region Action: RemoveHabboPermissionGroups

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool RemoveHabboPermissionGroups(int habboId, string groupName, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@group_name", groupName);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("DELETE FROM `permission_habbo_groups` WHERE `habbo_id` = @habbo_id AND `group_name` = @group_name").ExecuteNonQuery(parameters) > 0;
            }
        }

        #endregion

        #endregion
    }

    public class PermissionsGroupPermissionData
    {
        #region Property: GroupName
        /// <summary>
        /// 
        /// </summary>
        public string GroupName
        {
            get;
            private set;
        }
        #endregion

        #region Property: PermissionName
        /// <summary>
        /// 
        /// </summary>
        public string PermissionName
        {
            get;
            private set;
        }
        #endregion

        #region Property: PermissionState
        /// <summary>
        /// 
        /// </summary>
        public PermissionState PermissionState
        {
            get;
            private set;
        }
        #endregion

        #region Method: PermissionsGroupPermissionData (Constructor)
        public PermissionsGroupPermissionData(string groupName, string permissionName, PermissionState permissionState)
        {
            GroupName = groupName;
            PermissionName = permissionName;
            PermissionState = permissionState;
        }
        #endregion
    }
}