namespace ExtractionService.Test
{
    using System;
    using ExtractionService.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationExtractionServiceTest
    {
        private ConfigurationExtractionService _config;

        [TestInitialize]
        public void SetupTest()
        {
            try
            {
                _config = new ConfigurationExtractionService();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public void ConfigurationsExceptAttachment_IsNotNullOrWhiteSpace()
        {
            foreach (var conf in _config.ConfigurationStrings)
            {
                if (string.IsNullOrWhiteSpace(conf.Value))
                    Assert.Fail($"Configuration with key: {conf.Key} does not exist.");
            }
        }
    }
}
