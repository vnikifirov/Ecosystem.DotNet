namespace bank_identification_code.Persistence.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(bool includeReleted = false);
        Task<T> GetByVKEYAsync(string VKEY, bool includeReleted = false);
        void AddAsync(T model);
        void AddAsync(IEnumerable<T> model);
        void Update(T model);
        void Remove(T model);
    }
}