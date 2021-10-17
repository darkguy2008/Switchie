using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Switchie
{

    public class Helpers
    {
        public static byte[] GetResourceFromAssembly(Type type, string name)
        {
            MemoryStream ms = new MemoryStream();
            Assembly.GetAssembly(type).GetManifestResourceStream(name).CopyTo(ms);
            return ms.ToArray();
        }

        public static void AddMenuItem(Form main, ContextMenuStrip menu, ToolStripMenuItem m, Action onClick = null)
        {
            m.Click += (s, e) => onClick?.Invoke();
            menu.Items.Add(m);
        }

        public static Size AspectRatioResize(Size sz, int finalWidth, int finalHeight)
        {
            int iWidth;
            int iHeight;
            if ((finalHeight == 0) && (finalWidth != 0))
            {
                iWidth = finalWidth;
                iHeight = (sz.Height * iWidth / sz.Width);
            }
            else if ((finalHeight != 0) && (finalWidth == 0))
            {
                iHeight = finalHeight;
                iWidth = (sz.Width * iHeight / sz.Height);
            }
            else
            {
                iWidth = finalWidth;
                iHeight = finalHeight;
            }
            return new Size(iWidth, iHeight);
        }
    }

}