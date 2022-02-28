using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Services.Interfaces
{
    /// <summary>
    /// Proejcts business logic
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Get project by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cansellationToken"></param>
        /// <returns></returns>
        Task<ProjectResponse> GetByIdAsync(long id, CancellationToken cansellationToken, bool includeRelated = false);

        /// <summary>
        /// Get all projects and you can apply complex query (eg get project by its name, start date)
        /// </summary>
        /// <param name="queryObj">Query object</param>
        /// <param name="cansellationToken"></param>
        /// <returns></returns>
        Task<ICollection<ProjectResponse>> GetAllAsync(ProjectQuery queryObj, CancellationToken cansellationToken);

        /// <summary>
        /// Get all projects and you can apply simple filter (eg get project with specific id)
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="cansellationToken"></param>
        /// <returns></returns>
        //Task<ICollection<ProjectResponse>> GetAllAsync(Filter filter, CancellationToken cansellationToken);

        /// <summary>
        /// Insert new project into DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task AddAsync(CreateProjectRequest project, CancellationToken cancellationToken);

        /// <summary>
        /// Insert new project into DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task AddRangeAsync(IEnumerable<CreateProjectRequest> project, CancellationToken cancellationToken);

        /// <summary>
        /// Update existing project in DataBase 
        /// </summary>
        /// <param name="project">Project</param>
        Task UpdateAsync(SaveProjectRequest project, CancellationToken cancellationToken);
        
        /// <summary>
        /// Remove existing project from DataBase by id
        /// </summary>
        /// <param name="id">Project id</param>
        Task RemoveAsync(long id, CancellationToken cancellationToken);
    }
}
