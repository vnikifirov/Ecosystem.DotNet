namespace TODOList.Business.Repository.Context
{
    public interface ITaskRepository
    {
        /// <summary>
        /// Get All Items
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ICollection{T}"/></returns>
        Task<List<TODOList.Business.Context.Models.Item>> GetTasksAsync(CancellationToken cancellationToken);
    }
}
