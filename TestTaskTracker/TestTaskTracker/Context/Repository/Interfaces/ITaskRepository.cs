using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Context.Models;

namespace Context.Repository.Interfaces
{
    /// <summary>
    /// Tasks Data Layer
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Get All Tasks
        /// </summary>
        /// <param name="includeRelated"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ICollection{T}"/></returns>
        Task<ICollection<Models.Task>> GetTasksAsync(CancellationToken cancellationToken, bool includeRelated = false);

        /// <summary>
        /// Get Task By Id
        /// </summary>
        /// <param name="id">Id of Task</param>
        /// <param name="includeRelated"></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="Models.Task"/></returns>
        Task<Models.Task> GetTaskByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false);

        /// <summary>
        /// Insert new Task into DataBase 
        /// </summary>
        /// <param name="task">Task</param>
        System.Threading.Tasks.Task AddTaskAsync(Models.Task task, CancellationToken cancellationToken);

        /// <summary>
        /// Insert new Tasks into DataBase 
        /// </summary>
        /// <param name="tasks">Task</param>
        System.Threading.Tasks.Task AddTasksAsync(IEnumerable<Models.Task> tasks, CancellationToken cancellationToken);

        /// <summary>
        /// Update existing Task in DataBase 
        /// </summary>
        /// <param name="task">Task</param>
        System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task, CancellationToken cancellationToken);

        /// <summary>
        /// Remove existing Task from DataBase 
        /// </summary>
        /// <param name="task">Task</param>
        System.Threading.Tasks.Task RemoveTaskAsync(Models.Task task, CancellationToken cancellationToken);
    }
}

