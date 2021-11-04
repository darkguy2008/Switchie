using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Switchie
{

    public class WindowManager
    {
        static List<IntPtr> hWndBlacklist = new List<IntPtr>();

        static int GetWindowZOrder(IntPtr hWnd)
        {
            var zOrder = -1;
            while ((hWnd = WinAPI.GetWindow(hWnd, WinAPI.GW_HWNDNEXT)) != IntPtr.Zero) zOrder++;
            return zOrder;
        }

        public static List<Window> GetOpenWindows()
        {
            List<Window> rv = new List<Window>();
            IntPtr shellWindow = WinAPI.GetShellWindow();
            string[] classBlacklist = new string[] {
                "Windows.UI.Core.CoreWindow"
            };

            WinAPI.EnumWindows((IntPtr hWnd, int lParam) =>
            {
                if (hWndBlacklist.Contains(hWnd)) return true;
                if (hWnd == shellWindow) return true;
                if (!WinAPI.IsWindowVisible(hWnd)) return true;

                int length = WinAPI.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                WinAPI.GetWindowText(hWnd, builder, length + 1);

                WinAPI.RECT rct = new WinAPI.RECT();
                WinAPI.GetWindowRect(hWnd, ref rct);

                IntPtr nRet;
                StringBuilder className = new StringBuilder(256);
                nRet = WinAPI.GetClassName(hWnd, className, className.Capacity);
                if (classBlacklist.Contains(className.ToString())) return true;

                int index = 0;
                WinAPI.GetWindowThreadProcessId(hWnd, out uint pid);
                try { index = WindowsVirtualDesktopManager.GetInstance().FromDesktop(WindowsVirtualDesktopManager.GetInstance().FromWindow((IntPtr)hWnd)); }
                catch
                {
                    hWndBlacklist.Add(hWnd);
                    return true;
                }
                if (index < 0) return true;

                int hIcon = WinAPI.SendMessage(hWnd, WinAPI.WM_GETICON, WinAPI.ICON_SMALL2, 0);
                if (hIcon == 0) { hIcon = WinAPI.GetClassLongPtr(hWnd, WinAPI.GCL_HICON); }
                if (hIcon == 0) { hIcon = WinAPI.LoadIcon(IntPtr.Zero, (IntPtr)WinAPI.IDI_APPLICATION); }

                rv.Add(new Window()
                {
                    Handle = hWnd,
                    Title = builder.ToString(),
                    ProcessID = pid,
                    Class = className.ToString(),
                    ZOrder = GetWindowZOrder(hWnd),
                    Icon = hIcon != 0 ? new Bitmap(Icon.FromHandle((IntPtr)hIcon).ToBitmap(), 16, 16) : null,
                    IsActive = hWnd == WinAPI.GetForegroundWindow(),
                    Dimensions = new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top),
                    VirtualDesktopIndex = index
                });

                return true;
            }, 0);

            return rv;
        }

        public static Window GetActiveWindow()
        {
            var hwnd = WinAPI.GetForegroundWindow();
            return GetOpenWindows().SingleOrDefault(x => x.Handle == hwnd);
        }

        public static void SetAlwaysOnTop(IntPtr handle, bool value) => WinAPI.SetWindowPos(handle, value ? WinAPI.HWND_TOPMOST : WinAPI.HWND_NOTOPMOST, 0, 0, 0, 0, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOSIZE | WinAPI.SWP_SHOWWINDOW);
    }
}
