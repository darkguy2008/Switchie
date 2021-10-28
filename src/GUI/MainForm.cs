using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switchie
{
    public class MainForm : Form
    {
        private Point dragOffset;
        private bool _forceAlwaysOnTop = false;
        private string _windowsHash = string.Empty;
        private List<VirtualDesktop> _virtualDesktops = new List<VirtualDesktop>();

        public int BorderSize { get; set; } = 1;
        public int PagerHeight { get; set; } = 40;
        public bool IsDraggingWindow { get; set; }
        public int VirtualDesktopSpacing { get; set; } = 4;
        public Color DesktopColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color WindowColor { get; set; } = Color.Gray;
        public Color WindowBorderColor { get; set; } = Color.Silver;
        public Color ActiveWindowColor { get; set; } = Color.Silver;
        public Color ActiveWindowBorderColor { get; set; } = Color.White;
        public Color ActiveDesktopBorderColor { get; set; } = Color.White;
        public ConcurrentBag<Window> Windows = new ConcurrentBag<Window>();

        public MainForm()
        {
            SuspendLayout();
            DoubleBuffered = true;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            ClientSize = new System.Drawing.Size(1, 1);
            ControlBox = false;
            AllowDrop = true;
            MinimumSize = new System.Drawing.Size(1, 1);
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmMain";
            TopMost = true;
            Icon = new System.Drawing.Icon(new MemoryStream(Helpers.GetResourceFromAssembly(typeof(Program), "Switchie.Resources.icon.ico")));
            Enumerable.Range(0, WindowsVirtualDesktop.GetInstance().Count).ToList().ForEach(x =>
            {
                VirtualDesktop desktop = new VirtualDesktop(x, this, new Point(_virtualDesktops.Sum(y => y.Size.Width), 0));
                MouseUp += desktop.OnMouseUp;
                MouseDown += desktop.OnMouseDown;
                MouseMove += desktop.OnMouseMove;
                DragOver += desktop.OnDragOver;
                DragDrop += desktop.OnDragDrop;
                _virtualDesktops.Add(desktop);
            });
            Size = new Size(_virtualDesktops.Sum(x => x.Size.Width), PagerHeight);
            MinimumSize = Size;
            MaximumSize = Size;
            ClientSize = Size;
            Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2) - (Size.Width / 2), Screen.PrimaryScreen.WorkingArea.Bottom - Size.Height);
            ResumeLayout(false);
            Shown += OnShown;
            MouseUp += OnMouseUp;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
        }

        private void OnShown(object sender, EventArgs e)
        {
            WindowsVirtualDesktopManager.GetInstance().PinApplication(Handle);
            new TaskFactory().StartNew(async () =>
            {
                while (!Program.ApplicationClosing.IsCancellationRequested)
                {
                    Invoke(new Action(() =>
                    {
                        try
                        {
                            if (_forceAlwaysOnTop)
                                WindowManager.SetAlwaysOnTop(Handle, _forceAlwaysOnTop);
                            Windows = new ConcurrentBag<Window>(WindowManager.GetOpenWindows());
                            var hash = $"{Windows.Sum(x => Math.Abs(x.Dimensions.X))}{Windows.Sum(x => Math.Abs(x.Dimensions.Y))}{Windows.Sum(x => x.Dimensions.Width)}{Windows.Sum(x => x.Dimensions.Height)}{string.Join("", Windows.Select(x => x.IsActive ? 1 : 0))}{string.Join("", Windows.Select(x => x.VirtualDesktopIndex))}";
                            if (hash != _windowsHash)
                            {
                                _windowsHash = hash;
                                Invalidate();
                            }
                        }
                        catch { }
                    }));
                    await Task.Delay(1);
                }
            });
            new TaskFactory().StartNew(async () =>
            {
                while (!Program.ApplicationClosing.IsCancellationRequested)
                {
                    Invoke(new Action(() =>
                    {
                        try
                        {
                            Windows = new ConcurrentBag<Window>(WindowManager.GetOpenWindows());
                            Invalidate();
                        }
                        catch { }
                    }));
                    await Task.Delay(100);
                }
            });
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            IsDraggingWindow = false;
            Cursor = Cursors.Default;
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                _forceAlwaysOnTop = false;
                ContextMenuStrip menu = new ContextMenuStrip();
                Helpers.AddMenuItem(this, menu, new ToolStripMenuItem() { Text = "About..." }, () => { MessageBox.Show("Made by darkguy2008", "About"); _forceAlwaysOnTop = true; });
                Helpers.AddMenuItem(this, menu, new ToolStripMenuItem() { Text = "Exit" }, () => { Environment.Exit(1); });
                menu.Opened += (ss, ee) => _forceAlwaysOnTop = false;
                menu.Show(this, PointToClient(Cursor.Position));
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) == MouseButtons.Middle)
            {
                IsDraggingWindow = true;
                dragOffset = e.Location;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDraggingWindow)
            {
                Cursor = Cursors.SizeAll;
                Location = new Point(e.X + Location.X - dragOffset.X, e.Y + Location.Y - dragOffset.Y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try { _virtualDesktops.ForEach(x => x.OnPaint(e)); }
            catch
            {
                WindowsVirtualDesktop.Restart();
                WindowsVirtualDesktopManager.Restart();
            }
        }
    }
}
