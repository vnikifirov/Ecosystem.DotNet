namespace BackgroundFileService.Business.Services.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using BackgroundFileService.Business.Events;

    /// <summary>
    /// Service copy files from one directory to another directory
    /// </summary>
    public interface IFilesExtractorService
    {
        /// <summary>
        /// Copy the file, if file exist in specified directory and overwrite is true then file will be
        /// overwrited overwise this file will be skipped.
        /// </summary>
        /// <param name="sourceDir">Where files that you want to copy</param>
        /// <param name="targetDir">The direcory you want to copy files to</param>
        /// <param name="overwrite">Replace Files</param>
        void CopyFiles(string sourceDir, string targetDir, bool overwrite = true);

        /// <summary>
        /// Copy the file asynchronously, if file exist in specified directory and overwrite is true then file will be
        /// overwrited overwise this file will be skipped.
        /// </summary>
        /// <param name="sourceDir">Where files that you want to copy</param>
        /// <param name="targetDir">The direcory you want to copy files to</param>
        /// <param name="overwrite">Replace Files</param>
        Task CopyFilesAsync(string sourceDir, string targetDir, CancellationToken cancellationToken, bool overwrite = true);

        ///// <summary>
        ///// The event notified that file was copied
        ///// </summary>
        event ChangedEventHandler FilesCopied;
    }
}