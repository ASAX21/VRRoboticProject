using System;
using System.Collections.Generic;
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

    [SerializeField]
    private Toggle visualiseToggle;

    private bool useGlobalError;
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
    [Header("Driving")]
    public InputField robotXValue;
    public InputField robotZValue;
    public InputField robotPhiValue;
    public Text vwXtext;
    public Text vwYtext;
    public Text vwPhiText;

    [Header("Control")]
    public Text controlName;

    // Icon Images
    [Header("Icons")]
    public Image lockButtonImage;
    public Image trailButtonImage;
    public Sprite lockedImage;

    public Toggle toggleVWAccurate;

    // Robot Components
    private PSDController psdController;

    // Use this for initialization
    void Awake()
    {
        if (robot is ICameras)
        {
            if((cameraToDisplay = (robot as ICameras).GetCameraComponent(0)) != null)
                cameraTarget.texture = cameraToDisplay.rendTex;
            else
                Debug.Log("Failed to get camera component");
        }

        lockButtonImage.sprite = lockedImage;
        robotNumber.text = "ID # " + robot.objectID.ToString();
        robotName.text = robot.name;
        trailButtonImage.color = robot.trail.enabled ? Color.white : Color.grey;

        if (robot is IVWDrive)
            toggleVWAccurate.isOn = (robot as IVWDrive).VWAccurate;

        if (robot is IPSDSensors)
        {
            if ((psdController = robot.GetComponent<PSDController>()) == null)
                Debug.Log("No PSD Controller");
            if ((robot as IPSDSensors).UseGlobalError)
            {
                psdMeanError.text = PSDController.globalMean.ToString("N2");
                psdStdDevError.text = PSDController.globalStdDev.ToString("N2");
            }
            else
            {
                psdMeanError.text = (robot as IPSDSensors).MeanError.ToString("N2");
                psdStdDevError.text = (robot as IPSDSensors).StdDevError.ToString("N2");
            }
            useGlobalError = (robot as IPSDSensors).UseGlobalError;
        }

        visualiseToggle.isOn = SimManager.instance.defaultVis;
        SaltPepperNoiseToggle(false);
        GaussianNoiseToggle(false);
        PSDErrorEnabled(false);

        SimManager.instance.OnPause += OnSimPaused;
        SimManager.instance.OnResume += OnSimResumed;
    }

    void OnEnable()
    {
        robotXValue.interactable = SimManager.instance.isPaused;
        robotZValue.interactable = SimManager.instance.isPaused;
        robotPhiValue.interactable = SimManager.instance.isPaused;

        controlName.text = robot.controlBinaryName;
    }

    // Remove pause callbacks from delegate
    void OnDestroy()
    {
        SimManager.instance.OnPause -= OnSimPaused;
        SimManager.instance.OnResume -= OnSimResumed;
    }

    // Update is called once per frame
    void Update()
    {
        if (robot is IPSDSensors)
        {
            psdLeftValue.text = psdController.GetPSDValue(0).ToString();
            psdFrontValue.text = psdController.GetPSDValue(1).ToString();
            psdRightValue.text = psdController.GetPSDValue(2).ToString();
        }
        if (!SimManager.instance.isPaused)
        {
            robotXValue.text = (Eyesim.Scale * robot.transform.position.x).ToString("N2");
            robotZValue.text = (Eyesim.Scale * robot.transform.position.z).ToString("N2");
            robotPhiValue.text = Eyesim.UnityToEyeSimAngle(robot.transform.rotation.eulerAngles.y).ToString("N2");
        }
        if (robot is IVWDrive)
        {
            Int16[] pos = (robot as IVWDrive).GetPose();
            vwXtext.text = pos[0].ToString("N2");
            vwYtext.text = pos[1].ToString("N2");
            vwPhiText.text = pos[2].ToString("N2");
        }
    }

    // ----- CAMERA CONTENT -----

    // Refresh render target
    public void UpdateCameraTarget()
    {
        cameraTarget.texture = cameraToDisplay.rendTex;
    }

    public void SaltPepperNoiseToggle(bool val)
    {
        if (!(robot is ICameras))
            return;

        foreach(Text t in saltPepperText)
            t.color = val ? Color.black : Color.gray;
        saltPepperPercentSlider.interactable = val;
        saltPepperRatioSlider.interactable = val;
        (robot as ICameras).SaltPepperNoise = val;
    }

    public void SaltPepperNoisePercent(float val)
    {
        if (!(robot is ICameras))
            return;
        (robot as ICameras).SPPixelPercent = val;
    }

    public void SaltPepperNoiseRatio(float val)
    {
        if (!(robot is ICameras))
            return;
        (robot as ICameras).SPBWRatio = val;
    }

    public void GaussianNoiseToggle(bool val)
    {

    }

    // ----- PSD CONTENT -----
    // Toggle Visualize

    public void ToggleVisualise(bool val)
    {
        if (!(robot is IPSDSensors))
            return;

        (robot as IPSDSensors).SetVisualize(val);
    }

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
        (robot as IPSDSensors).UseError = val;
        globalError.interactable = val;
        psdMeanError.interactable = val && !useGlobalError;
        psdStdDevError.interactable = val && !useGlobalError;
    }

    public void PSDGlobalError(bool val)
    {
        if (!(robot is IPSDSensors))
            return;
        psdMeanError.interactable = !val;
        psdStdDevError.interactable = !val;
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
        (robot as IPSDSensors).UseGlobalError = val;
        useGlobalError = val;
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
    
    // ----- Driving Content -----
    // 

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
        pos.x = float.Parse(x)/Eyesim.Scale;
        robot.transform.position = pos;
    }

    public void SetYPosition(string y)
    {
        Vector3 pos = robot.transform.position;
        pos.z = float.Parse(y)/Eyesim.Scale;
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

    public override void Close()
    {
        robot.isWindowOpen = false;
        base.Close();
    }

    public void ToggleAccurate(bool toggle)
    {
        if(robot is IVWDrive)
            (robot as IVWDrive).VWAccurate = toggle;
    }

    // Control content

    public void AddRobotControl()
    {
        UIManager.instance.LoadControlProgram(robot);
    }

    public void DisconnectRobotControl()
    {
        robot.DisconnectRobot();
    }
}
