using AutoMapper;
using Business.Extentions;
using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;
using Business.Services.Interfaces;
using Context.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Services.Implementations
{
    /// <inheritdoc/>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task AddAsync(CreateProjectRequest project, CancellationToken cancellationToken)
        {
            var created = _mapper.Map<CreateProjectRequest, Context.Models.Project>(project);

            await _projectRepository.AddProjectAsync(created, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<CreateProjectRequest> projects, CancellationToken cancellationToken)
        {
            var createdProjects = _mapper.Map<IEnumerable<CreateProjectRequest>, IEnumerable<Context.Models.Project>>(projects);

            await _projectRepository.AddProjectsAsync(createdProjects, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ICollection<ProjectResponse>> GetAllAsync(ProjectQuery queryObj, CancellationToken cansellationToken)
        {
            var results = await _projectRepository.GetProjectsAsync(cansellationToken);

            var query = results.AsQueryable();

            var columnsMap = new Dictionary<string, Expression<Func<Context.Models.Project, object>>>
            {
                ["Name"] = p => p.Name,
                ["Priority"] = p => p.Priority,
                ["Start"] = p => p.Start,
                ["Completion"] = p => p.Completion,
                ["Status"] = p => p.Status
            };

            query = query.ApplyFilteringByExactValue(queryObj);
            query = query.ApplyOrdering(queryObj, columnsMap);
            query = query.ApplyPaging(queryObj?.Page, queryObj?.PageSize);

            //results = await query.ToListAsync();
            results = query.ToList();

            var response = _mapper.Map<ICollection<ProjectResponse>>(results);

            return response;
        }

        /// <inheritdoc/>
        public async Task<ProjectResponse> GetByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id, cancellationToken, includeRelated);

            var response = _mapper.Map<ProjectResponse>(project);

            return response;
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(long id, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id, cancellationToken);

            await _projectRepository.RemoveProjectAsync(project, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(SaveProjectRequest project, CancellationToken cancellationToken)
        {
            var updated = _mapper.Map<SaveProjectRequest, Context.Models.Project>(project);

            await _projectRepository.UpdateProjectAsync(updated, cancellationToken);
        }
    }
}