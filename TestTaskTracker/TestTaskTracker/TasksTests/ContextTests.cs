using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TasksTests;

namespace ContextTests
{
    public class ContextTests
    {
        private TasksContext _context;
        private Context.Models.Task _task;
        private Context.Models.Project _project;
        private IList<Context.Models.Task> _tasks;

        // setup
        [OneTimeSetUp]
        public void Setup()
        {
            // Create fake of DB (InMemory) by context factory
            var contextFactory = FakeDbContext.Get("TestDBContext");
            // Call factory to create new context
            _context = contextFactory();
            // Init _tasks 
            _tasks = new List<Context.Models.Task>();
        }

        [Test, Order(1)]
        public async Task Projects_AddNewProject_ShouldOk()
        {
            // Create new project
            var created = new Context.Models.Project
            {
                Name = "At work activities",
                Start = DateTime.Now,
                Completion = DateTime.Now.AddDays(5),
                Priority = 1,
                Status = Context.Models.ProjectStatus.NotStarted
            };

            // Assign as global var
            _project = created;

            // Add new project into DB context
            await _context.Projects.AddAsync(created);

            // Update DB (InMemory)
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that project was created
            Assert.NotNull(created);

            // Check that project was added in DB context
            var projectName = (await _context.Projects.FirstOrDefaultAsync()).Name;
            Assert.AreEqual(projectName, created.Name);
        }

        [Test, Order(2)]
        public async Task Projects_AddNewTask_ShouldOk()
        {
            // Create new task
            var created = new Context.Models.Task
            {
                Name = "Set constraints on columns",
                Description = "Set constraints on columns to put a limit between 1 and 10 of possible values in Projects and Tasks tables",
                Priority = 10,
                Status = Context.Models.TaskStatus.Done
            };

            // Assign as global var
            _task = created;
            _tasks.Add(created);

            // Add new task into DB context
            await _context.Tasks.AddAsync(created);
            // Save changes into DB 
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that task was created 
            Assert.NotNull(created);
            // Check that task was added in DB context
            var taskName = (await _context.Tasks.FirstOrDefaultAsync()).Name;
            Assert.AreEqual(taskName, created.Name);
        }

        [Test, Order(3), TestCase(1, 1)]
        public async Task Projects_AssignTaskByTaskIdToProject_ShouldOk(int taskId, int projectId)
        {
            // Seach task by its id
            var task =  await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            // Assign task to project by task id
            task.Id_Project = projectId;
            // Add changes into DB context
            _context.Entry(task).State = EntityState.Modified;
            // Update DB
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());
            // Check that project was assigned 
            Assert.NotNull(task.Id_Project);
        }

        [Test, Order(4), TestCase(1)]
        public async Task Projects_GetProjectAndTasks_ShouldOk(int projectId)
        {
            // Seach project with his tasks by its id
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            // Check that we have got project and it has assigned tasks 
            Assert.NotNull(project);
            Assert.AreEqual(_project.Name, project.Name);
            Assert.IsNotEmpty(project.Tasks);
        }

        [Test, Order(4), TestCase(1)]
        public async Task Projects_GetTaskAndProject_ShouldOk(int taskId)
        {
            // Seach project with his tasks by its id
            var task = await _context.Tasks
                .Include(p => p.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            // Check that we have got project and it has assigned tasks 
            Assert.NotNull(task);
            Assert.AreEqual(_task.Name, task.Name);
            Assert.NotNull(task.Project);
        }

        [Test, Order(5), TestCase(1)]
        public async Task Tasks_UnassignTaskFromProjectById_ShouldOk(int taskId)
        {
            int? newIdProject = null;
            // Search task by its id
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            // Check that task was found 
            Assert.NotNull(task);
            // Check that project is assigned
            Assert.AreNotEqual(newIdProject, task.Id_Project);

            // Unassign task from project by task id
            task.Id_Project = newIdProject;

            // Check that task was unassigned from project by task id
            Assert.Null(task.Id_Project);

            // Update task in DB context
            _context.Entry(task).State = EntityState.Modified;

            // Update DB
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that task was unassigned in DB and context
            var projectId = (await _context.Tasks.FirstOrDefaultAsync()).Id_Project;
            Assert.AreEqual(newIdProject, projectId);
        }

        // TODO: You need to make test on limtis (eg return 0, null)
        // TODO: If task wasn't found it'll throw exception, you've to make test for null exception 
        [Test, Order(6), TestCase(1)]
        public async Task Tasks_DeleteTask_ShouldOk(int taskId)
        {
            // Get one task by id and we have only one task in DB 
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            // Remove from global var
            _tasks.Remove(task);

            // Remove this task from DB
            _context.Tasks.Remove(task);

            // Save changes in DB
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that this task was removed from DB 
            var emptyResult = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            // Check that the result is null and task wasn't found 
            Assert.IsNull(emptyResult);
        }

        [Test, Order(7), TestCase(5)]
        public async Task Projects_AddManyNewTasks_ShouldOk(int numTasks)
        {
            // Create new tasks
            for (int i = 0; i < numTasks; i++)
            {
                var created = new Context.Models.Task
                {
                    Name = $"Task number: {i + 1}",
                    Description = $"Description of task number: {i + 1}",
                    Priority = 1,
                    Status = Context.Models.TaskStatus.InProgress
                };

                // Assign as global var
                _tasks.Add(created);
            }

            // Add new task into DB context
            await _context.Tasks.AddRangeAsync(_tasks);
            // Save changes into DB 
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that task was created 
            Assert.NotNull(_tasks);
            Assert.IsNotEmpty(_tasks);
            Assert.AreEqual(_tasks.Count, numTasks);
        }

        [Test, Order(8), TestCase(1)]
        public void Tasks_DeleteManyTasks_ShouldOk(int taskId)
        {
            // Remove tasks from DB context
            _context.Tasks.RemoveRange(_tasks);

            // Save changes in DB
            Assert.DoesNotThrowAsync(async () => await _context.SaveChangesAsync());

            // Check that all tasks was removed from DB 
            Assert.AreEqual(0, _context.Tasks.Count());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
