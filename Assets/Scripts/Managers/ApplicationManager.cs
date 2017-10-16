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
            SimManager.instance.CreateNewBox(2000, 2000);
    }

    // Check for command line arguments (input sim file)
    private bool GetCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();

        // Check if exactly one extra argument is given (simFile)
        if (args.Length != 2)
            return false;

        string simPath = args[1];

        if (Path.GetExtension(simPath) != ".sim" || !File.Exists(simPath))
            return false;
        else
        {
            EyesimLogger.instance.Log("Sim file provided from command line");
            SimReader.instance.ReceiveFile(simPath);
        }
        return true;
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
