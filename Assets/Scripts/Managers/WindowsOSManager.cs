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

    // Compile a RoBIOS program using cygwin (32 bit)
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
        UnityEngine.Debug.Log(Path.GetDirectoryName(filepath));
        UnityEngine.Debug.Log(Path.GetFileNameWithoutExtension(filepath));

        UnityEngine.Debug.Log(filepath);
        CompileProgram(filepath);
        return null;
    }
}

public class MacOSManager: OSManager
{	

    // Launch XQuarts when starting on Mac
    public MacOSManager()
    {
        Process[] pname = Process.GetProcessesByName("XQuartz");
        if(pname.Length == 0)
        {
            UnityEngine.Debug.Log("No XQuartz");
            EyesimLogger.instance.Log("Launching XQuartz");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.FileName = @"/Applications/Utilities/XQuartz.app/Contents/MacOS/X11";
            Process.Start(startInfo);
        }
        else
        {
            UnityEngine.Debug.Log("Xquarts!");
        }
    }
	public override void LaunchTerminal()
    {
        System.Diagnostics.Process.Start(@"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal");
    }

    public override void CompileProgram(string path)
    {
        return;
    }

    public override void Terminate()
    {
        return;
    }

    public override GameObject ReceiveFile(string filepath)
    {
        return null;
    }
}