using UnityEngine;
using UnityEngine.UI;
using RobotComponents;

public class RobotInspectorWindow : MonoBehaviour {

    public Robot robot;

    // Robot identifiers
    [SerializeField]
    private Text robotNumber, robotName, robotBinaryName;

    // Sensor variables
    [SerializeField]
    private Text psdFrontValue, psdLeftValue, psdRightValue;

    [SerializeField]
    private RawImage cameraTarget;
    public EyeCamera cameraToDisplay;

    // Position variables
    [SerializeField]
    private InputField robotXValue, robotZValue, robotPhiValue;

    // Icon Images
    public Image lockButtonImage;
    public Sprite lockedImage;
    public Sprite unlockedImage;

    // Use this for initialization
    void Start () {
        if(robot is ICameras)
        {
            cameraToDisplay = (robot as ICameras).GetCameraComponent(0);
            cameraTarget.texture = cameraToDisplay.rendTex;
        }
        lockButtonImage.sprite = robot.locked ? lockedImage : unlockedImage;
        robotNumber.text = "ID # " + robot.objectID.ToString();
        robotName.text = robot.name;
    }
	
	// Update is called once per frame
	void Update () {
        if (robot is IPSDSensors)
        {
            psdLeftValue.text = (robot as IPSDSensors).GetPSD(0).ToString();
            psdFrontValue.text = (robot as IPSDSensors).GetPSD(1).ToString();
            psdRightValue.text = (robot as IPSDSensors).GetPSD(2).ToString();
        }
        if (robot.locked)
        {
            robotXValue.text = robot.transform.position.x.ToString();
            robotZValue.text = robot.transform.position.z.ToString();
            robotPhiValue.text = robot.transform.rotation.eulerAngles.y.ToString();
        }
	}

    public void LockButton()
    {
        robot.locked = !robot.locked;
        lockButtonImage.sprite = robot.locked ? lockedImage : unlockedImage;
        robotXValue.readOnly = robot.locked;
        robotZValue.readOnly = robot.locked;
        robotPhiValue.readOnly = robot.locked;
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
}
