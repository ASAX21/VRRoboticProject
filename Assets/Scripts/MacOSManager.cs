using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

public class MacOSManager : OSManager
{
    public Process xWindowsServer;
    
    public MacOSManager()
    {
    }

    // Launch MAC Terminal
    public override void LaunchTerminal()
    {
        Process.Start(@"/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal");
    }


    public override void CompileProgram(string path)
    {

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