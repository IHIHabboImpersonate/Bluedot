using System.Collections.Generic;
using System.Data;

using MySql.Data.MySqlClient;


namespace IHI.Server.Database
{

    public class WrappedMySqlCommand
    {
        #region Fields
        #region Field: _command
        private readonly MySqlCommand _command;
        #endregion
        #endregion

        #region Methods
        #region Method: WrappedMySqlCommand (Constructor)
        public WrappedMySqlCommand(string query, MySqlConnection connection)
        {
            _command = new MySqlCommand(query, connection);
        }
        #endregion

        #region Method: ExecuteScalar
        public object ExecuteScalar(IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            PreExecuteCommon(parameters);

            return _command.ExecuteScalar();
        }
        #endregion

        #region Method:ExecuteReader
        public MySqlDataReader ExecuteReader(IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            PreExecuteCommon(parameters);

            return _command.ExecuteReader();
        }
        #endregion

        #region Method: ExecuteNonQuery
        public int ExecuteNonQuery(IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            PreExecuteCommon(parameters);

            return _command.ExecuteNonQuery();
        }
        #endregion

        #region Method: PreExecuteCommon
        private void PreExecuteCommon(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (_command.Connection.State == ConnectionState.Closed)
                _command.Connection.Open();

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    SetParameter(parameter.Key, parameter.Value);
                }
            }
        }
        #endregion

        #region Method: SetParameter
        private void SetParameter(string name, object value)
        {
            if (_command.Parameters.Contains(name))
                _command.Parameters[name].Value = value;
            else
                _command.Parameters.AddWithValue(name, value);
        }
        #endregion
        #endregion
    }
}
