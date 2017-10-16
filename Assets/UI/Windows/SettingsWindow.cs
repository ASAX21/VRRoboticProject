using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum SetDirectory { Home, World, Sim, Object, Robot, Control};

public class SettingsWindow : TabWindow, IFileReceiver
{
    [Header("Camera")]
    public Slider mouseLookSensSlider;
    public InputField mouseLookSensInput;

    public Slider keyLookSensSlider;
    public InputField keyLookSensInput;

    public Slider keyPanSensSlider;
    public InputField keyPanSensInput;

    public Slider zoomSensSlider;
    public InputField zoomSensInput;

    public Slider orthoPanSlider;
    public InputField orthoPanInput;

    public Slider orthoZoomSlider;
    public InputField orthoZoomInput;

    [Header("Directories")]
    public FileFinder dirFinder;

    public InputField homeDirInput;
    public InputField worldDirInput;
    public InputField simDirInput;

    [Header("Error")]
    public GaussianDrawer gaussDrawer;

    private SetDirectory dirSet;

    void OnEnable ()
    {
        windowTitle.color = UIManager.instance.windowHeaderTextColor;
        windowHeader.color = UIManager.instance.windowHeaderColor;
        dirFinder.Initialise("*", FileBrowserType.Directory, this);
        UpdateAllValues();
    }

    private void OnDisable()
    {
        SettingsManager.instance.SaveSettings();
    }

    // Updates the values in the sliders/input fields
    public void UpdateAllValues()
    {
        // Set all initial camera values
        mouseLookSensSlider.value = SettingsManager.instance.cameraController.mouseLookSens;
        mouseLookSensInput.text = SettingsManager.instance.cameraController.mouseLookSens.ToString();

        keyLookSensInput.text = SettingsManager.instance.cameraController.keyboardLookSens.ToString();
        keyLookSensSlider.value = SettingsManager.instance.cameraController.keyboardLookSens;

        keyPanSensInput.text = SettingsManager.instance.cameraController.keyboardPanSens.ToString();
        keyPanSensSlider.value = SettingsManager.instance.cameraController.keyboardPanSens;

        zoomSensInput.text = SettingsManager.instance.cameraController.zoomSens.ToString();
        zoomSensSlider.value = SettingsManager.instance.cameraController.zoomSens;

        orthoPanInput.text = SettingsManager.instance.cameraController.orthoPanSens.ToString();
        orthoPanSlider.value = SettingsManager.instance.cameraController.orthoPanSens;

        orthoZoomInput.text = SettingsManager.instance.cameraController.orthoZoomSens.ToString();
        orthoZoomSlider.value = SettingsManager.instance.cameraController.orthoZoomSens;

        // Set home dir
        homeDirInput.text = SettingsManager.instance.homeDirectory;
        worldDirInput.text = SettingsManager.instance.worldDirectory;
        simDirInput.text = SettingsManager.instance.simDirectory;
    }
	
    // ----- Camera Settings -----
    // PERSPECTIVE
    // Mouse Look
    public void SetMouseLookSensSlider(float val)
    {
        float sens = Mathf.Clamp(val, 1f, 20f);
        SettingsManager.instance.ChangeSettingsValue("mouseLook", sens);
        mouseLookSensInput.text = sens.ToString();
    }

    public void SetMouseLookSensInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 1f, 20f);
        SettingsManager.instance.ChangeSettingsValue("mouseLook", sens);
        mouseLookSensSlider.value = sens;
    }
    // Key Look
    public void SetKeyLookSensSlider(float val)
    {
        float sens = Mathf.Clamp(val, 1f, 100f);
        SettingsManager.instance.ChangeSettingsValue("keyLook", sens);
        keyLookSensInput.text = sens.ToString();
    }

    public void SetKeyLookSensInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 1f, 100f);
        SettingsManager.instance.ChangeSettingsValue("keyLook", sens);
        keyLookSensSlider.value = sens;
    }
    // Key Pan
    public void SetKeyPanSensSlider(float val)
    {
        float sens = Mathf.Clamp(val, 0f, 10f);
        SettingsManager.instance.ChangeSettingsValue("keyPan", sens);
        keyPanSensInput.text = sens.ToString();
    }

    public void SetKeyPanSensInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 0f, 10f);
        SettingsManager.instance.ChangeSettingsValue("keyPan", sens);
        keyPanSensSlider.value = sens;
    }
    // Zoom
    public void SetZoomSensSlider(float val)
    {
        float sens = Mathf.Clamp(val, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("zoom", sens);
        zoomSensInput.text = sens.ToString();
    }

    public void SetZoomSensInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("zoom", sens);
        zoomSensSlider.value = sens;
    }

    // ORTHOGRAPHIC
    // Pan
    public void SetOrthoPanSlider(float val)
    {
        float sens = Mathf.Clamp(val, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("orthoPan", sens);
        orthoPanInput.text = sens.ToString();
    }

    public void SetOrthoPanInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("orthoPan", sens);
        orthoPanSlider.value = sens;
    }
    // Zoom
    public void SetOrthoZoomSlider(float val)
    {
        float sens = Mathf.Clamp(val, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("orthoZoom", sens);
        orthoZoomInput.text = sens.ToString();
    }

    public void SetOrthoZoomInput(string val)
    {
        float sens;
        float.TryParse(val, out sens);
        sens = Mathf.Clamp(sens, 1f, 10f);
        SettingsManager.instance.ChangeSettingsValue("orthoZoom", sens);
        orthoZoomSlider.value = sens;
    }

    // ----- Directory Settings -----

    // Home Directory
    public void OpenHomeDirSelect()
    {
        dirSet = SetDirectory.Home;
        dirFinder.OpenFileSelection(SettingsManager.instance.GetSetting("homedir", ""));
    }

    public void InputHomeDir(string dirpath)
    {
        Debug.Log(dirpath);
        if (Directory.Exists(dirpath))
        {
            SettingsManager.instance.ChangeSettingsValue("homedir", dirpath);
            homeDirInput.text = SettingsManager.instance.homeDirectory;
        }
        else
        {
            Debug.Log("Bad Dirpath");
            homeDirInput.text = SettingsManager.instance.homeDirectory;
        }
    }

    // Home Directory
    public void OpenWorldDirSelect()
    {
        dirSet = SetDirectory.World;
        dirFinder.OpenFileSelection(SettingsManager.instance.GetSetting("worlddir", ""));
    }

    public void InputWorldDir(string dirpath)
    {
        Debug.Log(dirpath);
        if (Directory.Exists(dirpath))
        {
            SettingsManager.instance.ChangeSettingsValue("worlddir", dirpath);
            worldDirInput.text = SettingsManager.instance.worldDirectory;
        }
        else
        {
            Debug.Log("Bad Dirpath");
            worldDirInput.text = SettingsManager.instance.homeDirectory;
        }
    }

    // Home Directory
    public void OpenSimDirSelect()
    {
        dirSet = SetDirectory.Sim;
        dirFinder.OpenFileSelection(SettingsManager.instance.GetSetting("simdir", ""));
    }

    public void InputSimDir(string dirpath)
    {
        Debug.Log(dirpath);
        if (Directory.Exists(dirpath))
        {
            SettingsManager.instance.ChangeSettingsValue("simdir", dirpath);
            simDirInput.text = SettingsManager.instance.simDirectory;
        }
        else
        {
            Debug.Log("Bad Dirpath");
            simDirInput.text = SettingsManager.instance.homeDirectory;
        }
    }


    public GameObject ReceiveFile(string dirpath)
    {
        if (Directory.Exists(dirpath))
        {
            switch (dirSet)
            {
                case SetDirectory.Home:        
                    SettingsManager.instance.ChangeSettingsValue("homedir", dirpath);
                    homeDirInput.text = dirpath;
                    break;
                case SetDirectory.World:
                    SettingsManager.instance.ChangeSettingsValue("worlddir", dirpath);
                    worldDirInput.text = dirpath;
                    break;
                case SetDirectory.Sim:
                    SettingsManager.instance.ChangeSettingsValue("simdir", dirpath);
                    simDirInput.text = dirpath;
                    break;
                default:
                    Debug.Log("Failure to set directory");
                    break;
            }
        }
        return null;
    }

    // ----- Error Settings -----

    // PSD Error
    
    public void PSDMeanErrorInput(string val)
    {
        float mean;
        if (float.TryParse(val, out mean))
        {
            PSDController.globalMean = mean;
            gaussDrawer.DrawPSDGaussian(mean, PSDController.globalStdDev);
        }
    }

    public void PSDStdDevErrorInput(string val)
    {
        float sdev;
        if (float.TryParse(val, out sdev))
        {
            PSDController.globalStdDev = sdev;
            gaussDrawer.DrawPSDGaussian(PSDController.globalMean, sdev);
        }
    }

    // Close Window
    public override void Close()
    {
        UIManager.instance.closeWindow();
        base.Close();
    }
}
