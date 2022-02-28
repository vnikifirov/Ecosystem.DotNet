using Business;
using Business.Services.Domain.Responses;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestTaskTracker.Controllers
{
    /// <summary>
    /// Project controller
    /// </summary>
    [Route("v2")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IProjectService _projectService;

        public ProjectsController(ILogger<ProjectsController> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        /// <summary>
        /// Get project by id
        /// </summary>
        /// <param name="id">Id of project</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ProjectResponse"/></returns>
        [HttpGet]
        [Route("projects/{id}")]
        [ProducesResponseType(typeof(ProjectResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ProjectResponse> GetProject(long id, CancellationToken cancellationToken)
        {
            return await _projectService.GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Get projects
        /// </summary>
        /// <param name="queryObj">Query object</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ProjectResponse"/></returns>
        [HttpGet]
        [Route("projects")]
        [ProducesResponseType(typeof(ICollection<ProjectResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ICollection<ProjectResponse>> GetProjects(ProjectQuery queryObj, CancellationToken cancellationToken)
        {
            return await _projectService.GetAllAsync(queryObj, cancellationToken);
        }

        /// <summary>
        /// Get project with tasks
        /// </summary>
        /// <param name="id">Id of project</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ProjectResponse"/></returns>
        [HttpGet]
        [Route("projects/view")]
        [ProducesResponseType(typeof(ICollection<ProjectResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ProjectResponse> GetProjectWithTasks(long id, CancellationToken cancellationToken)
        {
            // .Net Core 3.0 possible object cycle was detected which is not supported
            // Install the package Microsoft.AspNetCore.Mvc.NewtonsoftJson
            // Solution of problem - https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
            return await _projectService.GetByIdAsync(id, cancellationToken, true);
        }
    }
}
