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

        public IntPtr worker { get; private set; }
        public IntPtr DCEx { get; private set; }

        public ShellWrapper()
        {
            SendMessage(0x052C);
            worker = GetShellWorker();
            DCEx = W32.GetDCEx(worker, IntPtr.Zero, (W32.DeviceContextValues)0x0403);

            if (worker == IntPtr.Zero)
                throw new Exception("Could not get workerw");

            if (DCEx == IntPtr.Zero)
                throw new Exception("Could not get DCEx");

            draw(new Action<Graphics>((d) => {
                d.FillRectangle(new SolidBrush(Color.Red), 0, 0, 500, 500);
            }));


        }

        public void draw(Action<Graphics> a)
        {
            if(DCEx != IntPtr.Zero)
            {
                using(Graphics g = Graphics.FromHdc(DCEx))
                {
                    a(g);
                }
                W32.ReleaseDC(worker, DCEx);
            }
        }
        
        public void drawForm(System.Windows.Forms.Form form)
        {
            W32.SetParent(form.Handle, worker);
        }

        public void drawWindow(System.Windows.Window window)
        {
            W32.SetParent(new WindowInteropHelper(window).Handle, worker);
        }

        public void clearGraphics()
        {

            // W32.RedrawWindow(worker, IntPtr.Zero, IntPtr.Zero, W32.RedrawWindowFlags.Invalidate);
            // W32.InvalidateRect(worker, IntPtr.Zero, true);

            var wpReg = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            var wallpaperPath = wpReg.GetValue("WallPaper").ToString();
            wpReg.Close();

            if (System.IO.File.Exists(wallpaperPath))
            {
                W32.SystemParametersInfo(W32.SPI_SETDESKWALLPAPER, 0, wallpaperPath, W32.SPIF_UPDATEINIFILE);
            }

        }

        private IntPtr GetShellWorker()
        {
            IntPtr worker = IntPtr.Zero;
            W32.EnumWindows(new W32.EnumWindowsProc((th, tph) => {

                IntPtr p = W32.FindWindowEx(th, IntPtr.Zero, "SHELLDLL_DefView", null);

                if(p != IntPtr.Zero)
                {
                    worker = W32.FindWindowEx(IntPtr.Zero, th, "WorkerW", null);
                }

                return true;

            }), IntPtr.Zero);

            return worker;
        }

        private static void SendMessage(uint address)
        {
            UIntPtr result;
            W32.SendMessageTimeout(W32.FindWindow("Progman", null), address, new UIntPtr(0), IntPtr.Zero, W32.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);
            Console.WriteLine(result.ToString());
        }

    }

}