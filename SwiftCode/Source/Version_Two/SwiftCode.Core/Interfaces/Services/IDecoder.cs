
namespace SwiftCode.Core.Interfaces.Services
{
    using SwiftCode.Core.Models.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDecoder
    {
        IStringDecoder StringDecoder { get; }
        IStringDecoder FromBase64 { get; }
        IEnumerableDecoder EnumerableDecoder { get; }
    }

    public interface IStringDecoder
    {
        Encodings Encodings { get; }
        string Decode(string source);
    }

    public interface IEnumerableDecoder
    {
        IStringDecoder Decoder { get; }
        IEnumerable<T> Decode<T>(IEnumerable<T> data) where T : class;
    }
}
