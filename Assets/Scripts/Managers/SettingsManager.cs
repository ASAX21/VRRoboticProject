using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Responsible for saving/loading preferences between sessions
// A central point where settings can be modified

public class SettingsManager : MonoBehaviour {

    static public SettingsManager instance;

    public CameraControl cameraController;

    public Dictionary<string, Func<float, bool, float>> floatSettings;
    public Dictionary<string, Func<string, bool, string>> stringSettings;

    public string homeDirectory;
    public string worldDirectory;
    public string simDirectory;
    public string defaultSim;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        homeDirectory = Directory.GetCurrentDirectory();
        worldDirectory = Directory.GetCurrentDirectory();
        simDirectory = Directory.GetCurrentDirectory();
        defaultSim = "";
    }

    private void Start()
    {
        floatSettings = new Dictionary<string, Func<float, bool, float>>();
        stringSettings = new Dictionary<string, Func<string, bool, string>>();
        ConfigureSettings();
        LoadSettings();
    }

    // Build the settings dictionary
    // Lamba: x is a string identifying, y is a boolean (True for set value, false for get value)
    private void ConfigureSettings()
    {
        floatSettings.Add("mouseLook", (x, y) => y ? cameraController.mouseLookSens = x : cameraController.mouseLookSens);
        floatSettings.Add("keyLook", (x, y) => y ? cameraController.keyboardLookSens = x : cameraController.keyboardLookSens);
        floatSettings.Add("keyPan", (x, y) => y ? cameraController.keyboardPanSens = x : cameraController.keyboardPanSens);
        floatSettings.Add("zoom", (x, y) => y ? cameraController.zoomSens = x : cameraController.zoomSens);
        floatSettings.Add("orthoPan", (x, y) => y ? cameraController.orthoPanSens = x : cameraController.orthoPanSens);
        floatSettings.Add("orthoZoom", (x, y) => y ? cameraController.orthoZoomSens = x : cameraController.orthoZoomSens);
        floatSettings.Add("psdMeanError", (x, y) => y ? PSDController.globalMean = x : PSDController.globalMean);
        floatSettings.Add("psdStdDevError", (x, y) => y ? PSDController.globalStdDev = x : PSDController.globalStdDev);

        stringSettings.Add("homedir", (x, y) => y ? homeDirectory = x : homeDirectory);
        stringSettings.Add("worlddir", (x, y) => y ? worldDirectory = x : worldDirectory);
        stringSettings.Add("simdir", (x, y) => y ? simDirectory = x : simDirectory);
        stringSettings.Add("defaultsim", (x, y) => y ? defaultSim = x : defaultSim);
    }

    public void SaveSettings()
    {
        foreach(KeyValuePair<string, Func<float, bool, float>> entry in floatSettings)
            PlayerPrefs.SetFloat(entry.Key, entry.Value(0, false));

        foreach (KeyValuePair<string, Func<string, bool, string>> entry in stringSettings)
            PlayerPrefs.SetString(entry.Key, entry.Value("", false));

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        foreach (KeyValuePair<string, Func<float, bool, float>> entry in floatSettings)
            entry.Value(PlayerPrefs.GetFloat(entry.Key, entry.Value(0, false)), true);

        foreach (KeyValuePair<string, Func<string, bool, string>> entry in stringSettings)
            entry.Value(PlayerPrefs.GetString(entry.Key, entry.Value("", false)), true);
    }

    // ----- Settings -----
    // Floats
    public void ChangeSettingsValue(string setting, float val)
    {
        if (!floatSettings.ContainsKey(setting))
        {
            Debug.Log("Set settings: " + setting + " - No such entry");
            return;
        }

        floatSettings[setting](val, true);
    }
    public float GetSetting(string setting, float defaultValue)
    {
        if(!floatSettings.ContainsKey(setting))
        {
            Debug.Log("Get settings: " + setting + " - No such entry");
            return defaultValue;
        }
        return floatSettings[setting](0, false);
    }

    // Strings
    public void ChangeSettingsValue(string setting, string val)
    {
        if (!stringSettings.ContainsKey(setting))
        {
            Debug.Log("Settings: " + setting + " - No such entry");
            return;
        }
        stringSettings[setting](val, true);
    }

    public string GetSetting(string setting, string defaultValue)
    {
        if (!stringSettings.ContainsKey(setting))
        {
            Debug.Log("Get settings: " + setting + " - No such entry");
            return defaultValue;
        }
        return stringSettings[setting]("", false);
    }
}
