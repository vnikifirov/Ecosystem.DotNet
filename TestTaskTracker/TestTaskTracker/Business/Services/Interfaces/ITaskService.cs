using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;

namespace Business.Services.Interfaces
{
    /// <summary>
    /// Tasks business logic
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Get task by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cansellationToken"></param>
        /// <param name="includeRelated"></param>
        /// <returns></returns>
        Task<TaskResponse> GetByIdAsync(long id, CancellationToken cansellationToken, bool includeRelated = false);

        /// <summary>
        /// Get all tasks
        /// </summary>
        /// <param name="cansellationToken"></param>
        /// <param name="includeRelated"></param>
        /// <returns></returns>
        Task<ICollection<TaskResponse>> GetAllAsync(CancellationToken cansellationToken, bool includeRelated = false);

        /// <summary>
        /// Insert new Task into DataBase 
        /// </summary>
        /// <param name="task">Task</param>
        Task AddAsync(CreateTaskRequest task, CancellationToken cancellationToken);

        /// <summary>
        /// Insert new Tasks into DataBase 
        /// </summary>
        /// <param name="tasks">Task</param>
        Task AddRangeAsync(IEnumerable<CreateTaskRequest> tasks, CancellationToken cancellationToken);

        /// <summary>
        /// Update existing Task in DataBase 
        /// </summary>
        /// <param name="id">id of task</param>
        /// <param name="task">Task</param>
        /// <param name="cancellationToken"></param>
        Task UpdateAsync(SaveTaskRequest task, CancellationToken cancellationToken);

        /// <summary>
        /// Remove existing Task from DataBase 
        /// </summary>
        /// <param name="id">Task id</param>
        Task RemoveAsync(long id, CancellationToken cancellationToken);
    }
}
