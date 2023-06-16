using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOList.BLL.Service.Interface;
using TODOList.Business.Context.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TODOList.BLL.Service.Implementation
{
    public class TaskDummyService : ITaskDummyService
    {
        private IList<Item> _context { get; set; }

        public TaskDummyService(IList<Item> context)
        {
            _context = context;
        }

        public async Task<IList<Item>> GetAllTasksAsync(CancellationToken cancellationToken = default)
        {
            if (_context == null)
                this.Init();

            return await Task.Run(() => _context);
        }

        public async Task<Item> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_context == null)
                this.Init();

            return await Task.Run(() => _context.FirstOrDefault(item => item.Id == id));
        }

        /// <summary>
        /// Init records for tests
        /// </summary>
        private void Init() 
        {
            this._context = new List<Item>()
            {
                new Item { Id = Guid.NewGuid(), Name = "Get done current work", IsCompleted = true },
                new Item { Id = Guid.NewGuid(), Name = "Send gift to my wife", IsCompleted = false },
            };
        }
    }
}
