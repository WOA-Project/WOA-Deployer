using System;
using System.Runtime.InteropServices;

namespace Deployer.Gui.Common
{
    public static class ConsoleEmbedder
    {
        private const uint WmChar = 0x0102;
        private const int VkEnter = 0x0D;

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        private const int AttachParentProcess = -1;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


        public static void ExecuteInsideConsole(Action consoleAction)
        {
            AttachConsole(AttachParentProcess);
            IntPtr cw = GetConsoleWindow();

            System.Console.WriteLine();

            consoleAction();
            SendMessage(cw, WmChar, (IntPtr) VkEnter, IntPtr.Zero);
            FreeConsole();
        }
    }
}