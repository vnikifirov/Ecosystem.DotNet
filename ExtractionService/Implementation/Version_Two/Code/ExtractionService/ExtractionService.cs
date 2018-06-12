namespace ExtractionService
{
    using System;
    using System.Timers;
    using System.Reflection;
    using System.ServiceProcess;
    using global::ExtractionService.Common;

    /// <summary>
    /// Extraction Service class
    /// </summary>
    public partial class ExtractionService : ServiceBase
    {
        #region log4net Members

        //Declare an instance for log4net
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Timer _timer;
        private DocumentExtraction _extractor;
        private ConfigurationExtractionService _config;

        #endregion

        #region Properties

        public DocumentExtraction Extractor
        {
            get
            {
                // If true than initialize of DocumentExtraction instance
                if (_extractor == null)
                {
                    _extractor = new DocumentExtraction();
                    _extractor.FilesCopied += FilesWasCopied;
                }

                return _extractor;
            }

            private set
            {
                _extractor = value;
            }
        }

        #endregion

        /// <summary>
        /// Initialize of a new instance of the ExtractionService class
        /// </summary>
        public ExtractionService()
        {
            InitializeComponent();
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            // Try re-loading app configurations
            try
            {
                _config = new ConfigurationExtractionService();
            }
            catch (System.Configuration.ConfigurationException ex)
            {
                log.Error("Incorrect configuration", ex);
            }
            // Setup timer
            SetupTimer();
        }

        // Stop the Windows service.
        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }
        
        /// <summary>
        /// Setup configurations for the timer
        /// </summary>
        private void SetupTimer()
        {
            int interval = 0;
            // Try and get Interval from the App Configuration
            // write any issues in log file
            try
            {
                interval = 60 * (1000 * int.Parse(_config["Interval"]));

                // If the interval is 0
                if (interval <= 0)
                    throw new ArgumentOutOfRangeException();
            }
            catch (Exception ex)
            {
                log.Error("The timer wasn't set successful!", ex);
            }    

            // Setup the task schedule
            _timer = new Timer(interval);
            _timer.Elapsed += new ElapsedEventHandler(CopyResentFiles);
            _timer.Start();
        }

        /// <summary>
        /// Invoke if the files have been copied
        /// </summary>
        private void FilesWasCopied(object sender, FilesCopiedEventArgs e)
        {
            // If files wasn't found
            if (e.Files.Count <= 0) return;

            // The copied files write down in the log file
            log.Info(LogMessageHelper.GetMessage(e.Files.ToArray()));

            // Send a message to recipient(s).
            SendMessage();
        }

        /// <summary>
        /// The copy only recently added files
        /// </summary>
        private void CopyResentFiles(object sender, ElapsedEventArgs e)
        {
            // Try to copy the files and write down any problems
            // that occurred in the log file
            try
            {
                Extractor.CopyFiles(_config["SourceDirectory"],
                                    _config["TargetDirectory"],
                                    false);
            }
            catch (Exception ex)
            {
                log.Error("The files weren't copied successful!", ex);
            }
        }

        /// <summary>
        /// Send an email to recipients
        /// </summary>
        private void SendMessage()
        {
            // Try send an email to recipients and write down any issues in log file
            try
            {
                string[] recipients = _config["Recipients"].Split(',');
                string[] smptConfigs = _config["Smpt"].Split(',');
                string attachment = _config["Attachment"];

                if (recipients.Length <= 0)
                    throw new ArgumentNullException();

                if (smptConfigs.Length <= 0)
                    throw new ArgumentNullException();

                // Send a message to recipient(s).
                EmailMessager.CreateEmailFrom(smptConfigs[0])
                             .To(recipients)
                             .WithSubject(_config["Subject"])
                             .WithBody(_config["Message"])
                             .SendWithAttachment(smptConfigs[1], smptConfigs[2], attachment);
            }
            catch (Exception ex)
            {
                log.Error("Message wasn't sended successful!", ex);
            }
        }        
    }
}
