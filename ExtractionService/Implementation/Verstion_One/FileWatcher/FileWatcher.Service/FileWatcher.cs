using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Timers;
using System.Net.Mail;
using System.Reflection;
using System.Configuration;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Net.Configuration;

using log4net;
using log4net.Appender;

namespace FileWatcher
{
    public partial class FileWatcher : ServiceBase
    {
        #region Fields

        // Interval in the minutes
        private static Timer _timer;

        // Path to files
        private double _interval;

        #endregion

        #region log4net Members

        //Declare an instance for log4net
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

        #endregion

        /// <summary>
        /// Initilaze a new instance of Service class
        /// </summary>
        public FileWatcher()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start the File tracking service
        /// </summary>
        protected override void OnStart(string[] args)
        {
            // Sets of a new delay
            SetUpTimer();
        }

        /// <summary>
        /// Stop the File tracking service
        /// </summary>
        protected override void OnStop()
        {
            _timer.Dispose();
        }

        /// <summary>
        /// Scanning for a recent | newest files
        /// </summary>
        /// <param name="fileName">The file path.</param>
        /// <returns></returns>
        public static string[] GetRecentFiles(DirectoryInfo srcDir, DirectoryInfo destDir)
        {
            try
            {
                if (srcDir == null || !srcDir.Exists)
                    throw new DirectoryNotFoundException();
                if (destDir == null || !destDir.Exists)
                    throw new DirectoryNotFoundException();

                FileInfo[] files = srcDir.GetFiles();
                FileInfo[] oldFiles = destDir.GetFiles();
                var fileComparer = new FilesEqualsComparer();

                // Scanning for a new files
                var recentFiles = (from file in files
                                   select file).Except(oldFiles, fileComparer)
                                               .Select( f => f.FullName ).ToList();             
                // Retrive a new files, if they was found
                return recentFiles.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To copy a file to another location and overwrite 
        /// the destination file if it already exists.
        /// </summary>
        /// <param name="files">The files to copy.</param>
        /// <param name="targetPath">The path of the destination.</param>
        public static bool CopyFiles(string[] files, string targetPath)
        {
            try
            {
                if (!Directory.Exists(targetPath))
                     Directory.CreateDirectory(targetPath);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(targetPath, fileName);

                    File.Copy(file, destFile, true);
                }

                return true;
            }
            catch (Exception)
            {
                throw new AccessViolationException();
            }
        }

        /// <summary>
        /// Sets of a timer to raise an event on a specified interval.
        /// </summary>
        /// <param name="timeToGo">Sets an interval in minutes.</param>
        private void SetUpTimer()
        {
            try
            {
                string inputTimer = ConfigurationManager.AppSettings["SetInterval"];

                if (!double.TryParse(inputTimer, out _interval))
                    throw new ArgumentException();

                _timer = new Timer();
                _timer.Interval = (_interval * 1000) * 60;
                _timer.Elapsed += new ElapsedEventHandler(Tasks);
                _timer.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deliver message delivery
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        private static bool SendEmail(string message)
        {
            try
            {
                string[] recipients = ConfigurationManager.AppSettings["MailTo"].Split(',');
                var smptConfigs = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                MailSystem.CreateEmailFrom(smptConfigs.From)
                    .To(recipients)
                    .WithSubject("Notification")
                    .WithBody(message)
                    .Send(smptConfigs.Network.UserName,
                          smptConfigs.Network.Password);

                return true;
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrive a message about the number of new files, their names and sizes.
        /// </summary>      
        private static string ParseToMessage(string[] files)
        {
            var msg = new StringBuilder();
            var fi = (from file in files
                      select new FileInfo(file))
                      .ToList();

            foreach (var file in fi)
            {
                msg.Append(file.Name.PadRight(32));
                msg.Append($"bytes: {file.Length}");
                msg.AppendLine();
            }

            msg.AppendLine($"Amount Files: {fi.Count}");
            
            msg.AppendLine();
            msg.AppendLine();

            return msg.ToString();
        }

        /// <summary>
        ///  Main point to executing the features of program
        /// </summary>
        private static void Tasks(object sender, ElapsedEventArgs e)
        {
            string pathToFiles = ConfigurationManager.AppSettings["PathSource"];
            string destination = ConfigurationManager.AppSettings["PathDestination"];
            
            if (string.IsNullOrEmpty(pathToFiles))
                throw new ArgumentNullException();

            // Get a new recent files
            string[] recentFiles = GetRecentFiles(new DirectoryInfo(pathToFiles),
                                                  new DirectoryInfo(destination));

            // If files were not found
            if (recentFiles.Length <= 0) return;

            // If the files were not copied due to an error
            if (!CopyFiles(recentFiles, destination))
                log.Error("Files were not copied successfully!");
            else
                log.Info(ParseToMessage(recentFiles));

            // Send a message
            SendEmail(GetFile(GetLogFileName("RollingFileAppender")));
        }

        /// <summary>
        /// Retrive a file name by appender name
        /// </summary>
        public static string GetLogFileName(string name)
        {
            var rootAppender = LogManager.GetRepository()
                                         .GetAppenders()
                                         .OfType<FileAppender>()
                                         .FirstOrDefault(fa => fa.Name == name);

            return rootAppender != null ? rootAppender.File : string.Empty;
        }

        /// <summary>
        /// Reading a file by name
        /// </summary>
        public static string GetFile(string fileName)
        {
            string file = string.Empty;

            try
            {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    file = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return file;
        }

    }

    /// <summary>
    /// This is Helper class to сompare a file to the identity
    /// </summary>
    public class FilesEqualsComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            if (first.FullName == second.FullName)
                return true;

            return true;
        }

        public int GetHashCode(FileInfo obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.FullName)) { return 0; }
            return obj.Name.GetHashCode();
        }
    }

}
