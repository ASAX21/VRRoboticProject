using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour {

    static public SettingsManager instance;

    public CameraControl cameraController;

    public Dictionary<string, Func<float, bool, float>> floatSettings;
    public Dictionary<string, Func<string, bool, string>> stringSettings;

    public string homeDirectory;
    public float test = 3;

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
        floatSettings = new Dictionary<string, Func<float, bool, float>>();
        stringSettings = new Dictionary<string, Func<string, bool, string>>();
        ConfigureSettings();
        LoadSettings();
    }

    // Build the settings dictionary
    private void ConfigureSettings()
    {
        floatSettings.Add("mouseLook", (x, y) => y ? cameraController.mouseLookSens = x : cameraController.mouseLookSens);
        floatSettings.Add("keyLook", (x, y) => y ? cameraController.keyboardLookSens = x : cameraController.keyboardLookSens);
        floatSettings.Add("keyPan", (x, y) => y ? cameraController.keyboardPanSens = x : cameraController.keyboardPanSens);
        floatSettings.Add("zoom", (x, y) => y ? cameraController.zoomSens = x : cameraController.zoomSens);
        floatSettings.Add("orthoPan", (x, y) => y ? cameraController.orthoPanSens = x : cameraController.orthoPanSens);
        floatSettings.Add("orthoSens", (x, y) => y ? cameraController.orthoZoomSens = x : cameraController.orthoZoomSens);

        stringSettings.Add("homedir", (x, y) => y ? homeDirectory = x : homeDirectory);
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
