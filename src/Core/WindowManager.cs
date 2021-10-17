using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Switchie
{

    public class WindowManager
    {
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

            WinAPI.EnumWindows((IntPtr hWnd, int lParam) =>
            {
                if (hWnd == shellWindow) return true;
                if (!WinAPI.IsWindowVisible(hWnd)) return true;

                int length = WinAPI.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                WinAPI.GetWindowText(hWnd, builder, length + 1);

                WinAPI.RECT rct = new WinAPI.RECT();
                WinAPI.GetWindowRect(hWnd, ref rct);

                WinAPI.GetWindowThreadProcessId(hWnd, out uint pid);
                int index = WindowsVirtualDesktopManager.FromDesktop(WindowsVirtualDesktopManager.FromWindow((IntPtr)hWnd));

                int hIcon = WinAPI.SendMessage(hWnd, WinAPI.WM_GETICON, WinAPI.ICON_SMALL2, 0);
                if (hIcon == 0) { hIcon = WinAPI.GetClassLongPtr(hWnd, WinAPI.GCL_HICON); }
                if (hIcon == 0) { hIcon = WinAPI.LoadIcon(IntPtr.Zero, (IntPtr)WinAPI.IDI_APPLICATION); }

                rv.Add(new Window()
                {
                    Handle = hWnd,
                    Title = builder.ToString(),
                    ProcessID = pid,
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