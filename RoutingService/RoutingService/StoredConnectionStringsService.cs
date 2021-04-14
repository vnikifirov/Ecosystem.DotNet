namespace RoutingService
{
    using RoutingService.Common;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    
    /// <summary>
    /// Service named connection string
    /// </summary>
    public class StoredConnectionStringsService : ConnectionStringService
    {
        /// <summary>
        /// Initialazes a new instance of the Routing.StoredConnectionStringsService class.
        /// </summary>
        public StoredConnectionStringsService()
        {
            _connectionStrings = new Dictionary<string, ConnectionString>
            {
                {
                    "DefaultConnectionString", new ConnectionString(new SqlConnectionStringBuilder("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"))
                }
            };
        }

        #region Contacts

        private readonly string _username = "RoutingService";
        private readonly string _password = "abcd1234";

        #endregion    

        /// In prodaction code you would have your public service method that takes all the parameters 
        /// you do a litle bit of validation and than you'd have some inernall method that actually 
        /// does the work of getting what you need and returning it.        
        #region Interface

        /// <summary>
        /// Gets the specified connection string using the specified credentials and key name.
        /// </summary>
        public override ConnectionString GetConnectionString(string username, string password, string name)
        {
            if (!username.Equals(_username) || !password.Equals(_password))
                return null;

            return InternalGetConnecionString(name);
        }

        #endregion

        /// After validation we'll be getting a connection string.
        #region Implementation

        private ConnectionString InternalGetConnecionString(string name)
        {
            if (_connectionStrings.ContainsKey(name))
                return _connectionStrings[name];

            return null;
        }

        #endregion
    }
}
