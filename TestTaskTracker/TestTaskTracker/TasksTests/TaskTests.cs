using Business.Services.Implementations;
using Business.Services.Interfaces;
using NUnit.Framework;

using Moq;
using Context.Repository.Interfaces;
using System.Threading;
using System.Collections.Generic;
using AutoMapper;
using Business;
using System.Threading.Tasks;
using Context.Repository.Implementations;
using System.Linq;
using Business.Services.Domain.Requests;
using Business.Mapping;
using Microsoft.Extensions.Options;
using Business.Configuration;

namespace TasksTests
{
    public class Tests
    {
        private IMapper _mapper = GetMapper();
        private ITaskService _taskService;

        private string _name = "Cover my code by unit tests";
        private long _assignedProject = 5;
        private string _descTask = "I have to cover all my code by unit tests";
        private long _priority = 1;
        private Context.Models.TaskStatus _status = Context.Models.TaskStatus.ToDo;

        // setup
        [SetUp]
        public void Setup()
        {
            var contextFactory = FakeDbContext.Get("Tasks");
            var tasksRepository = new TaskRepository(contextFactory);

            //_taskService = new TaskService(GetTaskRepository(), GetMapper());
            _taskService = new TaskService(tasksRepository, _mapper, GetOptions());

            var createdProject = new CreateTaskRequest
            {
                Name = _name,
                Id_Project = _assignedProject,
                Description = _descTask,
                Priority = _priority,
                Status = _status
            };

            _taskService.AddAsync(createdProject, CancellationToken.None);
        }

        // tests
        [Test, Order(0)]
        public async Task Tasks_GetAll_ShouldNotNull()
        {
            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);
        }

        [Test, Order(1)]
        public async Task Tasks_Add_ShouldOk()
        {
            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.FirstOrDefault().Name, _name);
        }

        // tests
        [Test, Order(2)]
        public async Task Tasks_AssignedToProjectById_ShouldOk()
        {
            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var assignedProjectId = result.FirstOrDefault().Id_Project;

            Assert.AreEqual(_assignedProject, assignedProjectId);
        }

        // tests
        [Test, Order(3)]
        public async Task Tasks_UnassignTaskFromProjectById_ShouldOk()
        {
            int? newIdProject = null;
            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var assigned= result.FirstOrDefault();

            Assert.AreEqual(_assignedProject, assigned.Id_Project);

            var saveTask = new SaveTaskRequest();
            var updated = _mapper.Map(assigned, saveTask);
            updated.Id_Project = newIdProject;

            await _taskService.UpdateAsync(updated, CancellationToken.None);

            var resultUnassigned = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(resultUnassigned.Count, 1);

            var unassignedProjectId = resultUnassigned.FirstOrDefault().Id_Project;

            Assert.Null(unassignedProjectId);
        }

        [Test, Order(4)]
        public async Task Tasks_UpdateTaskName_ShouldOk()
        {
            var newName = "Create middleware";

            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var task = result.FirstOrDefault();

            Assert.NotNull(task);
            Assert.AreNotEqual(newName, task.Name);

            var saveTask = new SaveTaskRequest();
            var updated = _mapper.Map(task, saveTask);
            updated.Name = newName;

            await _taskService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(resultUpdated);
            Assert.IsNotEmpty(resultUpdated);
            Assert.AreEqual(resultUpdated.Count, 1);

            var resultUpdatedTask = resultUpdated.FirstOrDefault();

            Assert.IsNotNull(resultUpdatedTask);
            Assert.AreEqual(resultUpdatedTask.Name, newName);
        }

        [Test, Order(5)]
        public async Task Tasks_UpdateTaskStatus_ShouldOk()
        {
            var newStatus = Context.Models.TaskStatus.InProgress;

            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var task = result.FirstOrDefault();

            Assert.NotNull(task);
            Assert.AreNotEqual(newStatus.ToString(), task.Status.ToString());

            var saveTask = new SaveTaskRequest();
            var updated = _mapper.Map(task, saveTask);
            updated.Status = newStatus;

            await _taskService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(resultUpdated);
            Assert.IsNotEmpty(resultUpdated);
            Assert.AreEqual(resultUpdated.Count, 1);

            var resultUpdatedTask = resultUpdated.FirstOrDefault();

            Assert.IsNotNull(resultUpdatedTask);
            Assert.AreEqual(resultUpdatedTask.Status.ToString(), newStatus.ToString());
        }

        [Test, Order(6)]
        public async Task Tasks_UpdateTaskDescription_ShouldOk()
        {
            var newDesc = "You have to create middleware to catch all your exception in business logic layer";

            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var task = result.FirstOrDefault();

            Assert.NotNull(task);
            Assert.AreNotEqual(newDesc, task.Description);

            var saveTask = new SaveTaskRequest();
            var updated = _mapper.Map(task, saveTask);
            updated.Description = newDesc;

            await _taskService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(resultUpdated);
            Assert.IsNotEmpty(resultUpdated);
            Assert.AreEqual(resultUpdated.Count, 1);

            var resultUpdatedTask = resultUpdated.FirstOrDefault();

            Assert.IsNotNull(resultUpdatedTask);
            Assert.AreEqual(resultUpdatedTask.Description, newDesc);
        }

        // TODO: I have to make a limit for priority by validators and in database by constraints, say it can't be greate than 10 and less than 1
        [Test, Order(7)]
        public async Task Tasks_UpdateTaskPriority_ShouldOk()
        {
            var newPriority = 10;

            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            var task = result.FirstOrDefault();

            Assert.NotNull(task);
            Assert.AreNotEqual(newPriority, task.Priority);

            var saveTask = new SaveTaskRequest();
            var updated = _mapper.Map(task, saveTask);
            updated.Priority = newPriority;

            await _taskService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(resultUpdated);
            Assert.IsNotEmpty(resultUpdated);
            Assert.AreEqual(resultUpdated.Count, 1);

            var resultUpdatedTask = resultUpdated.FirstOrDefault();

            Assert.IsNotNull(resultUpdatedTask);
            Assert.AreEqual(resultUpdatedTask.Priority, newPriority);
        }

        [Test, Order(8)]
        public async Task Tasks_Delete_ShouldOk()
        {
            var result = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);

            await _taskService.RemoveAsync(result.FirstOrDefault().Id, CancellationToken.None);

            var resultEmpty = await _taskService.GetAllAsync(CancellationToken.None);

            Assert.IsNotNull(resultEmpty);
            Assert.IsEmpty(resultEmpty);
            Assert.AreEqual(resultEmpty.Count, 0);
        }


        // private 
        private static ITaskRepository GetTaskRepository()
        {
            var taskRepository = new Mock<ITaskRepository>();
            taskRepository.Setup(r => r.GetTasksAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<Context.Models.Task> { new Context.Models.Task() });
            return taskRepository.Object;
        }

        private static IMapper GetMapper()
        {
            var configMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(TasksMapper));
            });
            var mapper = new Mapper(configMapper);

            return mapper;
        }

        private static IOptions<TaskConfig> GetOptions()
        {
            var options = new Mock<IOptions<TaskConfig>>();

            return options.Object;
        }
    }
}