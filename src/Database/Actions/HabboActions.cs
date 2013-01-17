using System;

using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Bluedot.HabboServer.Database.Actions
{
    public static class HabboActions
    {
        #region Action: DoesHabboIdExist
        /// <summary>
        ///   
        /// </summary>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool DoesHabboIdExist(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("SELECT 1 FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters) != null;
            }
        }
        #endregion

        #region Action: GetHabboIdFromSSOTicket
        /// <summary>
        ///   Retrieves the ID of the Habbo matching the specified SSO Ticket.
        ///   If no match is made, -1 is returned.
        /// </summary>
        /// <param name="ssoTicket">The SSO Ticket to match.</param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns>The ID of the Habbo, or -1 if no Habbo has the specified SSO Ticket.</returns>
        public static int GetHabboIdFromSSOTicket(string ssoTicket, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@sso_ticket", ssoTicket);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `habbo_id` FROM `habbos` WHERE `sso_ticket` = @sso_ticket").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (int)returnValue;
            throw new NoResultsException();
        }
        #endregion

        #region Action: GetLoginIdFromHabboId
        /// <summary>
        ///   Retrieves the ID of the Login matching the specified Habbo ID.
        ///   If no match is made, -1 is returned.
        /// </summary>
        /// <param name="habboId">The Habbo ID to match.</param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns>The ID of the Login, or -1 if no Habbo has the specified Habbo ID.</returns>
        public static int GetLoginIdFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `login_id` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (int)returnValue;
            throw new NoResultsException();
        }
        #endregion

        #region Action: GetHabboIdFromHabboUsername
        /// <summary>
        ///   Retrieves the ID of the Habbo matching the specified username.
        /// </summary>
        /// <param name="username">The Habbo username to match.</param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns>The ID of the Habbo.</returns>
        public static int GetHabboIdFromHabboUsername(string username, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@username", username);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `habbo_id` FROM `habbos` WHERE `username` = @username").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (int)returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: GetHabboUsernameFromHabboId
        /// <summary>
        ///   Retrieves the username of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name="habboId">The Habbo ID to match.</param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns>The username of the Habbo.</returns>
        public static string GetHabboUsernameFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            string returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `username` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters) as string;
            }

            if (returnValue != null)
                return returnValue;
            throw new NoResultsException();
        }
        #endregion

        #region Action: GetCreationDateFromHabboId
        /// <summary>
        ///   Retrieves the creation date of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static DateTime GetCreationDateFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            
            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database.
                returnValue = connection.GetCachedCommand("SELECT `creation_date` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters);
            }
            
            if (returnValue != null)
                return (DateTime)returnValue;
            throw new NoResultsException();
        }
        #endregion

        #region Action: GetLastAccessDateFromHabboId
        /// <summary>
        /// Retrieves the last access date of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static DateTime GetLastAccessDateFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database.
                returnValue = connection.GetCachedCommand("SELECT `last_access` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (DateTime)returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetLastAccessDateFromHabboId
        /// <summary>
        /// Updates the last access date of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetLastAccessDateFromHabboId(int habboId, DateTime lastAccessDate, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@last_access", lastAccessDate);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                return connection.GetCachedCommand("UPDATE `habbos` SET `last_access` = @last_access WHERE `habbo_id` = @habbo_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion
        
        #region Action: GetSSOTicketFromHabboId
        /// <summary>
        /// Retrieves the SSO ticket of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetSSOTicketFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            string returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                returnValue = connection.GetCachedCommand("SELECT `sso_ticket` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters) as string;
            }

            if (returnValue != null)
                return returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetSSOTicketFromHabboId
        /// <summary>
        /// Updates the SSO ticket of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetSSOTicketFromHabboId(int habboId, string ssoTicket, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@sso_ticket", ssoTicket);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("UPDATE `habbos` SET `sso_ticket` = @sso_ticket WHERE `habbo_id` = @habbo_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion

        #region Action: GetFigureFromHabboId
        /// <summary>
        /// Retrieves the figure details of the Habbo matching the specified Habbo ID.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool GetFigureFromHabboId(int habboId, out string figureString, out bool gender, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `figure`, `gender` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteReader(parameters))
                {
                    if (!reader.HasRows)
                    {
                        figureString = "";
                        gender = true;
                        return false;
                    }

                    while (reader.Read())
                    {
                        figureString = reader["figure"] as string;
                        gender = (bool)reader["gender"];
                        return true;
                    }
                }
            }

            // Should never even be reached... but it is needed...
            figureString = "";
            gender = true;
            return false;
        }
        #endregion

        #region Action: GetCreditsFromHabboId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static int GetCreditsFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database.
                returnValue = connection.GetCachedCommand("SELECT `last_access` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (int)returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetCreditsFromHabboId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetCreditsFromHabboId(int habboId, int credits, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@credits", credits);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                return connection.GetCachedCommand("UPDATE `habbos` SET `credits` = @credits WHERE `habbo_id` = @habbo_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion

        #region Action: GetMottoFromHabboId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetMottoFromHabboId(int habboId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);

            object returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database.
                returnValue = connection.GetCachedCommand("SELECT `motto` FROM `habbos` WHERE `habbo_id` = @habbo_id").ExecuteScalar(parameters);
            }

            if (returnValue != null)
                return (string)returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetMottoFromHabboId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetMottoFromHabboId(int habboId, string motto, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@habbo_id", habboId);
            parameters.Add("@motto", motto);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                return connection.GetCachedCommand("UPDATE `habbos` SET `motto` = @motto WHERE `habbo_id` = @habbo_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion
    }
}