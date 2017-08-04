using UnityEngine;
using UnityEngine.UI;
using RobotComponents;

public class RobotInspectorWindow : MonoBehaviour {

    public Robot robot;

    // Robot identifiers
    [Header("Identifier Text Displays")]
    [SerializeField]
    private Text robotNumber, robotName, robotBinaryName;

    // Sensor variables
    [Header("PSD Text Displays")]
    [SerializeField]
    private Text psdFrontValue, psdLeftValue, psdRightValue;

    [Header("Camera")]
    [SerializeField]
    private RawImage cameraTarget;
    public EyeCamera cameraToDisplay;

    // Position variables
    [Header("Position Text Displays")]
    [SerializeField]
    private Text robotXValue, robotZValue, robotPhiValue;

    // Icon Images
    [Header("Icons")]
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
        robotXValue.text = (1000f * robot.transform.position.x).ToString("N2");
        robotZValue.text = (1000f *robot.transform.position.z).ToString("N2");
        robotPhiValue.text = robot.transform.rotation.eulerAngles.y.ToString("N2");
	}

    public void LockButton()
    {
        robot.locked = !robot.locked;
        lockButtonImage.sprite = robot.locked ? lockedImage : unlockedImage;
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
