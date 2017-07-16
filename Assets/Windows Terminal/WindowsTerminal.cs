using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

public class WindowsTerminal : MonoBehaviour {

    // Compile a RoBIOS program using cygwin (32 bit)
    public void CompileRobiosProgram()//string filePath)
    {

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WorkingDirectory = @"C:\cygwin64\bin";
        //   startInfo.RedirectStandardInput = true;
        //startInfo.RedirectStandardOutput = true;
        //startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        //startInfo.CreateNoWindow = true;

        startInfo.FileName = "gcc.exe";
        startInfo.Arguments = @"C:\UNIVERSITY\EyeSim\Demos\examples\basic\hello.c -leyesim -lX11 -o C:\UNIVERSITY\EyeSim\Demos\examples\basic\test";
        // @"gcc C:\\UNIVERSITY\\EyeSim\\Demos\\examples\\basic -lX11 -leyesim -o C:\\UNIVERSITY\\EyeSim\\Demos\\examples\\basic\\test"
        //string progFilePath = @"C:\UNIVERSITY\EyeSim\Demos\examples\basic\hello.c";

        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();

    //    StreamWriter stdin = proc.StandardInput;

    //    stdin.WriteLine(@"gcc C:\\UNIVERSITY\\EyeSim\\Demos\\examples\\basic -leyesim -lX11 -o C:\\UNIVERSITY\\EyeSim\\Demos\\examples\\basic\\test");

        UnityEngine.Debug.Log("Done");



    }

}
