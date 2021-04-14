namespace ExtractionService.Common
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// Service named ResentFilesExtractionService
    /// </summary>
    public class DocumentExtraction : IDocumentExtraction
    {
        #region Interface
        /// <summary>
        /// Copy the file, if file does exist in specified directory and overwrite is true than file will be
        /// overwrited overwise this file will be skipped.
        /// </summary>
        public virtual void CopyFiles(string sourceDirectory, string targetDirectory, bool overwrite = true)
        {
            // If a zero-length string, contains only white space,
            // or contains one or more invalid characters 
            if (string.IsNullOrWhiteSpace(sourceDirectory))
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(targetDirectory))
                throw new ArgumentNullException();

            // Check for the path specified in sourceFileName 
            // or destFileName is invalid
            if (!IsExistDirectory(sourceDirectory))
                throw new DirectoryNotFoundException();

            if (!IsExistDirectory(targetDirectory))
                throw new DirectoryNotFoundException();

            // Create a blank list for the source files
            var sourceFiles = new List<string>();
            // Try and get files from the directories
            // catch any issues
            try
            {
                // Get files from the source directory
                string[] source = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                if (source.Length > 0)
                    sourceFiles.AddRange(source);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // Copy files in the target directory
            InnerCopyFiles(sourceFiles, targetDirectory, overwrite);
        }

        #endregion

        #region Implementation

        private void InnerCopyFiles(List<string> sourceFiles, string targetDirectory, bool overwrite)
        {
            // Create a blank list for the copied files
            var copiedFiles = new List<string>();

            sourceFiles.ForEach(filePath => {
                // Get a file name
                string filename = Path.GetFileName(filePath);

                // Try copy file from the Source Directory to Target Directory
                // catch any issues
                try
                {
                    // Get a target path
                    string targetPath = Path.Combine(targetDirectory, filename);
                    // Copy a file from the Source Directory to Target Directory
                    File.Copy(filePath, targetPath, overwrite);
                    // Add to the blank copied file
                    copiedFiles.Add(targetPath);
                }
                catch (IOException ex) 
                {
                    // If file alrady exist
                    System.Diagnostics.Trace.WriteLine(ex.Message, "INFO"); 
                }
                catch(Exception ex) // If something goes wrong
                {
                    throw new NotSupportedException(ex.ToString());
                }
            });
            // Fired an event and sending the list with copied files to there as agrs
            OnFilesCopied(new FilesCopiedEventArgs(copiedFiles));
        }

        #endregion

        #region Validation
        /// <summary>
        /// If directory exist throw true otherwise false
        /// </summary>
        private bool IsExistDirectory(string path)
        {
            try
            {
                Path.GetFullPath(path);

                return true;
            }
            catch (PathTooLongException ex)
            {
                throw ex;
            }
            catch (NotSupportedException ex)
            {
                throw ex;
            }
        }

        #endregion     

        // An event that ExtractionService can use to be notified whenever the
        // files was copied.
        public event ChangedEventHandler FilesCopied;
        protected void OnFilesCopied(FilesCopiedEventArgs e)
        {
            FilesCopied?.Invoke(this, e);
        }
    }
}