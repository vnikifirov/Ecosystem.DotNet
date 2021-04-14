namespace RoutingService
{
    using RoutingService.Common;
    using System.Collections.Generic;

    public abstract class ConnectionStringService : IConnectionStringService
    {
        protected Dictionary<string, ConnectionString> _connectionStrings;

        #region ConnectionStrings Members

        public abstract ConnectionString GetConnectionString(string username, string password, string name);

        #endregion
    }
}
