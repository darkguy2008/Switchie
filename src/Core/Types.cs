using System;
using System.Drawing;

namespace Switchie
{

    public class Window
    {
        public bool IsActive { get; set; }
        public int VirtualDesktopIndex { get; set; }
        public int ZOrder { get; set; }
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public uint ProcessID { get; set; }
        public Rectangle Dimensions { get; set; }
        public Bitmap Icon { get; set; }
    }

    public class DragDropData
    {
        public int OriginDesktopIndex { get; set; }
        public Window DraggedWindow { get; set; }
    }

}