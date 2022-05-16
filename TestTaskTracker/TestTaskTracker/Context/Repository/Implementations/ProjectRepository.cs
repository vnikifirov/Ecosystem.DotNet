using Context.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Context.Repository.Implementations
{
    /// <inheritdoc/>
    public class ProjectRepository : IProjectRepository
    {
        private readonly Func<TasksContext> _contextFactory;

        /// <inheritdoc/>
        public ProjectRepository(Func<TasksContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc/>
        public async Task AddProjectAsync(Models.Project project, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Add new project into DB context
            await context.AddAsync(project, cancellationToken);

            // Save new project into DataBase
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AddProjectsAsync(IEnumerable<Models.Project> projects, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Add new projects into DB context
            await context.AddRangeAsync(projects, cancellationToken);

            // Save new projects into DataBase
            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Models.Project> GetProjectByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false)
        {
            using var context = _contextFactory();

            if (!includeRelated) return await context.Projects.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            return await context.Projects
                    .Include(t => t.Tasks)
                    .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ICollection<Models.Project>> GetProjectsAsync(CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            var result = await context.Projects.ToListAsync(cancellationToken);

            return result;
        }

        /// <inheritdoc/>
        public async Task RemoveProjectAsync(Models.Project project, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Remove project from DB context
            context.Remove(project);

            // Save changes into DataBase
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateProjectAsync(Models.Project project, CancellationToken cancellationToken)
        {
            using var context = _contextFactory();

            // Add changes of tasks entity into DB context
            context.Entry(project).State = EntityState.Modified;

            // Save changes into DataBase
            await context.SaveChangesAsync();
        }
    }
}
