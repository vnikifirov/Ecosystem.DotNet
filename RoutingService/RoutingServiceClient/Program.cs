namespace RoutingServiceClient
{
    using System;
    using RoutingService.Common;
    using System.ServiceModel;

    class Program
    {
        static void Main(string[] args)
        {
            var basicHttpBinding = new BasicHttpBinding();
            var endpont = new EndpointAddress("http://localhost:8080/RoutingService/RoutingService.StoredConnectionStringsService.svc");

            var connectionStringService = new ChannelFactory<IConnectionStringService>(basicHttpBinding, endpont);

            IConnectionStringService client = connectionStringService.CreateChannel();

            ConnectionString connectionString = client.GetConnectionString("RoutingService","abcd1234","DefaultConnectionString");

            IClientChannel chanel = (IClientChannel)client;
            chanel.Close();
            chanel.Dispose();

            Console.WriteLine(connectionString);
            Console.ReadKey();
        }
    }
}
