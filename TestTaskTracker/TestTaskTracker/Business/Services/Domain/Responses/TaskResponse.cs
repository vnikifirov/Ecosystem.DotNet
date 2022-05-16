using System;
using System.Collections.Generic;
using System.Text;
using Context.Models;

namespace Business.Services.Domain.Responses
{
    /// <summary>
    /// Task respone API resource
    /// </summary>
    public class TaskResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public ProjectResponse Project { get; set; }

        /// <summary>
        /// Id_Project
        /// </summary>
        public long? Id_Project { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public long Priority { get; set; }
    }
}
