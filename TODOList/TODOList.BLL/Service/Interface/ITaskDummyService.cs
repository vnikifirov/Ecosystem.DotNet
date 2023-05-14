using TODOList.Business.Context.Models;

namespace TODOList.BLL.Service.Interface
{
    public interface ITaskDummyService
    {
        Task<List<Item>> GetAllTasksAsync();
    }
}
