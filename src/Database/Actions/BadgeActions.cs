using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace Bluedot.HabboServer.Database.Actions
{
    public static class BadgeActions
    {
        #region Action: GetBadgeTypeCodeFromBadgeTypeId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetBadgeTypeCodeFromBadgeTypeId(int badgeTypeId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@type_id", badgeTypeId);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("SELECT `code` FROM `badge_types` WHERE `type_id` = @type_id").ExecuteScalar(parameters) as string;
            }
        }
        #endregion
        #region Action: GetBadgeTypeIdFromBadgeTypeCode
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static int GetBadgeTypeIdFromBadgeTypeCode(string badgeTypeCode, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@code", badgeTypeCode);
            
            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `type_id` FROM `badge_types` WHERE `code` = @code").ExecuteScalar(parameters);
            }

            // Was a value found?
            if (returnValue != null)
            {
                // Yes, return it.
                return (int)returnValue;
            }

            // No, return -1.
            return -1;
        }
        #endregion

        #region Action: GetBadgeTypeIdFromBadgeTypeCode
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static IDictionary<int, int> GetBadgeDataFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            Dictionary<int, int> badges = new Dictionary<int, int>();

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `badge_type_id`, `badge_slot` FROM `badge_assignments` WHERE `habbo_id` = @habbo_id").ExecuteReader(parameters))
                {
                    while (reader.Read())
                    {
                        badges.Add((int)reader["badge_type_id"], (int)reader["badge_slot"]);
                    }

                    return badges;
                }
            }
        }
        #endregion
    }
}