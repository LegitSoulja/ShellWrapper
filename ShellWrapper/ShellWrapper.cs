using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Interop;

namespace ShellWrapper
{

    public class ShellWrapper 
    {

        private IntPtr worker;
        private IntPtr DCEx;
        private Version v;
        private bool ready = false;

        public ShellWrapper()
        {

            v = System.Environment.OSVersion.Version;

            if (v.Major == 6 && v.Minor == 1)
            {
                throw new Exception("Windows 7 is not supported. Yet...");
            }

            SendMessage(0x052C);

            worker = GetShellWorker();

            if (worker == IntPtr.Zero)
                throw new Exception("Could not find background worker");

            DCEx = NativeMethods.GetDCEx(worker, IntPtr.Zero, (NativeMethods.DeviceContextValues)0x0403);

            if (DCEx == IntPtr.Zero)
                throw new Exception("Could not get DCEx");

            ready = true;
        }

        public void draw(Action<Graphics> a)
        {
            if (!ready) return;
            if(DCEx != IntPtr.Zero)
            {
                using(Graphics g = Graphics.FromHdc(DCEx))
                {
                    a(g);
                }
                NativeMethods.ReleaseDC(worker, DCEx);
            }
        }
        
        public void drawForm(System.Windows.Forms.Form form)
        {
            if (!ready) return;
            NativeMethods.SetParent(form.Handle, worker);
            NativeMethods.ReleaseDC(worker, DCEx);
        }

        public void drawWindow(System.Windows.Window window)
        {
            if (!ready) return;
            NativeMethods.SetParent(new WindowInteropHelper(window).Handle, worker);
            NativeMethods.ReleaseDC(worker, DCEx);
        }

        public void drawHandle(IntPtr handle)
        {
            if (!ready) return;
            NativeMethods.SetParent(handle, worker);
            NativeMethods.ReleaseDC(worker, DCEx);
        }

        public void clearGraphics()
        {

            clearAllGraphics();

        }

        public void clearAllGraphics()
        {

            // NativeMethods.RedrawWindow(worker, IntPtr.Zero, IntPtr.Zero, NativeMethods.RedrawWindowFlags.Invalidate);
            // NativeMethods.InvalidateRect(worker, IntPtr.Zero, true);

            var wpReg = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            var wallpaperPath = wpReg.GetValue("WallPaper").ToString();
            wpReg.Close();

            if (System.IO.File.Exists(wallpaperPath))
            {
                NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETDESKWALLPAPER, 0, wallpaperPath, NativeMethods.SPIF_UPDATEINIFILE);
            }

        }

        private IntPtr GetShellWorker()
        {
            IntPtr worker = IntPtr.Zero;
            NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc((th, tph) => {

                if (v.Major == 6 && v.Minor == 1)
                {

                    // Windows 7
                    IntPtr p = NativeMethods.FindWindowEx(th, IntPtr.Zero, "SHELLDLL_DefView", null);

                    if (p != IntPtr.Zero)
                    {
                        worker = NativeMethods.FindWindowEx(IntPtr.Zero, th, "SysListView32", null);
                    }
                }
                else
                {
                    // Windows 8+
                    IntPtr p = NativeMethods.FindWindowEx(th, IntPtr.Zero, "SHELLDLL_DefView", null);

                    if (p != IntPtr.Zero)
                    {
                        worker = NativeMethods.FindWindowEx(IntPtr.Zero, th, "WorkerW", null);
                    }

                }

                return true;

            }), IntPtr.Zero);

            return worker;
        }

        private static void SendMessage(uint address)
        {
            UIntPtr result;
            NativeMethods.SendMessageTimeout(NativeMethods.FindWindow("Progman", null), address, new UIntPtr(0), IntPtr.Zero, NativeMethods.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);
        }

        ~ShellWrapper()
        {
            worker = IntPtr.Zero;
            DCEx = IntPtr.Zero;
            v = null;
            GC.SuppressFinalize(this);
        }

    }

}