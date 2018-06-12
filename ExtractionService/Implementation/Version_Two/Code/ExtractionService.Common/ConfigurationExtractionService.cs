namespace ExtractionService.Common
{
    using System.Configuration;
    using System.Collections.Generic;

    /// <summary>
    /// The class provides the configurations for an app.
    /// </summary>
    public class ConfigurationExtractionService
    {
        private Dictionary<string, string> configurationStrings;

        /// <summary>
        /// Gets the configurations by configuration name
        /// </summary>
        public string this[string configName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(configName))
                    return null;

                return configurationStrings[configName];
            }

            private set
            {
                configurationStrings[configName] = value;
            }
        }

        /// <summary>
        /// Gets the configurations
        /// </summary>
        public Dictionary<string, string> ConfigurationStrings
        {
            get
            {
                return configurationStrings;
            }
            private set
            {
                configurationStrings = value;
            }
        }

        public ConfigurationExtractionService()
        {
            Initialization();
        }

        private void Initialization()
        {
            var smpt = (System.Net.Configuration.SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            ConfigurationStrings = new Dictionary<string, string>
            {
                { "SourceDirectory",    ConfigurationManager.AppSettings["SourceDirectory"]},
                { "TargetDirectory",    ConfigurationManager.AppSettings["TargetDirectory"]},
                { "Recipients",         ConfigurationManager.AppSettings["Recipients"]},
                { "Interval",           ConfigurationManager.AppSettings["Interval"]},
                { "Subject",            ConfigurationManager.AppSettings["Subject"]},
                { "Message",            ConfigurationManager.AppSettings["Message"]},
                { "Attachment",        ConfigurationManager.AppSettings["Attachment"]},
                { "Smpt", string.Format($"{smpt.From},{smpt.Network.UserName},{smpt.Network.Password}")}
            };
        }
    }
}
