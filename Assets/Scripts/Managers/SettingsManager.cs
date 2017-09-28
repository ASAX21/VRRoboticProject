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

    public void TEST()
    {

        Debug.Log("currentDir: " + stringSettings["homedir"]("", false));
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

        //PlayerPrefs.SetFloat("mouseLook", (float) Math.Round(cameraController.mouseLookSens,5));
        //PlayerPrefs.SetFloat("keyLook", (float) Math.Round(cameraController.keyboardLookSens, 5));
        //PlayerPrefs.SetFloat("keyPan", (float) Math.Round(cameraController.keyboardPanSens, 5));
        //PlayerPrefs.SetFloat("zoom", (float) Math.Round(cameraController.zoomSens, 5));
        //PlayerPrefs.SetFloat("orthoPan", (float) Math.Round(cameraController.orthoPanSens, 5));
        //PlayerPrefs.SetFloat("orthoSens", (float) Math.Round(cameraController.orthoZoomSens, 5));
        //PlayerPrefs.SetString("homedir", homeDirectory);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        foreach (KeyValuePair<string, Func<float, bool, float>> entry in floatSettings)
            entry.Value(PlayerPrefs.GetFloat(entry.Key), true);

        foreach (KeyValuePair<string, Func<string, bool, string>> entry in stringSettings)
            entry.Value(PlayerPrefs.GetString(entry.Key), true);

        //cameraController.mouseLookSens = PlayerPrefs.GetFloat("mouseLook", cameraController.mouseLookSens);
        //cameraController.keyboardLookSens = PlayerPrefs.GetFloat("keyLook", cameraController.keyboardLookSens);
        //cameraController.keyboardPanSens = PlayerPrefs.GetFloat("keyPan", cameraController.keyboardPanSens);
        //cameraController.zoomSens = PlayerPrefs.GetFloat("zoom", cameraController.zoomSens);
        //cameraController.orthoPanSens = PlayerPrefs.GetFloat("orthoPan", cameraController.orthoPanSens);
        //cameraController.orthoZoomSens = PlayerPrefs.GetFloat("orthoSens", cameraController.orthoZoomSens);
        //homeDirectory = PlayerPrefs.GetString("homedir", Directory.GetCurrentDirectory());
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
