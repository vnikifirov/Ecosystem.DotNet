using System;
using System.Collections.Generic;
using Context.Models;

namespace Business.Services.Domain.Requests
{
    /// <summary>
    /// On Create Project API resource
    /// </summary>
    public class CreateProjectRequest
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
        public ICollection<CreateTaskRequest> Tasks { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public long Priority { get; set; }
    }
}
