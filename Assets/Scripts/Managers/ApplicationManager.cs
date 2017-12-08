using System;
using System.IO;
using UnityEngine;

public class ApplicationManager : MonoBehaviour {

    public OSManager osManager;

    private void Awake()
    {
        // Do platform specific things in Awake - Ready for other scripts in Start
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            osManager = new WindowsOSManager();
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            osManager = new MacOSManager();
        }
    }

    private void Start()
    {
        if (!GetCommandLineArguments())
            SimManager.instance.CreateInitialWorld();
    }

    // Check for command line arguments (input sim file)
    private bool GetCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();

        // Check if exactly two extra arguments are given (simFile and directory)
        if (args.Length != 3)
            return false;

        string simPath = args[1];

        if (Path.GetExtension(simPath) != ".sim")
            simPath += ".sim";

        EyesimLogger.instance.Log(simPath);
        EyesimLogger.instance.Log(args[2]);

        simPath = IO.FindFileFromDirectory(args[1], new string[]{ args[2], SettingsManager.instance.GetSetting("simdir", "")});
        if (simPath == "")
        {
            EyesimLogger.instance.Log("Unable to find sim file " + args[1]);
            return false;
        }
        else
        {
            EyesimLogger.instance.Log("Sim file provided from command line " + simPath);
            SimReader.instance.ReceiveFile(simPath);
            return true;
        }
    }

    public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

    private void OnApplicationQuit()
    {
        if (osManager != null)
            osManager.Terminate();
    }

    public void LaunchTerminal()
    {
        if (osManager != null)
            osManager.LaunchTerminal();
    }
}
