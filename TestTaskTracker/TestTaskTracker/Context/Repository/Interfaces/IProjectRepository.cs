using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Context.Repository.Interfaces
{
    /// <summary>
    /// Projects Data Layer
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Get All Projects
        /// </summary>
        /// <returns><see cref="ICollection{T}"/></returns>
        Task<ICollection<Models.Project>> GetProjectsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get Project By Id
        /// </summary>
        /// <param name="id">Id of Project</param>
        /// <returns><see cref="Models.Project"/></returns>
        Task<Models.Project> GetProjectByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false);

        /// <summary>
        /// Insert new Project into DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task AddProjectAsync(Models.Project project, CancellationToken cancellationToken);

        /// <summary>
        /// Insert new Project into DataBase 
        /// </summary>
        /// <param name="projects">Project</param>
        Task AddProjectsAsync(IEnumerable<Models.Project> projects, CancellationToken cancellationToken);

        /// <summary>
        /// Update existing Project in DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task UpdateProjectAsync(Models.Project project, CancellationToken cancellationToken);

        /// <summary>
        /// Remove existing Project from DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task RemoveProjectAsync(Models.Project project, CancellationToken cancellationToken);
    }
}
