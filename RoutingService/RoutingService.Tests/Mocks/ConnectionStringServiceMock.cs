namespace RoutingService.Tests.Mocks
{
    using RoutingService.Common;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    class ConnectionStringServiceMock : IConnectionStringService
    {
        private Dictionary<string, ConnectionString> _connectionStrings;

        private readonly string _username = "testuser";
        private readonly string _password = "password";

        public ConnectionStringServiceMock()
        {
            _connectionStrings = new Dictionary<string, ConnectionString>
            {
                {
                    "DefaultConnectionString", new ConnectionString(new SqlConnectionStringBuilder("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"))
                }   
            };
        }

        #region IConnectionStringService Members

        public ConnectionString GetConnectionString(string username, string password, string name)
        {
            if (!username.Equals(_username) || !password.Equals(_password))
                return null;

            if (_connectionStrings.ContainsKey(name))            
                return _connectionStrings[name];

            return null;
        }
        
        #endregion
    }
}
