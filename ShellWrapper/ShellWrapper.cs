using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using Microsoft.Win32;

namespace ShellWrapper
{

    public static class ShellWrapper
    {

        public static void Debug(string[] args)
        {

            SendMessage(0x052C); // activate workerw process on shell
            IntPtr workerw = GetShellWorker();
            Console.WriteLine(workerw.ToString());
            Console.WriteLine("Drawing will begin after a key press");
            Console.ReadKey();
            drawTest(workerw);
            Console.WriteLine("Did it work? :). Press enter to exit now.");
            Console.ReadLine();

            clearGraphics(workerw);

            Console.ReadKey();
            Environment.Exit(0);

        }

        private static void drawTest(IntPtr worker)
        {
            IntPtr dc = W32.GetDCEx(worker, IntPtr.Zero, (W32.DeviceContextValues)0x403);

            if (dc != IntPtr.Zero)
            {
                using(Graphics g = Graphics.FromHdc(dc))
                {
                    g.FillRectangle(new SolidBrush(Color.Red), 0, 0, 500, 500);
                }

                W32.ReleaseDC(worker, dc);

            }
        }


        public static void clearGraphics(IntPtr worker)
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

        public static IntPtr GetShellWorker()
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

        public static void SendMessage(uint address)
        {
            UIntPtr result;
            W32.SendMessageTimeout(W32.FindWindow("Progman", null), address, new UIntPtr(0), IntPtr.Zero, W32.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);
            Console.WriteLine(result.ToString());
        }

    }

}