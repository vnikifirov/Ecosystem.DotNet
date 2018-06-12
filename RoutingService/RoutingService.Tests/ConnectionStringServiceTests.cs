namespace RoutingService.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RoutingService.Tests.Mocks;
    using RoutingService.Common;

    [TestClass]
    public class ConnectionStringServiceTests
    {
        [TestMethod]
        public void GetConnectionStringTest()
        {
            var service = new ConnectionStringServiceMock();
            ConnectionString builder = service.GetConnectionString("testuser", "password", "DefaultConnectionString");

            Assert.IsNotNull(builder);
        }
    }
}
