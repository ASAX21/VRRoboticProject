﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RobotComponents;

public class RobotInspectorWindow : TabWindow {

    // Reference to the robot this window displays
    // This is set during the creation of the window
    public Robot robot;

    // Robot identifiers
    [Header("Identifier Text Displays")]
    public Text robotNumber;
    public Text robotName;
    public Text robotBinaryName;

    // Sensor variables
    [Header("PSD")]
    public Text psdFrontValue;
    public Text psdLeftValue;
    public Text psdRightValue;

    public Toggle globalError;
    [SerializeField]
    private Text globalErrorLabel, psdMeanLabel, psdStdDevLabel;
    [SerializeField]
    private InputField psdMeanError, psdStdDevError;

    // Camera Display
    [Header("Camera")]
    [SerializeField]
    private RawImage cameraTarget;
    public EyeCamera cameraToDisplay;
    public Text cameraResolution;

    // Camera Noise
    [SerializeField]
    private List<Text> saltPepperText;
    [SerializeField]
    private Slider saltPepperPercentSlider, saltPepperRatioSlider;
    [SerializeField]
    private List<Text> gaussNoiseText;
    [SerializeField]
    private Input gaussMeanInput, gaussStdDevInput;


    // Position variables
    [Header("Position Text Displays")]
    public InputField robotXValue;
    public InputField robotZValue;
    public InputField robotPhiValue;

    // Icon Images
    [Header("Icons")]
    public Image lockButtonImage;
    public Image trailButtonImage;
    public Sprite lockedImage;

    public Toggle toggleVWAccurate;

    // Use this for initialization
    public void Initialize () {
        if(robot is ICameras)
        {
            cameraToDisplay = (robot as ICameras).GetCameraComponent(0);
            cameraTarget.texture = cameraToDisplay.rendTex;
        }

        lockButtonImage.sprite = lockedImage;
        robotNumber.text = "ID # " + robot.objectID.ToString();
        robotName.text = robot.name;

        trailButtonImage.color = robot.trail.enabled ? Color.white : Color.grey;

        robotXValue.interactable = SimManager.instance.isPaused;
        robotZValue.interactable = SimManager.instance.isPaused;
        robotPhiValue.interactable = SimManager.instance.isPaused;

        if (robot is LabBot)
            toggleVWAccurate.isOn = (robot as LabBot).wheelController.realCoords;
        else if (robot is S4)
            toggleVWAccurate.isOn = (robot as S4).wheelController.realCoords;

        if(robot is IPSDSensors)
        {
            if((robot as IPSDSensors).UseGlobalError)
            {
                psdMeanError.text = PSDController.globalMean.ToString("N2");
                psdStdDevError.text = PSDController.globalStdDev.ToString("N2");
            }
            else
            {
                psdMeanError.text = (robot as IPSDSensors).MeanError.ToString("N2");
                psdStdDevError.text = (robot as IPSDSensors).StdDevError.ToString("N2");
            }
        }
        SaltPepperNoiseToggle(false);
        GaussianNoiseToggle(false);
        PSDErrorEnabled(false);
        SimManager.instance.OnPause += OnSimPaused;
        SimManager.instance.OnResume += OnSimResumed;
    }

    // Remove pause callbacks from delegate
    void OnDestroy()
    {
        SimManager.instance.OnPause -= OnSimPaused;
        SimManager.instance.OnResume -= OnSimResumed;
    }

    // Update is called once per frame
    void Update () {
        if (robot is IPSDSensors)
        {
            psdLeftValue.text = (robot as IPSDSensors).GetPSD(0).ToString();
            psdFrontValue.text = (robot as IPSDSensors).GetPSD(1).ToString();
            psdRightValue.text = (robot as IPSDSensors).GetPSD(2).ToString();
        }
        if (!SimManager.instance.isPaused)
        {
            robotXValue.text = (1000f * robot.transform.position.x).ToString("N2");
            robotZValue.text = (1000f * robot.transform.position.z).ToString("N2");
            robotPhiValue.text = robot.transform.rotation.eulerAngles.y.ToString("N2");
        }
        if(robot is ICameras)
        {
            cameraResolution.text = (robot as ICameras).GetCameraResolution(0);
        }
        robotBinaryName.text = robot.controlBinaryPath;
	}
    
    // ----- CAMERA CONTENT -----

    public void SaltPepperNoiseToggle(bool val)
    {
        if (!(robot is ICameras))
            return;

        foreach(Text t in saltPepperText)
            t.color = val ? Color.black : Color.gray;
        saltPepperPercentSlider.interactable = val;
        saltPepperRatioSlider.interactable = val;
    }

    public void GaussianNoiseToggle(bool val)
    {

    }

    // ----- PSD CONTENT -----
    // Enable Error
    public void PSDErrorEnabled(bool val)
    {
        if (!(robot is IPSDSensors))
            return;
        if (val)
        {
            psdMeanError.textComponent.color = Color.black;
            psdStdDevError.textComponent.color = Color.black;
            psdMeanLabel.color = Color.black;
            psdStdDevLabel.color = Color.black;
            globalErrorLabel.color = Color.black;
        }
        else
        {
            psdMeanError.textComponent.color = Color.grey;
            psdStdDevError.textComponent.color = Color.grey;
            psdMeanLabel.color = Color.grey;
            psdStdDevLabel.color = Color.grey;
            globalErrorLabel.color = Color.grey;
        }
        psdMeanError.interactable = val;
        psdStdDevError.interactable = val;
        globalError.interactable = val;
    }

    public void PSDGlobalError(bool val)
    {
        if (!(robot is IPSDSensors))
            return;
        psdMeanError.interactable = !val;
        psdStdDevError.interactable = !val;
        (robot as IPSDSensors).UseGlobalError = val;
        if (val)
        {
            psdMeanError.text = PSDController.globalMean.ToString("N2");
            psdStdDevError.text = PSDController.globalStdDev.ToString("N2");
        }
        else
        {
            psdMeanError.text = (robot as IPSDSensors).MeanError.ToString("N2");
            psdStdDevError.text = (robot as IPSDSensors).StdDevError.ToString("N2");
        }
    }
    // PSD Sensor Error Inputs
    public void SetErrorMean(string mean)
    {
        float m;
        if(float.TryParse(mean, out m))
        {
            (robot as IPSDSensors).MeanError = m;
        }
    }

    public void SetErrorStdDev(string dev)
    {
        float d;
        if (float.TryParse(dev, out d))
        {
            (robot as IPSDSensors).StdDevError = d;
        }
    }

    private void OnSimPaused()
    {
        robotXValue.interactable = true;
        robotZValue.interactable = true;
        robotPhiValue.interactable = true;
    }

    private void OnSimResumed()
    {
        robotXValue.interactable = false;
        robotZValue.interactable = false;
        robotPhiValue.interactable = false;
    }

    public void SetXPosition(string x)
    {
        Vector3 pos = robot.transform.position;
        pos.x = float.Parse(x)/1000f;
        robot.transform.position = pos;
    }

    public void SetYPosition(string y)
    {
        Vector3 pos = robot.transform.position;
        pos.z = float.Parse(y)/1000f;
        robot.transform.position = pos;
    }

    public void SetPhiPosition(string phi)
    {
        robot.transform.rotation = Quaternion.Euler(0, float.Parse(phi), 0);
    }

    public void TrailButton()
    {
        robot.ToggleTrail();
        trailButtonImage.color = robot.trail.enabled ? Color.white : Color.grey;
    }

    public void LockButton()
    {
        robot.locked = !robot.locked;
        lockButtonImage.color = robot.locked ? Color.white : Color.grey;
    }

    public void DeleteButton()
    {
        SimManager.instance.RemoveRobotFromScene(robot);
        Destroy(gameObject);
    }

    public void CloseWindow()
    {
        robot.isWindowOpen = false;
        gameObject.SetActive(false);
    }

    public void ToggleAccurate(bool toggle)
    {
        if(robot is LabBot)
            (robot as LabBot).wheelController.realCoords = toggle;
        if (robot is S4)
            (robot as S4).wheelController.realCoords = toggle;
    }

    public void AddRobotControl()
    {
        UIManager.instance.LoadControlProgram(robot);
    }

    public void DisconnectRobotControl()
    {
        robot.DisconnectRobot();
    }
}
