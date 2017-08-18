using UnityEngine;
using UnityEngine.UI;
using RobotComponents;

public class RobotInspectorWindow : MonoBehaviour {

    // Reference to the robot this window displays
    // This is set during the creation of the window
    public Robot robot;

    // Robot identifiers
    [Header("Identifier Text Displays")]
    [SerializeField]
    private Text robotNumber;
    [SerializeField]
    private Text robotName, robotBinaryName;

    // Sensor variables
    [Header("PSD Text Displays")]
    [SerializeField]
    private Text psdFrontValue;
    [SerializeField]
    private Text psdLeftValue, psdRightValue;

    [Header("Camera")]
    [SerializeField]
    private RawImage cameraTarget;
    public EyeCamera cameraToDisplay;

    // Position variables
    [Header("Position Text Displays")]
    [SerializeField]
    private InputField robotXValue;
    [SerializeField]
    private InputField robotZValue, robotPhiValue;

    // Icon Images
    [Header("Icons")]
    public Image lockButtonImage;
    public Image trailButtonImage;
    public Sprite lockedImage;

    // Use this for initialization
    void Start () {
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
        robotBinaryName.text = robot.controlBinaryPath;
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
        Destroy(gameObject);
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
