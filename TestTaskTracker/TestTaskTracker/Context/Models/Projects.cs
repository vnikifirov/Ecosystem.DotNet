using System;
using System.Collections.Generic;
using System.Text;

namespace Context.Models
{
    /// <summary>
    /// Project status
    /// </summary>
    public enum ProjectStatus
    {
        NotStarted,
        Active,
        Completed
    }

    /// <summary>
    /// Project
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Completion date
        /// </summary>
        public DateTime Completion { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Projects
        /// </summary>
        public ICollection<Task> Tasks { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public long Priority { get; set; }
    }
}
