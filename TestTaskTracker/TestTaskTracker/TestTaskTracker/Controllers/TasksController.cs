using Business.Services.Domain.Requests;
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
    /// Task controller
    /// </summary>
    [Route("v1")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly ITaskService _taskService;

        public TasksController(ILogger<TasksController> logger, ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        /// <summary>
        /// Get task by id
        /// </summary>
        /// <param name="id">Id of task</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="TaskResponse"/></returns>
        [HttpGet]
        [Route("tasks/{id}")]
        [ProducesResponseType(typeof(TaskResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<TaskResponse> GetTask(long id, CancellationToken cancellationToken)
        {
            return await _taskService.GetByIdAsync(id, cancellationToken);
        }

        /// <summary>
        /// Get tasks
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="TaskResponse"/></returns>
        [HttpGet]
        [Route("tasks")]
        [ProducesResponseType(typeof(ICollection<TaskResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ICollection<TaskResponse>> GetTasks(CancellationToken cancellationToken)
        {
            return await _taskService.GetAllAsync(cancellationToken);
        }

        /// <summary>
        /// Create new task
        /// </summary>
        /// <param name="resource">New task</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="CreateTaskRequest"/></returns>
        [HttpPost]
        [Route("tasks")]
        [ProducesResponseType(typeof(CreateTaskRequest), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest resource, CancellationToken cancellationToken)
        {
            if (resource == null) return BadRequest();

            await _taskService.AddAsync(resource, cancellationToken);

            return Ok(resource);
        }

        /// <summary>
        /// Update existing task
        /// </summary>
        /// <param name="id">id of task</param>
        /// <param name="resource">Updated task</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="SaveTaskRequest"/></returns>
        [HttpPut]
        [Route("tasks")]
        [ProducesResponseType(typeof(SaveTaskRequest), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(long id, [FromBody] SaveTaskRequest resource, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();
            if (resource == null) return BadRequest();

            await _taskService.UpdateAsync(resource, cancellationToken);

            return Ok(resource);
        }


        /// <summary>
        /// Delete existing task by id
        /// </summary>
        /// <param name="id">id of task</param>
        /// <returns><see cref="long"/></returns>
        [HttpDelete]
        [Route("tasks/{id}")]
        [ProducesResponseType(typeof(long), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            if (id <= 0) return NotFound();

            await _taskService.RemoveAsync(id, cancellationToken);
        
            return Ok(id);
        }
    }
}
