using System.Runtime.InteropServices;

namespace OsWarningDemo.App;

/// <summary>
/// Detects whether the app is running on a supported OS (Windows 10 or Windows 11, client editions).
/// Uses the same rule as the installer (see Package.wxs) so both warnings agree.
/// </summary>
internal static class OsSupport
{
    private const byte VER_NT_WORKSTATION = 1;
    private const int Windows11FirstBuild = 22000;

    public static OsInfo GetCurrent()
    {
        // RtlGetVersion always reports the real version, unlike GetVersionEx which
        // depends on the app manifest's supportedOS entries.
        var info = new OSVERSIONINFOEXW { dwOSVersionInfoSize = Marshal.SizeOf<OSVERSIONINFOEXW>() };
        if (RtlGetVersion(ref info) != 0)
        {
            var v = Environment.OSVersion.Version;
            return new OsInfo(v.Major, v.Minor, v.Build, VER_NT_WORKSTATION);
        }

        return new OsInfo(info.dwMajorVersion, info.dwMinorVersion, info.dwBuildNumber, info.wProductType);
    }

    /// <summary>Windows 10 and 11 both report major version 10; servers are excluded by product type.</summary>
    public static bool IsSupported(OsInfo os) => os.Major == 10 && os.ProductType == VER_NT_WORKSTATION;

    public static string GetFriendlyName(OsInfo os)
    {
        if (os.ProductType != VER_NT_WORKSTATION)
            return $"Windows Server (version {os.Major}.{os.Minor}, build {os.Build})";

        return (os.Major, os.Minor) switch
        {
            (10, _) when os.Build >= Windows11FirstBuild => $"Windows 11 (build {os.Build})",
            (10, _) => $"Windows 10 (build {os.Build})",
            (6, 3) => "Windows 8.1",
            (6, 2) => "Windows 8",
            (6, 1) => "Windows 7",
            _ => $"Windows {os.Major}.{os.Minor} (build {os.Build})",
        };
    }

    [DllImport("ntdll.dll")]
    private static extern int RtlGetVersion(ref OSVERSIONINFOEXW versionInfo);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OSVERSIONINFOEXW
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
        public ushort wServicePackMajor;
        public ushort wServicePackMinor;
        public ushort wSuiteMask;
        public byte wProductType;
        public byte wReserved;
    }
}

internal readonly record struct OsInfo(int Major, int Minor, int Build, byte ProductType);
