using System.Reflection;

namespace Test.Services
{
    public static class AppInfoService
    {
        public static string GetFormattedVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null
                ? $"v{version.Major}.{version.Minor}.{version.Build}"
                : "v1.0.0";
        }
    }
}