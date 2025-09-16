using System.Reflection;

namespace GameTools.Client.Wpf.Common
{
    public static class VersionInfo
    {
        public static string InformationalVersion =>
            Assembly.GetEntryAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        public static string UserVersion => InformationalVersion.Split('+')[0];
    }
}
