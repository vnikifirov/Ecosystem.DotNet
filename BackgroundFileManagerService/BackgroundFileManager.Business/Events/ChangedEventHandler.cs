using System;
using System.Collections.Generic;

namespace BackgroundFileService.Business.Events
{
    /// <summary>
    /// The event notified that file was copied
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Class that holds the event data, if the event provides data.</param>
    public delegate void ChangedEventHandler(object sender, FilesCopiedEventArgs e);

    /// <summary>
    /// Class that holds the event data, if the event provides data.
    /// </summary>
    public class FilesCopiedEventArgs : EventArgs
    {
        public IEnumerable<string> Files { get; set; }

        public FilesCopiedEventArgs(IEnumerable<string> files) => Files = new List<string>(files);
    }
}
