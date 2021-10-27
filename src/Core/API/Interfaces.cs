using System;

namespace Switchie
{

    public interface IIVirtualDesktop { }

    public interface IWindowsVirtualDesktopManager
    {
        void PinApplication(IntPtr hWnd);
        int FromDesktop(IWindowsVirtualDesktop desktop);
        IWindowsVirtualDesktop FromWindow(IntPtr hWnd);
    }

    public interface IWindowsVirtualDesktop
    {
        int Count { get; }
        void MakeVisible();
        void MoveWindow(IntPtr hWnd);
        IIVirtualDesktop ivd { get; set; }
        IWindowsVirtualDesktop Current { get; }
        IWindowsVirtualDesktop FromIndex(int index);
    }

}