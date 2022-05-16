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
using TasksTests;
using Business.Services.Domain.Requests;
using System.Linq;
using Business.Services.Domain.Responses;
using System;
using Business.Mapping;

namespace ProjectTests
{
    public class ProjectTest
    {
        private IMapper _mapper = GetMapper();
        private IProjectService _projectService;

        //private ProjectQuery _query = new ProjectQuery { ProjectId = 1 };
        public int _projectId = 1;
        private string _name = "Activity";
        private DateTime _start = DateTime.Now;
        private DateTime _completion = DateTime.Now.AddDays(3);
        private long _priority = 9;
        private Context.Models.ProjectStatus _status = Context.Models.ProjectStatus.NotStarted;

        public IList<CreateProjectRequest> _projects { get; set; }

        // setup
        [SetUp]
        public void Setup()
        {
            var contextFactory = FakeDbContext.Get("Projects");
            var projectRepository = new ProjectRepository(contextFactory);
            _projects = new List<CreateProjectRequest>();

            //_taskService = new TaskService(GetTaskRepository(), GetMapper());
            _projectService = new ProjectService(projectRepository, _mapper);
            
            var createdProject = new CreateProjectRequest
            {
                Name = _name,
                Start = _start,
                Completion = _completion,
                Priority = _priority,
                Status = _status         
            };

            _projectService.AddAsync(createdProject, CancellationToken.None);

            // To get random numbers 
            var rnd = new Random();
            // To get random project status
            var values = Enum.GetValues(typeof(Context.Models.ProjectStatus));

            // Create new projects
            for (int i = 0; i < 5; i++)
            {
                var created = new CreateProjectRequest
                {
                    Name = $"Project number: {i + 1}",
                    Priority = rnd.Next(1, 8),
                    Status = (Context.Models.ProjectStatus)values.GetValue(rnd.Next(values.Length))
                };

                // Assign as global var
                _projects.Add(created);
            }

            // Add new projects into DB
            _projectService.AddRangeAsync(_projects, CancellationToken.None);
        }

        [Test, Order(0)]
        public async Task Projects_AddOne_ShouldOk()
        {
            var result = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.AreEqual(result.Name, _name);
        }

        // tests
        [Test, Order(1)]
        public async Task Projects_GetAllByQueryId_ShouldNotNull()
        {
            var queryObj = new ProjectQuery { ProjectId = _projectId };
            var result = await _projectService.GetAllAsync(queryObj, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.Count);
        }

        // TODO: Unit test for sorting by acsending or decsending is incorrect
        // tests
        [Test, TestCase("Priority", false), Order(3)]
        public async Task Projects_GetAllByFilterAndSortingAsc_ShouldNotNull(string columnName, bool isSortAcsending)
        {
            var queryObj = new ProjectQuery { IsSortAcsending = isSortAcsending, SortBy = columnName };
            var result = await _projectService.GetAllAsync(queryObj, CancellationToken.None);
            var project = result.FirstOrDefault();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(_priority, project.Priority);
        }

        [Test, TestCase("Priority", true), Order(3)]
        public async Task Projects_GetAllByFilterAndSortingDesc_ShouldNotNull(string columnName, bool isSortAcsending)
        {
            var queryObj = new ProjectQuery { IsSortAcsending = isSortAcsending, SortBy = columnName };
            var result = await _projectService.GetAllAsync(queryObj, CancellationToken.None);
            var project = result.LastOrDefault();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(_priority, project.Priority);
        }

        [Test, TestCase("Activity"), Order(3)]
        public async Task Projects_GetAllByFilter_ShouldNotNull(string columnName)
        {
            var queryObj = new ProjectQuery { Name = columnName };
            var result = await _projectService.GetAllAsync(queryObj, CancellationToken.None);
            var project = result.LastOrDefault();

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_name, project.Name);
        }

        // tests
        [Test, Order(4)]
        public async Task Projects_GetAll_ShouldNotNull()
        {
            var result = await _projectService.GetAllAsync(queryObj: null, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(6, result.Count);
        }

        [Test, Order(5)]
        public async Task Projects_UpdateProjectName_ShouldOk()
        {
            var newName = "Free time";

            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(project);
            Assert.AreNotEqual(newName, project.Name);

            var saveProject = new SaveProjectRequest();
            var updated = _mapper.Map(project, saveProject);
            updated.Name = newName;

            await _projectService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.IsNotNull(resultUpdated);
            Assert.AreEqual(resultUpdated.Name, newName);
        }

        [Test, Order(6)]
        public async Task Projects_UpdateProjectStartDate_ShouldOk()
        {
            var newStartDate = DateTime.Now;

            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(project);
            Assert.AreNotEqual(newStartDate, project.Start);

            var saveProject = new SaveProjectRequest();
            var updated = _mapper.Map(project, saveProject);
            updated.Start = newStartDate;

            await _projectService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.IsNotNull(resultUpdated);
            Assert.AreEqual(resultUpdated.Start, newStartDate);
        }

        [Test, Order(7)]
        public async Task Projects_UpdateProjectCompletioDate_ShouldOk()
        {
            var newCompletionDate = DateTime.Now.AddDays(5);

            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(project);
            Assert.AreNotEqual(newCompletionDate, project.Start);

            var saveProject = new SaveProjectRequest();
            var updated = _mapper.Map(project, saveProject);
            updated.Completion = newCompletionDate;

            await _projectService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.IsNotNull(resultUpdated);
            Assert.AreEqual(resultUpdated.Completion, newCompletionDate);
        }

        [Test, Order(8)]
        public async Task Projects_UpdateProjectStatus_ShouldOk()
        {
            var newStatus = Context.Models.ProjectStatus.Active;

            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(project);
            Assert.AreNotEqual(newStatus, project.Status);

            var saveProject = new SaveProjectRequest();
            var updated = _mapper.Map(project, saveProject);
            updated.Status = newStatus;

            await _projectService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.IsNotNull(resultUpdated);
            Assert.AreEqual(resultUpdated.Status, newStatus);
        }

        // TODO: I have to make a limit for priority by validators and in database by constraints, say it can't be greate than 10 and less than 1
        [Test, Order(9)]
        public async Task Projects_UpdateProjectPriority_ShouldOk()
        {
            var newPriority = 10;

            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.NotNull(project);
            Assert.AreNotEqual(newPriority, project.Priority);

            var saveProject = new SaveProjectRequest();
            var updated = _mapper.Map(project, saveProject);
            updated.Priority = newPriority;

            await _projectService.UpdateAsync(updated, CancellationToken.None);

            var resultUpdated = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.IsNotNull(resultUpdated);
            Assert.AreEqual(resultUpdated.Priority, newPriority);
        }

        // TODO: I have to make check for RemoveAsync if project id is 0 or project not found
        [Test, Order(10)]
        public async Task Projects_Delete_ShouldOk()
        {
            var project = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);
            await _projectService.RemoveAsync(project.Id, CancellationToken.None);

            var resultEmpty = await _projectService.GetByIdAsync(_projectId, CancellationToken.None);

            Assert.Null(resultEmpty);
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

    }
}
