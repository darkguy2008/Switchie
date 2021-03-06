using System;
using System.Threading;
using System.Windows.Forms;

namespace Switchie
{
    static class Program
    {
        public static CancellationToken ApplicationClosing = new CancellationToken();
        public static WindowsVersion WindowsVersion = new WindowsVersion();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
