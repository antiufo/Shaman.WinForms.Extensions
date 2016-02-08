using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace Shaman.WinForms
{
    public static class WindowsFormsExtensions
    {
        public static void InitWindowLocation(string serializedWindowPosition, Form form)
        {
            LoadWindowPosition(serializedWindowPosition, form);
            EnsureVisible(form, false);
        }


        public static void EnsureVisible(Form form, bool restartFromTopLeft)
        {
            if (!Screen.AllScreens.Any(s => s.WorkingArea.Contains(new Rectangle(form.Location, form.Size))))
            {
                if (restartFromTopLeft)
                {
                    Point desktopLocation = new Point(10, 10);
                    form.DesktopLocation = desktopLocation;
                }
                else
                {
                    CenterToScreen(form);
                }
            }
        }


        private static void CenterToScreen(Form form)
        {
            Point location = default(Point);
            Screen screen = Screen.FromControl(form);

            Rectangle workingArea = screen.WorkingArea;
            location.X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - form.Width) / 2);
            location.Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - form.Height) / 2);
            form.Location = location;
        }

        public static void CopyWindowPosition(Form modelForm, Form form)
        {
            if (modelForm.WindowState == FormWindowState.Normal && !IsSnapped(modelForm))
            {
                form.StartPosition = FormStartPosition.Manual;
                Point pos = modelForm.Location;
                pos.X += 20;
                pos.Y += 20;
                form.Location = pos;
                form.Size = modelForm.Size;
                EnsureVisible(form, true);
                form.WindowState = FormWindowState.Normal;
            }
            else if (modelForm.WindowState == FormWindowState.Maximized)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Normal;
            }
        }


        private static bool IsSnapped(Form form)
        {
            if (form.WindowState != FormWindowState.Normal) return false;
            if (form.DesktopBounds.Top != 0) return false;

            return (form.DesktopBounds.Left == 0 ||
                    form.DesktopBounds.Right == Screen.GetWorkingArea(form).Right);
        }



        private static void LoadWindowPosition(string serializedWindowPosition, Form form)
        {
            if (string.IsNullOrEmpty(serializedWindowPosition))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                return;
            }

            string[] array = serializedWindowPosition.Split(' ');
            FormWindowState windowState = (FormWindowState)Convert.ToInt32(array[0]);
            if (windowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
            else if (windowState == FormWindowState.Normal)
            {
                form.StartPosition = FormStartPosition.Manual;
                Point location = new Point(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
                form.Location = location;
                Size size = new Size(Convert.ToInt32(array[3]), Convert.ToInt32(array[4]));
                form.Size = size;
                form.WindowState = FormWindowState.Normal;
            }
            else
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.WindowState = FormWindowState.Normal;
            }

        }


        public static string SerializePosition(Form form)
        {
            if (IsSnapped(form)) return string.Empty;

            return string.Join(" ", new[]{
    
                Convert.ToString((int)form.WindowState),
                Convert.ToString(form.Left),
                Convert.ToString(form.Top),
                Convert.ToString(form.Width),
                Convert.ToString(form.Height)
            });


        }


        public static void ShowAroundVisualOwner(this Form form, Form visualOwner)
        {
            if (!form.IsHandleCreated)
                form.CenterAroundVisualOwner(visualOwner);
            form.Show();
            form.Activate();
        }


        public static void CenterAroundVisualOwner(this Form form, Form visualOwner)
        {
            if (visualOwner != null && visualOwner.WindowState != FormWindowState.Minimized)
            {
                form.Left = visualOwner.Left + (visualOwner.Width - form.Width) / 2;
                form.Top = visualOwner.Top + (visualOwner.Height - form.Height) / 2;
                form.StartPosition = FormStartPosition.Manual;
                EnsureVisible(form, false);
            }
        }

        public static bool IsClosed(this Form form)
        {
            return form.Disposing || form.IsDisposed;
        }

        public static bool IsClosedOrNull(this Form form)
        {
            if (form == null) return true;
            return IsClosed(form);
        }

        public static bool IsClosedOrNullOrHidden(this Form form)
        {
            return IsClosedOrNull(form) || !form.Visible;
        }

        public static Size GetClientSize(this Form control)
        {
            var c = control.ClientSize;
            if (c.Width == 0 || c.Height == 0)
            {
                var size = control.Size;
                return new Size(Math.Max(0, size.Width - 16), Math.Max(0, size.Height - 39));
            }
            return c;
        }
    }
}
