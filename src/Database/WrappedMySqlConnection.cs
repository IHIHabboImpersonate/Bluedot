using System;

using MySql.Data.MySqlClient;

using IHI.Server.Useful;

namespace IHI.Server.Database
{
    public class WrappedMySqlConnection : IDisposable
    {
        #region Field: _queryString
        /// <summary>
        /// 
        /// </summary>
        private readonly MySqlConnection _connection;
        #endregion

        #region Field: _commandCache

        /// <summary>
        /// 
        /// </summary>
        private readonly WeakCache<string, WrappedMySqlCommand> _commandCache;
        #endregion

        public WrappedMySqlConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _commandCache = new WeakCache<string, WrappedMySqlCommand>(NewMySqlCommand);
        }

        private WrappedMySqlCommand NewMySqlCommand(string queryString)
        {
            WrappedMySqlCommand command = new WrappedMySqlCommand(queryString, _connection);
            return command;
        }

        #region Method: GetCachedStatement
        public WrappedMySqlCommand GetCachedCommand(string queryString)
        {
            return _commandCache[queryString];
        }
        #endregion

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
