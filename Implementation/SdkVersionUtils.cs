namespace WcfApplicationInsights.Implementation
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class SdkVersionUtils
    {
        public static string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        }
    }
}
