using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using TODOList.Business.Repository.Context;
using TODOList.Business.Context.Models;

namespace TODOList.Controllers
{
    [BindProperties]
    public class TodoController : PageModel
    {
        private readonly ITaskRepository _context;

        [BindProperty]
        public List<Item> Tasks { get; set; } = default(List<Item>);

        [BindProperty]
        public Item TaskItem { get; set; } = default(Item);

        public TodoController(ITaskRepository context) =>_context = context;

        public async void Get(CancellationToken cancellationToken)
{
            this.Tasks = await _context.GetTasksAsync(cancellationToken);
        }
    }
}