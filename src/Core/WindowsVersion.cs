namespace Switchie
{

    public class WindowsVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public string Name { get; set; }

        public WindowsVersion()
        {
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            Major = int.Parse(registryKey.GetValue("CurrentMajorVersionNumber").ToString());
            Minor = int.Parse(registryKey.GetValue("CurrentMinorVersionNumber").ToString());
            Build = int.Parse(registryKey.GetValue("CurrentBuildNumber").ToString());
            Name = registryKey.GetValue("DisplayVersion", string.Empty).ToString().ToUpperInvariant().Trim();
        }

        // Windows version detection could improve, but I think this works, tested on:
        // Microsoft Windows [Version 10.0.22000.282]  (Windows 11)
        // Microsoft Windows [Version 10.0.19043.1288] (Windows 10)
        // Microsoft Windows [Version 10.0.17763.2237] (Windows 10 LTSC)
        public bool IsWin11() => Major == 10 && Minor == 0 && Build >= 22000 && Name == "21H2";
        public bool IsWin10() => Major == 10 && Minor == 0 && Build > 17763 && Build < 22000;
        public bool IsWin10LTSC() => Major == 10 && Minor == 0 && Build <= 17763;
    }

}