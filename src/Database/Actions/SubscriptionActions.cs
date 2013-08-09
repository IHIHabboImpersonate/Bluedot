using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace IHI.Server.Database.Actions
{
    public static class SubscriptionActions
    {
        #region Action: GetSubscriptionData
        /// <summary>
        /// SUMMARY
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool GetSubscriptionData(int habboId, string subscriptionType, out int totalBought, out int skippedLength, out int pausedStart, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@subscription_type", subscriptionType);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `total_bought`, `skipped_length`, `paused_start` FROM `subscriptions` WHERE `habbo_id` = @habbo_id AND `subscription_type` = @subscription_type LIMIT 1").ExecuteReader(parameters))
                {
                    if (!reader.HasRows)
                    {
                        totalBought = 0;
                        skippedLength = 0;
                        pausedStart = 0;
                        return false;
                    }

                    reader.Read();
                    totalBought = (int)reader["total_bought"];
                    skippedLength = (int)reader["skipped_length"];
                    pausedStart = (int)reader["paused_start"];
                    return true;
                }
            }
        }
        #endregion
        #region Action: SetSubscriptionData
        /// <summary>
        /// SUMMARY
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetSubscriptionData(int habboId, string subscriptionType, int totalBought, int skippedLength, int pausedStart, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@subscription_type", subscriptionType);
            parameters.Add("@total_bought", totalBought);
            parameters.Add("@skipped_length", skippedLength);
            parameters.Add("@paused_start", pausedStart);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("INSERT INTO `subscriptions` (`habbo_id`, `subscription_type`, `total_bought`, `skipped_length`, `paused_start`) VALUES (@habbo_id, @subscription_type, @total_bought, @skipped_length, @paused_start) ON DUPLICATE KEY UPDATE `total_bought` = @total_bought, `skipped_length` = @skipped_length, `paused_start` = @paused_start").ExecuteNonQuery(parameters) >= 1;
            }
        }
        #endregion
    }
}