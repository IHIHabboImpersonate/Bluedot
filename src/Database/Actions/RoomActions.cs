using System;

using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace IHI.Server.Database.Actions
{
    public static class RoomActions
    {
        #region Action: DoesRoomIdExist
        /// <summary>
        ///   
        /// </summary>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool DoesRoomIdExist(int roomId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                return connection.GetCachedCommand("SELECT 1 FROM `rooms` WHERE `room_id` = @room_id").ExecuteScalar(parameters) != null;
            }
        }
        #endregion

        #region Action: GetRoomIdsFromRoomOwner
        /// <summary>
        ///   Retrieves the IDs of the Rooms owned by owner ID.
        /// </summary>
        /// <param name="ownerId">The owner ID to search with.</param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        public static IEnumerable<int> GetRoomIdsFromRoomOwner(string ownerId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@owner_id", ownerId);

            List<int> roomIds = new List<int>();

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                using (MySqlDataReader reader = connection.GetCachedCommand("SELECT `room_id` FROM `rooms` WHERE `owner_id` = @owner_id").ExecuteReader(parameters))
                {
                    while (reader.Read())
                    {
                        roomIds.Add((int)reader["room_id"]);
                    }

                    return roomIds;
                }
            }
        }
        #endregion

        #region Action: GetRoomModelFromRoomId
        /// <summary>
        ///   
        /// </summary>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetRoomModelFromRoomId(int roomId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);

            string returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `model` FROM `rooms` WHERE `room_id` = @room_id").ExecuteScalar(parameters) as string;
            }

            if (returnValue != null)
                return returnValue;
            throw new NoResultsException();
        }
        #endregion

        #region Action: GetRoomNameFromRoomId
        /// <summary>
        ///   
        /// </summary>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetRoomNameFromRoomId(int roomId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);

            string returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `name` FROM `rooms` WHERE `room_id` = @room_id").ExecuteScalar(parameters) as string;
            }

            if (returnValue != null)
                return returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetRoomNameFromRoomId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetRoomNameFromRoomId(int roomId, string name, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);
            parameters.Add("@name", name);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                return connection.GetCachedCommand("UPDATE `rooms` SET `name` = @name WHERE `room_id` = @room_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion

        #region Action: GetRoomDescriptionFromRoomId
        /// <summary>
        ///   
        /// </summary>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static string GetRoomDescriptionFromRoomId(int roomId, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);

            string returnValue;
            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                returnValue = connection.GetCachedCommand("SELECT `description` FROM `rooms` WHERE `room_id` = @room_id").ExecuteScalar(parameters) as string;
            }

            if (returnValue != null)
                return returnValue;
            throw new NoResultsException();
        }
        #endregion
        #region Action: SetRoomDescriptionFromRoomId
        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="connection">The connection to use. If not specified (or null) then a connection will be picked automatically.</param>
        /// <returns></returns>
        public static bool SetRoomDescriptionFromRoomId(int roomId, string description, WrappedMySqlConnection connection = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@room_id", roomId);
            parameters.Add("@description", description);

            using (connection = connection ?? CoreManager.ServerCore.MySqlConnectionProvider.GetConnection())
            {
                // Get the value from the database and return it.
                return connection.GetCachedCommand("UPDATE `rooms` SET `description` = @description WHERE `room_id` = @room_id").ExecuteNonQuery(parameters) > 0;
            }
        }
        #endregion

        //TODO: Finish off these actions
    }
}