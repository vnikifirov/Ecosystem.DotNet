using System.Net.NetworkInformation;
using TODOList.BLL.Service.Implementation;
using TODOList.Business.Context.Models;

namespace TODOList.BLL.Tests.Unit
{
    public class Tests
    {
        private TaskDummyService _taskService;

        private string _descTaskOne = "Cover my code by unit tests";
        private string _descTaskTwo = "Go and buy milk and banana in the grocery store store";

        // setup
        [SetUp]
        public void Setup()
        {
            _taskService = new TaskDummyService();
        }

        [Test, Order(0)]
        public async Task Tasks_GetAll_ShouldNotNull()
        {
            var result = await _taskService.GetAllTasksAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 2);
        }

        [Test, Order(1)]
        public async Task Tasks_Add_ShouldOkAndCountBeFour()
        {
            var createdTaskOne = new Item
            {
                Id = Guid.NewGuid(),
                Name = _descTaskOne,
                IsCompleted = false
            };

            var createdTaskTwo = new Item
            {
                Id = Guid.NewGuid(),
                Name = _descTaskTwo,
                IsCompleted = false
            };

            //await _taskService.AddTasksAsync(createdTaskOne);
            // _taskService.AddTasksAsync(createdTaskTwo);

            var result = await _taskService.GetAllTasksAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 4);
        }
    }
}