using System;
using System.Threading.Tasks;
using bank_identification_code.Core.Models;
using bank_identification_code.Persistence.Repositories;

namespace bank_identification_code.Persistence
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}