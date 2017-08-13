/* This class provides support for the Windows enviroment, automates
 * much of the required setup necessary for a Linux enviroment.
 * 
 * XMing is used as the xWindows server (LCD Display), and cygwin
 * for RoBIOS program compilation and execution 
 */

using System;
using System.IO;
using System.Diagnostics;

public class WindowsOSManager
{
    public Process xWindowsServer;

    // Launch X Windows server (XMing) on start
    public WindowsOSManager()
    {
        LaunchXMing();
    }

    private void LaunchXMing()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;
        startInfo.FileName = @"Xming\Xming.exe";
        startInfo.Arguments = @"-screen 0 -multiwindow";
        xWindowsServer = new Process();
        xWindowsServer.StartInfo = startInfo;
        xWindowsServer.Start();
    }

    // Compile a RoBIOS program using cygwin (32 bit)
    public void LaunchCygwinTerminal()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WorkingDirectory = @"cygwin";
        startInfo.FileName = "Cygwin.bat";

        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();
    }

    public void Terminate()
    {
        xWindowsServer.Close();
    }
}
