using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Switchie
{

    public class VirtualDesktopScreen
    {
        public Size Size { get; set; }
        public MainForm Form { get; set; }
        public Point Location { get; set; }
        public Screen AttachedScreen { get; set; }
        public VirtualDesktop VirtualDesktop { get; set; }
        public Dictionary<IntPtr, Rectangle> WindowAreas { get; set; } = new Dictionary<IntPtr, Rectangle>();

        private Point MousePosition { get => Control.MousePosition; }
        private Window ActiveWindow { get => Form.Windows.SingleOrDefault(x => x.IsActive); }

        public void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            var windows = Form.Windows.Where(x => x.VirtualDesktopIndex == VirtualDesktop.VirtualDesktopIndex).OrderBy(x => x.ZOrder).ToArray();

            foreach (var w in windows)
            {
                if (Screen.FromHandle(w.Handle).DeviceName == AttachedScreen.DeviceName)
                {
                    Color fillColor = w.IsActive ? Form.ActiveWindowColor : Form.WindowColor;
                    Color borderColor = w.IsActive ? Form.ActiveWindowBorderColor : Form.WindowBorderColor;

                    var x = w.Dimensions.X;
                    var y = w.Dimensions.Y;
                    x -= AttachedScreen.Bounds.Left;
                    y -= AttachedScreen.Bounds.Top;
                    var area = new Rectangle(x, y, w.Dimensions.Width - Form.BorderSize, w.Dimensions.Height - Form.BorderSize);

                    // Scale rectangles down to the thumbnail's desired size
                    var ar = Helpers.AspectRatioResize(new Size(AttachedScreen.Bounds.Width, AttachedScreen.Bounds.Height), 0, Form.PagerHeight);
                    float percentageWidth = (float)ar.Width * 100 / AttachedScreen.Bounds.Width;
                    float percentageHeight = (float)ar.Height * 100 / AttachedScreen.Bounds.Height;

                    area.X = (int)(area.X * (percentageWidth / 100));
                    area.Y = (int)(area.Y * (percentageHeight / 100));
                    area.Width = (int)(area.Width * (percentageWidth / 100));
                    area.Height = (int)(area.Height * (percentageWidth / 100));

                    area.X += Location.X;
                    area.Y += Location.Y;
                    WindowAreas[w.Handle] = area;

                    // Window rectangle
                    g.FillRectangle(new SolidBrush(fillColor), new Rectangle(area.X, area.Y, area.Width - (Form.BorderSize), area.Height - (Form.BorderSize)));

                    // Window icon
                    var oldBounds = e.Graphics.ClipBounds;
                    e.Graphics.Clip = new Region(area);
                    g.DrawImage(w.Icon, new Point(
                        (area.X + area.Width / 2) - w.Icon.Width / 2,
                        (area.Y + area.Height / 2) - w.Icon.Height / 2
                    ));
                    e.Graphics.Clip = new Region(oldBounds);

                    // Window border
                    g.DrawRectangle(new Pen(new SolidBrush(borderColor), Form.BorderSize), new Rectangle(area.X, area.Y, area.Width - (Form.BorderSize), area.Height - (Form.BorderSize)));
                }
                else
                    continue;
            }
        }
    }

}