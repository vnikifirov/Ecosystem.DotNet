using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

using AutoMapper;
using Business.Configuration;
using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;
using Business.Services.Interfaces;

using Context.Repository.Interfaces;
using Microsoft.Extensions.Options;

namespace Business.Services.Implementations
{
    /// <inheritdoc/>
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly TaskConfig _config;

        public TaskService(ITaskRepository taskRepository, IMapper mapper, IOptions<TaskConfig> options)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _config = options.Value;
        }

        /// <inheritdoc/>
        public async Task AddAsync(CreateTaskRequest task, CancellationToken cancellationToken)
        {
            var created = _mapper.Map<CreateTaskRequest, Context.Models.Task>(task);

            await _taskRepository.AddTaskAsync(created, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<CreateTaskRequest> tasks, CancellationToken cancellationToken)
        {
            var createdTasks = _mapper.Map<IEnumerable<CreateTaskRequest>, IEnumerable<Context.Models.Task> >(tasks);

            await _taskRepository.AddTasksAsync(createdTasks, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<ICollection<TaskResponse>> GetAllAsync(CancellationToken cansellationToken, bool includeRelated = false)
        {
            var results = await _taskRepository.GetTasksAsync(cansellationToken, includeRelated);

            var response = _mapper.Map<ICollection<TaskResponse>>(results);

            return response;
        }

        /// <inheritdoc/>
        public async Task<TaskResponse> GetByIdAsync(long id, CancellationToken cancellationToken, bool includeRelated = false)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id, cancellationToken, includeRelated);

            var response = _mapper.Map<TaskResponse>(task);

            return response;
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(long id, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id, cancellationToken);

            await _taskRepository.RemoveTaskAsync(task, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(SaveTaskRequest task, CancellationToken cancellationToken)
        {
            if (!task.XPass.Equals(_config.XPass))
                throw new Exception("Not authorized");

            var taskId = task.Id;
            var source = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);

            var updated = _mapper.Map(task,source);

            await _taskRepository.UpdateTaskAsync(updated, cancellationToken);
        }
    }
}
