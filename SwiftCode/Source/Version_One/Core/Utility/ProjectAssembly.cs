namespace bank_identification_code.Core.Utility
{
  using System;
  using System.Reflection;

    // ? Class helper to get a project assembly/namespace/app name
    public static class ProjectAssembly
    {
        public static string Get()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}