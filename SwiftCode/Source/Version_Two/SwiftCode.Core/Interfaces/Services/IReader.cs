
namespace SwiftCode.Core.Interfaces.Services
{
    using SwiftCode.Core.Interfaces.Models.Common;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IReader
    {
        Task<IEnumerable<T>> ReadAsync<T>(string fileName, Encoding encoding) where T : BaseModel;
    }
}
