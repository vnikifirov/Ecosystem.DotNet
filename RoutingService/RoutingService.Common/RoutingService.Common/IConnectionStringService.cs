namespace RoutingService.Common
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IConnectionStringService
    {
        [OperationContract]
        ConnectionString GetConnectionString(string username, string password, string name);        
    }
}
