using Business.Services.Interfaces;
using Context.Models;

namespace Business.Services.Domain.Requests
{
    /// <summary>
    /// On Save Task API resource
    /// </summary>
    public class SaveTaskRequest : IXPass
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
        public SaveProjectRequest Project { get; set; }

        /// <summary>
        /// Id_Project
        /// </summary>
        public long? Id_Project { get; set; }

        /// <summary>
        /// Priority
        /// </summary>
        public long Priority { get; set; }

        /// <inheritdoc/>
        public string XPass { get; set; }
    }
}
