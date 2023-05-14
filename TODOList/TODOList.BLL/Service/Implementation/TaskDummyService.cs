using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOList.BLL.Service.Interface;
using TODOList.Business.Context.Models;

namespace TODOList.BLL.Service.Implementation
{
    public class TaskDummyService : ITaskDummyService
    {
        public async Task<List<Item>> GetAllTasksAsync()
        {
            return await Task.Run(() => new List<Item>()
            {
                new Item { Id = Guid.NewGuid(), Name = "Get done current work", IsCompleted = true },
                new Item { Id = Guid.NewGuid(), Name = "Send gift to my wife", IsCompleted = false }
            });
        }
    }
}
