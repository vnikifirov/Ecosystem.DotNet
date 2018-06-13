namespace SwiftCode.Core.Interfaces.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    // ? Generic Repository
    public interface IRepository<Entity> where Entity : class
    {
        Task<IEnumerable<Entity>> GetAllAsync(bool includeReleted = false);
        Task<Entity> GetByVKEYAsync(string VKEY, bool includeReleted = false);
        void AddAsync(Entity entity);
        void AddAsync(IEnumerable<Entity> entities);
        void Update(Entity entity);
        void Remove(Entity entity);
    }
}
