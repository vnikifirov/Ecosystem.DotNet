
namespace SwiftCode.Core.Utilities
{
    using System.Reflection;

    public static class ProjectAssembly
    {
        public static string Get()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
