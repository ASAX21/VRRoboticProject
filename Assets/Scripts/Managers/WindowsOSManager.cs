/* This class provides support for the Windows enviroment, automates
 * much of the required setup necessary for a Linux enviroment.
 * 
 * XMing is used as the xWindows server (LCD Display), and cygwin
 * for RoBIOS program compilation and execution 
 */

using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

public abstract class OSManager : IFileReceiver
{
    public abstract GameObject ReceiveFile(string filepath);
    public abstract void Terminate();
    public abstract void LaunchTerminal();
    public abstract void CompileProgram(string path);
}

public class WindowsOSManager: OSManager
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

    // Launch cygwin terminal
    public override void LaunchTerminal()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WorkingDirectory = @"cygwin";
        startInfo.FileName = "Cygwin.bat";

        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();
    }

    public override void CompileProgram(string path)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WorkingDirectory = @"cygwin\bin";
        startInfo.FileName = "gcc.exe";
        startInfo.Arguments = path + @" -I../usr/local/include -leyesim -lX11 -o " + @Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path);

        UnityEngine.Debug.Log("Trying to compile");
        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();
    }

    // Close the XMing instance
    public override void Terminate()
    {
        xWindowsServer.CloseMainWindow();
        xWindowsServer.Close();
    }

    public override GameObject ReceiveFile(string filepath)
    {
        UnityEngine.Debug.Log("HELLO!");
        UnityEngine.Debug.Log(Path.GetDirectoryName(filepath));
        UnityEngine.Debug.Log(Path.GetFileNameWithoutExtension(filepath));

        UnityEngine.Debug.Log(filepath);
        CompileProgram(filepath);
        return null;
    }
}
