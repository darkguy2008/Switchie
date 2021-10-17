// Adapted from https://github.com/MScholtes/VirtualDesktop with a few modifications
// TODO: Needs to be adapted to the latest versions on his repo targeting Win10/Win11

using System;
using System.Runtime.InteropServices;

namespace Switchie
{

    public static class WindowsVirtualDesktopAPI
    {
        public static readonly Guid CLSID_IMMERSIVESHELL = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        public static readonly Guid CLSID_VIRTUALDESKTOPMANAGERINTERNAL = new Guid("C5E0CDCA-7B6E-41B2-9FC4-D93975CC467B");
        public static readonly Guid CLSID_VIRTUALDESKTOPMANAGER = new Guid("AA509086-5CA9-4C25-8F95-589D3C07B48A");
        public static readonly Guid CLSID_VIRTUALDESKTOPPINNEDAPPS = new Guid("B5A399E7-1C87-46B8-88E9-FC5747B171BD");

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum APPLICATION_VIEW_CLOAK_TYPE : int
        {
            AVCT_NONE = 0,
            AVCT_DEFAULT = 1,
            AVCT_VIRTUAL_DESKTOP = 2
        }

        public enum APPLICATION_VIEW_COMPATIBILITY_POLICY : int
        {
            AVCP_NONE = 0,
            AVCP_SMALL_SCREEN = 1,
            AVCP_TABLET_SMALL_SCREEN = 2,
            AVCP_VERY_SMALL_SCREEN = 3,
            AVCP_HIGH_SCALE_FACTOR = 4
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9")]
        public interface IObjectArray
        {
            void GetCount(out int count);
            void GetAt(int index, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object obj);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("4CE81583-1E4C-4632-A621-07A53543148F")]
        public interface IVirtualDesktopPinnedApps
        {
            bool IsAppIdPinned(string appId);
            void PinAppID(string appId);
            void UnpinAppID(string appId);
            bool IsViewPinned(IApplicationView applicationView);
            void PinView(IApplicationView applicationView);
            void UnpinView(IApplicationView applicationView);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
        public interface IServiceProvider10
        {
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object QueryService(ref Guid service, ref Guid riid);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
        [Guid("372E1D3B-38D3-42E4-A15B-8AB2B178F513")]
        public interface IApplicationView
        {
            int SetFocus();
            int SwitchTo();
            int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);
            int GetThumbnailWindow(out IntPtr hwnd);
            int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);
            int GetVisibility(out int visibility);
            int SetCloak(APPLICATION_VIEW_CLOAK_TYPE cloakType, int unknown);
            int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);
            int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);
            int InsertAfterWindow(IntPtr hwnd);
            int GetExtendedFramePosition(out RECT rect);
            int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);
            int SetAppUserModelId(string id);
            int IsEqualByAppUserModelId(string id, out int result);
            int GetViewState(out uint state);
            int SetViewState(uint state);
            int GetNeediness(out int neediness);
            int GetLastActivationTimestamp(out ulong timestamp);
            int SetLastActivationTimestamp(ulong timestamp);
            int GetVirtualDesktopId(out Guid guid);
            int SetVirtualDesktopId(ref Guid guid);
            int GetShowInSwitchers(out int flag);
            int SetShowInSwitchers(int flag);
            int GetScaleFactor(out int factor);
            int CanReceiveInput(out bool canReceiveInput);
            int GetCompatibilityPolicyType(out APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
            int SetCompatibilityPolicyType(APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
            int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out SIZE size1, out SIZE size2);
            int GetSizeConstraintsForDpi(uint uint1, out SIZE size1, out SIZE size2);
            int SetSizeConstraintsForDpi(ref uint uint1, ref SIZE size1, ref SIZE size2);
            int OnMinSizePreferencesUpdated(IntPtr hwnd);
            int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);
            int IsTray(out bool isTray);
            int IsInHighZOrderBand(out bool isInHighZOrderBand);
            int IsSplashScreenPresented(out bool isSplashScreenPresented);
            int Flash();
            int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);
            int EnumerateOwnershipTree(out IObjectArray ownershipTree);
            int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);
            int IsMirrored(out bool isMirrored);
            int Unknown1(out int unknown);
            int Unknown2(out int unknown);
            int Unknown3(out int unknown);
            int Unknown4(out int unknown);
            int Unknown5(out int unknown);
            int Unknown6(int unknown);
            int Unknown7();
            int Unknown8(out int unknown);
            int Unknown9(int unknown);
            int Unknown10(int unknownX, int unknownY);
            int Unknown11(int unknown);
            int Unknown12(out SIZE size1);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("1841C6D7-4F9D-42C0-AF41-8747538F10E5")]
        public interface IApplicationViewCollection
        {
            int GetViews(out IObjectArray array);
            int GetViewsByZOrder(out IObjectArray array);
            int GetViewsByAppUserModelId(string id, out IObjectArray array);
            int GetViewForHwnd(IntPtr hwnd, out IApplicationView view);
            int GetViewForApplication(object application, out IApplicationView view);
            int GetViewForAppUserModelId(string id, out IApplicationView view);
            int GetViewInFocus(out IntPtr view);
            int Unknown1(out IntPtr view);
            void RefreshCollection();
            int RegisterForApplicationViewChanges(object listener, out int cookie);
            int UnregisterForApplicationViewChanges(int cookie);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("F31574D6-B682-4CDC-BD56-1827860ABEC6")]
        public interface IVirtualDesktopManagerInternal
        {
            int GetCount();
            void MoveViewToDesktop(IApplicationView view, IVirtualDesktop desktop);
            bool CanViewMoveDesktops(IApplicationView view);
            IVirtualDesktop GetCurrentDesktop();
            void GetDesktops(out IObjectArray desktops);
            [PreserveSig]
            int GetAdjacentDesktop(IVirtualDesktop from, int direction, out IVirtualDesktop desktop);
            void SwitchDesktop(IVirtualDesktop desktop);
            IVirtualDesktop CreateDesktop();
            void RemoveDesktop(IVirtualDesktop desktop, IVirtualDesktop fallback);
            IVirtualDesktop FindDesktop(ref Guid desktopid);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("A5CD92FF-29BE-454C-8D04-D82879FB3F1B")]
        public interface IVirtualDesktopManager
        {
            bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);
            Guid GetWindowDesktopId(IntPtr topLevelWindow);
            void MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("FF72FFDD-BE7E-43FC-9C03-AD81681E88E4")]
        public interface IVirtualDesktop
        {
            bool IsViewVisible(IApplicationView view);
            Guid GetId();
        }
    }

    public class WindowsVirtualDesktop
    {
        [DllImport("user32.dll")] private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public WindowsVirtualDesktopAPI.IVirtualDesktop ivd;
        public WindowsVirtualDesktop(WindowsVirtualDesktopAPI.IVirtualDesktop desktop) { this.ivd = desktop; }
        public void MakeVisible() => WindowsVirtualDesktopManager.VirtualDesktopManagerInternal.SwitchDesktop(ivd);
        public static WindowsVirtualDesktop FromIndex(int index) => new WindowsVirtualDesktop(WindowsVirtualDesktopManager.GetDesktop(index));

        public static int Count
        {
            get => WindowsVirtualDesktopManager.VirtualDesktopManagerInternal.GetCount();
        }

        public static WindowsVirtualDesktop Current
        {
            get => new WindowsVirtualDesktop(WindowsVirtualDesktopManager.VirtualDesktopManagerInternal.GetCurrentDesktop());
        }

        public void MoveWindow(IntPtr hWnd)
        {
            int processId;
            if (hWnd == IntPtr.Zero) throw new ArgumentNullException();
            GetWindowThreadProcessId(hWnd, out processId);
            WindowsVirtualDesktopAPI.IApplicationView view;
            WindowsVirtualDesktopManager.ApplicationViewCollection.GetViewForHwnd(hWnd, out view);
            try
            {
                WindowsVirtualDesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, ivd);
            }
            catch
            {
                WindowsVirtualDesktopManager.ApplicationViewCollection.GetViewForHwnd(System.Diagnostics.Process.GetProcessById(processId).MainWindowHandle, out view);
                WindowsVirtualDesktopManager.VirtualDesktopManagerInternal.MoveViewToDesktop(view, ivd);
            }
        }
    }

    public static class WindowsVirtualDesktopManager
    {
        public static WindowsVirtualDesktopAPI.IVirtualDesktopPinnedApps VirtualDesktopPinnedApps;
        public static WindowsVirtualDesktopAPI.IVirtualDesktopManager VirtualDesktopManagerPrivate;
        public static WindowsVirtualDesktopAPI.IApplicationViewCollection ApplicationViewCollection;
        public static WindowsVirtualDesktopAPI.IVirtualDesktopManagerInternal VirtualDesktopManagerInternal;
        public static int FromDesktop(WindowsVirtualDesktop desktop) => desktop != null ? GetDesktopIndex(desktop.ivd) : -1;
        public static bool IsWindowPinned(IntPtr hWnd) => VirtualDesktopPinnedApps.IsViewPinned(hWnd.GetApplicationView());
        public static bool IsApplicationPinned(IntPtr hWnd) => VirtualDesktopPinnedApps.IsAppIdPinned(GetAppId(hWnd));

        private static WindowsVirtualDesktopAPI.IApplicationView GetApplicationView(this IntPtr hWnd)
        {
            ApplicationViewCollection.GetViewForHwnd(hWnd, out WindowsVirtualDesktopAPI.IApplicationView view);
            return view;
        }

        private static string GetAppId(IntPtr hWnd)
        {
            hWnd.GetApplicationView().GetAppUserModelId(out string appId);
            return appId;
        }

        static WindowsVirtualDesktopManager()
        {
            var shell = (WindowsVirtualDesktopAPI.IServiceProvider10)Activator.CreateInstance(Type.GetTypeFromCLSID(WindowsVirtualDesktopAPI.CLSID_IMMERSIVESHELL));
            VirtualDesktopManagerPrivate = (WindowsVirtualDesktopAPI.IVirtualDesktopManager)Activator.CreateInstance(Type.GetTypeFromCLSID(WindowsVirtualDesktopAPI.CLSID_VIRTUALDESKTOPMANAGER));
            ApplicationViewCollection = (WindowsVirtualDesktopAPI.IApplicationViewCollection)shell.QueryService(typeof(WindowsVirtualDesktopAPI.IApplicationViewCollection).GUID, typeof(WindowsVirtualDesktopAPI.IApplicationViewCollection).GUID);
            VirtualDesktopManagerInternal = (WindowsVirtualDesktopAPI.IVirtualDesktopManagerInternal)shell.QueryService(WindowsVirtualDesktopAPI.CLSID_VIRTUALDESKTOPMANAGERINTERNAL, typeof(WindowsVirtualDesktopAPI.IVirtualDesktopManagerInternal).GUID);
            VirtualDesktopPinnedApps = (WindowsVirtualDesktopAPI.IVirtualDesktopPinnedApps)shell.QueryService(WindowsVirtualDesktopAPI.CLSID_VIRTUALDESKTOPPINNEDAPPS, typeof(WindowsVirtualDesktopAPI.IVirtualDesktopPinnedApps).GUID);
        }

        public static WindowsVirtualDesktopAPI.IVirtualDesktop GetDesktop(int index)
        {
            int count = VirtualDesktopManagerInternal.GetCount();
            if (index < 0 || index >= count) throw new ArgumentOutOfRangeException("index");
            WindowsVirtualDesktopAPI.IObjectArray desktops;
            VirtualDesktopManagerInternal.GetDesktops(out desktops);
            object objdesktop;
            desktops.GetAt(index, typeof(WindowsVirtualDesktopAPI.IVirtualDesktop).GUID, out objdesktop);
            Marshal.ReleaseComObject(desktops);
            return (WindowsVirtualDesktopAPI.IVirtualDesktop)objdesktop;
        }

        public static int GetDesktopIndex(WindowsVirtualDesktopAPI.IVirtualDesktop desktop)
        {
            int index = -1;
            Guid IdSearch = desktop.GetId();
            VirtualDesktopManagerInternal.GetDesktops(out WindowsVirtualDesktopAPI.IObjectArray desktops);
            for (int i = 0; i < VirtualDesktopManagerInternal.GetCount(); i++)
            {
                desktops.GetAt(i, typeof(WindowsVirtualDesktopAPI.IVirtualDesktop).GUID, out object objdesktop);
                if (IdSearch.CompareTo(((WindowsVirtualDesktopAPI.IVirtualDesktop)objdesktop).GetId()) == 0)
                {
                    index = i;
                    break;
                }
            }
            Marshal.ReleaseComObject(desktops);
            return index;
        }

        public static WindowsVirtualDesktop FromWindow(IntPtr hWnd)
        {
            Guid id = VirtualDesktopManagerPrivate.GetWindowDesktopId(hWnd);
            if (id != Guid.Empty) return new WindowsVirtualDesktop(VirtualDesktopManagerInternal.FindDesktop(ref id));
            return null;
        }

        public static void PinApplication(IntPtr hWnd)
        {
            string appId = GetAppId(hWnd);
            if (!VirtualDesktopPinnedApps.IsAppIdPinned(appId))
                VirtualDesktopPinnedApps.PinAppID(appId);
        }
    }

}