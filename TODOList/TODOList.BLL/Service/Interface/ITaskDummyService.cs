using TODOList.Business.Context.Models;

namespace TODOList.BLL.Service.Interface
{
    public interface ITaskDummyService
    {
        Task<IList<Item>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    }
}
