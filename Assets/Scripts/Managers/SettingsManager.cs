using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.iOS.Xcode;

public class SettingsManager : MonoBehaviour {

    static public SettingsManager instance;

    public CameraControl cameraController;

    public string homeDirectory;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        homeDirectory = Directory.GetCurrentDirectory();
    }

    private void Start()
    {
        LoadSettings();
    }

    public void SaveSettings()
    {
        INIParser ini = new INIParser();
        // Windows: Write INI file to /Users/username/AppData/Roaming/eyesim/config.ini
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // Get Directory
            string appDir = Environment.GetEnvironmentVariable("APPDATA");
            appDir = Path.Combine(appDir, "eyesim");
            if (!Directory.Exists(appDir))
                Directory.CreateDirectory(appDir);
            appDir = Path.Combine(appDir, "config.ini");
            // Open INI file
            ini.Open(appDir);
			// Camera
			ini.WriteValue("Camera", "mouseLook", Math.Round(cameraController.mouseLookSens,5));
			ini.WriteValue("Camera", "keyLook", Math.Round(cameraController.keyboardLookSens, 5));
			ini.WriteValue("Camera", "keyPan", Math.Round(cameraController.keyboardPanSens, 5));
			ini.WriteValue("Camera", "zoom", Math.Round(cameraController.zoomSens, 5));
			ini.WriteValue("Camera", "orthoPan", Math.Round(cameraController.orthoPanSens, 5));
			ini.WriteValue("Camera", "orthoSens", Math.Round(cameraController.orthoZoomSens, 5));

			// Directory
			ini.WriteValue("Directory", "home", homeDirectory);

			// Close config file (Completes Write)
			ini.Close();
        }

        // TODO: Mac
        else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
			string plistPath = @"test.plist";
			PlistDocument plist = new PlistDocument ();
			if(!File.Exists(plistPath))
				//plist.Create(plistPath);
				Debug.Log("nofile");

			PlistElementDict dict = plist.root;

			// Camera
			dict.SetString("mouseLook", Math.Round(cameraController.mouseLookSens,5).ToString());
			dict.SetString("keyLook", Math.Round(cameraController.keyboardLookSens, 5).ToString());
			dict.SetString("keyPan", Math.Round(cameraController.keyboardPanSens, 5).ToString());
			dict.SetString("zoom", Math.Round(cameraController.zoomSens, 5).ToString());
			dict.SetString("orthoPan", Math.Round(cameraController.orthoPanSens, 5).ToString());
			dict.SetString("orthoSens", Math.Round(cameraController.orthoZoomSens, 5).ToString());

			// Directory
			dict.SetString("home", homeDirectory);
			plist.WriteToFile (plistPath);
            return;
        }
    }

    public void LoadSettings()
    {
        INIParser ini = new INIParser();
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string appDir = Environment.GetEnvironmentVariable("APPDATA");
            appDir = Path.Combine(appDir, "eyesim" + Path.DirectorySeparatorChar + "config.ini");
            if (!File.Exists(appDir))
            {
                Debug.Log(appDir);
                Debug.Log("Could not find config file!");
                return;
            }
            ini.Open(appDir);
        }

        // Camera settings
        cameraController.mouseLookSens = (float) ini.ReadValue("Camera", "mouseLook", 5f);
        cameraController.keyboardLookSens = (float)ini.ReadValue("Camera", "keyLook", 5f);
        cameraController.keyboardPanSens = (float)ini.ReadValue("Camera", "keyPan", 5f);
        cameraController.zoomSens = (float)ini.ReadValue("Camera", "zoom", 5f);
        cameraController.orthoPanSens = (float)ini.ReadValue("Camera", "orthoPan", 5f);
        cameraController.orthoZoomSens = (float)ini.ReadValue("Camera", "orthoSens", 5f);

        // Directory settings
        homeDirectory = ini.ReadValue("Directory", "home", Directory.GetCurrentDirectory());

        ini.Close();
    }

    // ----- Camera Settings -----
    public void SetCamMouseLookSens(float sens)
    {
        cameraController.mouseLookSens = sens;
    }

    public void SetCamZoomSens(float sens)
    {
        cameraController.zoomSens = sens;
    }

    public void SetCamKeyLookSens(float sens)
    {
        cameraController.keyboardLookSens = sens;
    }

    public void SetCamPanSens(float sens)
    {
        cameraController.keyboardPanSens = sens;
    }

    public void SetOrthoPanSens(float sens)
    {
        cameraController.orthoPanSens = sens;
    }

    public void SetOrthoZoomSens(float sens)
    {
        cameraController.orthoZoomSens = sens;
    }

    // ----- Directory Settings -----
    public void SetHomeDirectory(string dirPath)
    {
        if (Directory.Exists(dirPath))
            homeDirectory = dirPath;
        else
            Debug.Log("Bad directory given to Set Home Directory");
    }
}
