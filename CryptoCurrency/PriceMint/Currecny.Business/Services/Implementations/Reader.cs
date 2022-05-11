using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.Business.Services.Implementations
{
    public class Reader
    {
        public static async Task<string> ReadAsync(Uri url)
        {
            var web = new HttpClient();
            using var stream = await web.GetStreamAsync(url);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
