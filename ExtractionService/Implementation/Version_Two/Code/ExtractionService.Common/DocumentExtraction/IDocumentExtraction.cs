namespace ExtractionService.Common
{
    using System;
    using System.Collections.Generic;

    public delegate void ChangedEventHandler(object sender, FilesCopiedEventArgs e);
    
    public interface IDocumentExtraction
    {
        void CopyFiles(string sourceDirectory, string targetDirectory, bool overwirte = true);        
        ////Task CopyFilesAsync(string sourceDir, string targetDir, bool overwrite = true);
        
        ///// <summary>
        ///// The event an notified that file was copied
        ///// </summary>
        event ChangedEventHandler FilesCopied;
    }

    /// <summary>
    ///  A class that holds the event data, if the event provides data.
    /// </summary>
    public class FilesCopiedEventArgs : EventArgs
    {
        public List<string> Files { get; set; }

        public FilesCopiedEventArgs(List<string> files)
        {
            Files = new List<string>(files);
        }
    }
}