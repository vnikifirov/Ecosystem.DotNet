using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TODOList.Business.Context.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TODOList.BLL.Service.Implementation
{
    internal class CacheService
    {
        private IList<Item> _context { get; set; }


        public async Task<Item> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_context == null)
                this.Init();

            var key = $"task_{id}";

            var task = _cacheService.Get(key);
            if (task == null)
            {
                task = await Task.Run(() => _taskService.GetTaskByIdAsync(id, cancellationToken);
                _cacheService.Set(key, task);
            }

            return task as Item;
        }   
    }}
