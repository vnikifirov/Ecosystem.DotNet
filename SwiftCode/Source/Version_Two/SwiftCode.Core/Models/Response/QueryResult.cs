namespace SwiftCode.Core.Models.Response
{
    using System.Collections.Generic;
    using SwiftCode.Core.Interfaces.Models.Common;

    // ? I've a thought that should be separated like requests models
    public sealed class QueryResult<T> where T : BaseModel
    {
        public int TotalItems { get; set; }

        // ? Advantage of returning one item at time, don't load the complete set into memory.
        // ? https://docs.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/control-flow/walkthrough-implementing-ienumerable-of-t
        public IEnumerable<T> Items { get; set; }
    }
}
