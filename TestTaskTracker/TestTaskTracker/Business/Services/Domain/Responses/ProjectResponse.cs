using Context.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Services.Domain.Responses
{
    /// <summary>
    /// Project respone API resource
    /// </summary>
    public class ProjectResponse
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
        /// Tasks
        /// </summary>
        public ICollection<TaskResponse> Tasks { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public long Priority { get; set; }
    }
}
