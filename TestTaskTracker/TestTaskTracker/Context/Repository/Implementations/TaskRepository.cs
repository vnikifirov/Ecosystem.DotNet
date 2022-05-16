using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Context.Repository.Interfaces;
using Context;

namespace Context.Repository.Implementations
{
    /// <inheritdoc/>
    public class TaskRepository : ITaskRepository
    {
        private readonly Func<TasksContext> _contextFactory;

        /// <inheritdoc/>
        public TaskRepository(Func<TasksContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc/>
        public async Task AddTaskAsync(Models.Task task, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Add new task into DB context
            await context.AddAsync(task, cancellationToken);

            // Save new task into DataBase
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddTasksAsync(IEnumerable<Models.Task> tasks, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Add new tasks into DB context
            await context.AddRangeAsync(tasks, cancellationToken);

            // Save new tasks into DataBase
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Models.Task> GetTaskByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false)
        {
            using var context = _contextFactory();

            // Invalid Operation, Error connection is closed. 
            // This could happen if you do not implement Async operation all the way up.
            // Soluion of issue - https://github.com/dotnet/efcore/issues/19842
            if (!includeRelated) return await context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            return await context.Tasks
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ICollection<Models.Task>> GetTasksAsync(CancellationToken cancellationToken, bool includeRelated = false)
        {
            using var context = _contextFactory();
            IQueryable<Models.Task> query = null;

            if (!includeRelated) query = context.Tasks;
            else
            {
                // ? If it's true then returning and the related data, also.
                query = context.Tasks.Include(t => t.Project);
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RemoveTaskAsync(Models.Task task, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Remove task from DB context
            context.Remove(task);

            // Save changes into DataBase
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateTaskAsync(Models.Task task, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();
            
            // How to add existing task to project by project id
            // Solution - https://stackoverflow.com/questions/45759143/how-to-insert-a-record-into-a-table-with-a-foreign-key-using-entity-framework-in
            
            // Add changes of tasks entity into DB context
            context.Entry(task).State = EntityState.Modified;

            // Save changes into DataBase
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
