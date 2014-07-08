using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

class SendMessage
{
    [DllImport("User32.dll")]
    private static extern int RegisterWindowMessage(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessageA(IntPtr hwnd, int wMsg, int wParam, uint lParam); 

    [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
    public static extern bool SetForegroundWindow(int hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPTStr)] string lpClassName, [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName); 

    public int sendWindowsMessage(IntPtr hWnd, int Msg, int wParam, uint lParam)
    {
        int result = 0;

        if (hWnd.ToInt32() > 0)
        {
            result = SendMessageA(hWnd, Msg, wParam, lParam);
        }

        return result;
    }
}

