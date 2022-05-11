using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundFileService.Business.Events;
using BackgroundFileService.Business.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackgroundFileService.Business.Services.Implementations
{
    /// <inheritdoc/>
    public class FilesExtractorService : IFilesExtractorService
    {
        /// <summary>
        /// Logging
        /// </summary>
        private readonly ILogger _logger;

        public FilesExtractorService(ILogger logger) => _logger = logger;

        #region Interface

        /// <inheritdoc/>
        public virtual void CopyFiles(string sourceDir, string targetDir, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                var ex = new ArgumentNullException(nameof(sourceDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(targetDir))
            {
                var ex = new ArgumentNullException(nameof(targetDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            // Check for the path specified in sourceFileName or destFileName is invalid
            if (!IsExistDirectory(sourceDir))
            {
                var ex = new DirectoryNotFoundException(nameof(sourceDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            if (!IsExistDirectory(targetDir))
            {
                var ex = new DirectoryNotFoundException(nameof(targetDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            // Create a blank list for the source files
            var files = new List<string>();

            // Try and get files from the directories
            // catch any issues
            try
            {
                // Get files from the source directory
                var newFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

                if (newFiles.Length > 0)
                    files.AddRange(newFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            // Copy files in the target directory
            InnerCopyFiles(files, targetDir, overwrite);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Copy files into the target directory
        /// </summary>
        /// <param name="files">Where files that you want to copy</param>
        /// <param name="targetDir">The direcory you want to copy files to</param>
        /// <param name="overwrite">Replace Files</param>
        private void InnerCopyFiles(IEnumerable<string> files, string targetDir, bool overwrite = true)
        {
            // Create a blank list for the copied files
            var copiedFiles = new List<string>();

            files
                .ToList()
                .ForEach(filePath =>
                {
                    // Get file name
                    var filename = Path.GetFileName(filePath);

                    // Try copy file from the Source Directory to Target Directory
                    // catch any issues
                    try
                    {
                        // Get a target path
                        var targetPath = Path.Combine(targetDir, filename);
                        // Copy a file from the Source Directory to Target Directory
                        File.Copy(filePath, targetPath, overwrite);
                        // Add to the blank copied file
                        copiedFiles.Add(targetPath);
                    }
                    catch (IOException ex)
                    {
                        // If file alrady exist
                        _logger.LogInformation(ex, ex.Message);
                        throw ex;
                    }
                    catch (Exception ex) // If something goes wrong
                    {
                        var notSupportedException = new NotSupportedException(ex.ToString());
                        _logger.LogError(notSupportedException, notSupportedException.Message);
                        throw notSupportedException;
                    }
                });

            // Fired event and sending the list with copied files to there as agrs
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
                var dirPath = Path.GetFullPath(path);
                if (string.IsNullOrWhiteSpace(dirPath))
                    return true;

                return false;
            }
            catch (PathTooLongException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        #endregion

        /// <inheritdoc/>
        public event ChangedEventHandler FilesCopied;

        protected void OnFilesCopied(FilesCopiedEventArgs e) => FilesCopied?.Invoke(this, e);

        /// <inheritdoc/>
        public async Task CopyFilesAsync(string sourceDir, string targetDir, CancellationToken cancellationToken, bool overwrite = true)
        {
            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                var ex = new ArgumentNullException(nameof(sourceDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(targetDir))
            {
                var ex = new ArgumentNullException(nameof(targetDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            // Check for the path specified in sourceFileName or destFileName is invalid
            if (!IsExistDirectory(sourceDir))
            {
                var ex = new DirectoryNotFoundException(nameof(sourceDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            if (!IsExistDirectory(targetDir))
            {
                var ex = new DirectoryNotFoundException(nameof(targetDir));
                _logger.LogError(ex, ex.Message);
                throw ex;
            }       

            // catch any issues
            try
            {
                // Get files from the source directory
                var newFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);

                if (newFiles.Length <= 0)
                    return;

                var tasks = new List<Task>();

                newFiles
                    .ToList()
                    .ForEach(filePath =>
                    {
                        // Get file name
                        var filename = Path.GetFileName(filePath);

                        // catch any issues
                        try
                        {
                            // Get a target path
                            var targetPath = Path.Combine(targetDir, filename);
                            // Copy a file from the Source Directory to Target Directory
                            using var sourceStream = File.OpenRead(sourceDir);
                            using var targetStream = File.Create(targetPath);
                            // Copy a file from the Source Directory to Target Directory
                            tasks.Add(sourceStream.CopyToAsync(targetStream, cancellationToken));
                        }
                        catch (IOException ex)
                        {
                            // If file alrady exist
                            _logger.LogInformation(ex, ex.Message);
                            throw ex;
                        }
                        catch (Exception ex) // If something goes wrong
                        {
                            var notSupportedException = new NotSupportedException(ex.ToString());
                            _logger.LogError(notSupportedException, notSupportedException.Message);
                            throw notSupportedException;
                        }
                    });

                await Task.WhenAll(tasks.ToArray());

                // Fired event and sending the list with copied files to there as agrs
                OnFilesCopied(new FilesCopiedEventArgs(newFiles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}