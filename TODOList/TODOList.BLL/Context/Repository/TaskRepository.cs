using Microsoft.EntityFrameworkCore;
using TODOList.BLL.Context.Context;
using TODOList.Business.Repository.Context;

namespace TODOList.Business.Context
{
    /// <inheritdoc/>
    public class TaskRepository : ITaskRepository
    {
        private readonly Func<TodoListContext> _contextFactory;

        /// <inheritdoc/>
        public TaskRepository(Func<TodoListContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc/>
        public async Task<List<Models.Item>> GetTasksAsync(CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            return await context.Todo.ToListAsync();
        }
    }
}
