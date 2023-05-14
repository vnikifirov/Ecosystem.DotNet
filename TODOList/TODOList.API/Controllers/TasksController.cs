using Microsoft.AspNetCore.Mvc;
using TODOList.BLL.Service.Implementation;
using TODOList.BLL.Service.Interface;
using TODOList.Business.Context.Models;

namespace TODOList.API.Controllers
{
    [ApiController]
    [Route("Todo")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        public ITaskDummyService _taskService { get; set; }

        public TodoController(ILogger<TodoController> logger, ITaskDummyService _service)
        {
            _logger = logger;
            _taskService = _service;
        }

        [HttpGet(Name = "Tasks")]
        public async Task<List<Item>> Get() => await _taskService.GetAllTasksAsync();
    }
}