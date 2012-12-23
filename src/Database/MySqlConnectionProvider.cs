
using MySql.Data.MySqlClient;

namespace Bluedot.HabboServer.Database
{
    public class MySqlConnectionProvider
    {
        #region Field: _connectionString
        private readonly MySqlConnectionStringBuilder _connectionString;
        #endregion

        #region Property: Host
        /// <summary>
        /// The host of the MySQL server to connect to.
        /// </summary>
        public string Host
        {
            get
            {
                return _connectionString.Server;
            }
            set
            {
                _connectionString.Server = value;
            }
        }
        #endregion
        
        #region Property: Port
        /// <summary>
        /// The port of the MySQL server to connect to.
        /// </summary>
        public uint Port
        {
            get
            {
                return _connectionString.Port;
            }
            set
            {
                _connectionString.Port = value;
            }
        }
        #endregion

        #region Property: User
        /// <summary>
        /// The user to authenticate with when connecting to the MySQL server.
        /// </summary>
        public string User
        {
            get
            {
                return _connectionString.UserID;
            }
            set
            {
                _connectionString.UserID = value;
            }
        }
        #endregion

        #region Property: Password
        /// <summary>
        /// The password to authenticate with when connecting to the MySQL server.
        /// Notice: For security reasons, this value cannot be read using this property!
        /// </summary>
        public string Password
        {
            set
            {
                _connectionString.Password = value;
            }
        }
        #endregion

        #region Property: Database
        /// <summary>
        /// The name of the database to use on the MySQL server.
        /// </summary>
        public string Database
        {
            get
            {
                return _connectionString.Database;
            }
            set
            {
                _connectionString.Database = value;
            }
        }
        #endregion

        #region Property: ConnectionString
        /// <summary>
        /// The connection string used when making connections to the MySQL server.
        /// Notice: For security reasons, the password is excluded from the value this property returns.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString.GetConnectionString(false);
            }
        }
        #endregion

        #region Method: MySqlConnectionProvider (Constructor)
        public MySqlConnectionProvider()
        {
            _connectionString = new MySqlConnectionStringBuilder();
        }
        #endregion

        #region Method: GetConnection
        public WrappedMySqlConnection GetConnection()
        {
            return new WrappedMySqlConnection(_connectionString.GetConnectionString(true));
        }
        #endregion
    }
}
