namespace SwiftCode.Core.Interfaces.Services
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using SwiftCode.Core.Interfaces.Models.Common;

    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<IEnumerable<T>> FetchDataAsync<T>(string filePath) where T : BaseModel;
        Task SaveAsync<T>(IEnumerable<T> data) where T : BaseModel;
    }
}
