
using System.Collections.Generic;

namespace Bluedot.HabboServer.Database.Actions
{
    public static class PersistenceActions
    {
        #region Action: GetPersistentValue
        private static WrappedMySqlCommand _getPersistentValue;

        /// <summary>
        /// Retrieves a persistent value.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static byte[] GetPersistentValue(string typeName, long instanceId, string variableName, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@type_name", typeName);
            parameters.Add("@instance_id", instanceId);
            parameters.Add("@variable_name", variableName);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("SELECT `value` FROM `persistent_storage` WHERE `type_name` = @type_name AND `instance_id` = @instance_id AND `variable_name` LIMIT 1").ExecuteScalar(parameters) as byte[];
            }
        }
        #endregion
        #region Action: SetPersistentValue
        /// <summary>
        /// Sets a persistent value.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetPersistentValue(string typeName, long instanceId, string variableName, byte[] value, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@type_name", typeName);
            parameters.Add("@instance_id", instanceId);
            parameters.Add("@variable_name", variableName);
            parameters.Add("@value", value);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                if(value != null)
                    return connection.GetCachedCommand("INSERT INTO `persistent_storage` (`type_name`, `instance_id`, `variable_name`, `value`) VALUES (@type_name, @instance_id, @variable_name, @value) ON DUPLICATE KEY UPDATE `value` = @value").ExecuteNonQuery(parameters) > 0;
                return connection.GetCachedCommand("DELETE FROM `persistent_storage` WHERE `type_name` = @type_name AND `instance_id` = @instance_id AND `variable_name` = @variable_name").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion
    }
}