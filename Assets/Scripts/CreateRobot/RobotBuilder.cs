using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class RobotBuilder: MonoBehaviour, IFileReceiver{

    public static RobotBuilder instance = null;
	public Robot robot;
	public GameObject robotObject;
	public string filepath;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject ReceiveFile(string filepath)
    {
		robotObject = new GameObject("Robot");
		Rigidbody rb = robotObject.AddComponent<Rigidbody>();
		rb.interpolation = RigidbodyInterpolation.None;
		rb.mass = 1500;
        this.filepath = filepath;
        IO io = new IO();
        if (!io.Load(filepath))
            return null;
        string line;

        while ( (line = io.readLine()) != "ENDOFFILE") {
			if (line.Length > 0) {
				if (line [0] != '#') {
					process (line);
				}
			}
		}
        robot.PostBuild();
        ObjectManager.instance.AddObjectToMouse(robotObject.GetComponent<PlaceableObject>());
        ServerManager.instance.testBot = robot;
		return robotObject;
	}

	public void process(string line){
        
		string[] args = line.Split (' ');
        switch (args[0]) {
            case "name":
                SetType(args[1]);
                break;  
		    case "model":
			    addModel(args[1]);
			    break;
            case "axis":
				addAxel(float.Parse(args[1])/1000, float.Parse(args[2])/1000);
                break;
		    case "psd":
				Vector3 position = new Vector3 (-1*float.Parse(args[4])/1000,float.Parse(args[5])/1000,float.Parse(args[3])/1000);
				Quaternion rotation = Quaternion.Euler (0, -1*float.Parse(args[6]), 0);
				addPSD(args[1], args[2], position, rotation);
			    break;
		    case "wheel":
                Debug.Log("ADDING WHEEL");
                addWheels(float.Parse (args [1]) / 1000f, float.Parse(args[2]), int.Parse(args[3]), float.Parse (args [4]) / 1000f, 0);
			    break;
			case "camera":
                Debug.Log("ADDING CAMERA");
				addCamera(new Vector3(float.Parse (args [2]) / 1000f, float.Parse (args [3]) / 1000f, float.Parse (args [1]) / 1000f), 
						float.Parse (args [4]), float.Parse (args [5]), int.Parse (args [6]), int.Parse (args [7]));
				break;
		    default:
			    break;
		}
	}

	void addPSD(string name, string id, Vector3 position, Quaternion rotation) {
		if(!(robot is IPSDSensors))
		{
			Debug.Log("Trying to add PSD to unsupported robot type");
		}
		else
		{
			Transform psdContainer = robotObject.transform.Find("PSDContainer");
			if (psdContainer == null)
				psdContainer = AddPSDContainer();

			GameObject psd = new GameObject(name);
			psd.transform.SetParent(psdContainer, false);
			psd.transform.localPosition = position;
			psd.transform.localRotation = rotation;

			PSDSensor sensor = psd.AddComponent<PSDSensor>();
			(robot as LabBot).psdController.sensors.Add(sensor);
		}
	}



	public void SetType(string type)
	{
		switch (type)
		{
		case "LabBot":
			robot = robotObject.AddComponent<LabBot>();
            (robot as LabBot).Initialize();
			break;
		default:
			Debug.Log("Couldnt find robot type");
			break;
		}
	}

	public void addModel(string modelPath)
	{

		string fullpath = filepath.Substring (0, filepath.LastIndexOf (ApplicationHelper.slash())) + ApplicationHelper.slash() + modelPath;
		GameObject model = OBJLoader.LoadOBJFile(fullpath);
        model.name = "Model";
		model.transform.SetParent(robotObject.transform, false);
		model.transform.rotation = Quaternion.Euler(0, 90, 0);

		BoxCollider robCollider = robotObject.AddComponent<BoxCollider>();
		robCollider.center = new Vector3(0, 0.101f, 0);
		robCollider.size = new Vector3(0.18f, 0.195f, 0.2f);
		robCollider.material = Resources.Load ("nofriction") as PhysicMaterial;
	}

	public void AddCube()
	{
		robot = robotObject.AddComponent<LabBot>();
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.SetParent(robotObject.transform, false);
		cube.transform.localScale = new Vector3(1, 1, 3);
	}

	private Transform AddPSDContainer()
	{
		GameObject psdContainer = new GameObject("PSDContainer");
		psdContainer.transform.SetParent(robotObject.transform, false);
		return psdContainer.transform;
	}

	private Transform AddWheelContainer()
	{
		GameObject wheelContainer = new GameObject("WheelContainer");
		wheelContainer.transform.SetParent(robotObject.transform, false);
		return wheelContainer.transform;
	}

	// Adds an axel (this is just a gameobject that represnets the 
	// z (forwards/backwars) and y (up/down) locaation
	public void addAxel(float z, float y)
	{
		GameObject axel = new GameObject("Axel" + robot.axels);
		robot.axels++;
		axel.transform.SetParent(robotObject.transform, false);
		axel.transform.position = new Vector3(0, y, z);
	}

	// Add Wheels adds a pair of wheels on an axis
	public void addWheels(float diameter, float vel, int enc, float wBase, int axel)
	{
		Transform axelTransform = robotObject.transform.Find("Axel" + axel);
		if(axelTransform == null)
		{
			Debug.Log("Trying to attach wheels to a non existand axels");
			return;
		}
		else
		{
			Object wheelPrefab = Resources.Load("WheelPrefab");
			GameObject wheelLeftObj = Object.Instantiate(wheelPrefab, axelTransform, false) as GameObject;
			wheelLeftObj.name = "leftwheel";
			GameObject wheelRightObj = Object.Instantiate(wheelPrefab, axelTransform, false) as GameObject;
			wheelRightObj.name = "rightwheel";

			wheelLeftObj.transform.SetParent(axelTransform, false);
			wheelRightObj.transform.SetParent(axelTransform, false);
			wheelLeftObj.transform.localScale = new Vector3(diameter, diameter, diameter);
			wheelRightObj.transform.localScale = new Vector3(diameter, diameter, diameter);
			wheelLeftObj.transform.localPosition = new Vector3(-wBase, 0, 0);
			wheelRightObj.transform.localPosition = new Vector3(wBase, 0, 0);

			Wheel wheelLeft = wheelLeftObj.AddComponent<Wheel> ();
			wheelLeft.rigidBody = wheelLeftObj.GetComponent<Rigidbody>();
			wheelLeft.wheelHingeJoint = wheelLeftObj.GetComponent<HingeJoint>();
			wheelLeft.GetComponent<HingeJoint>().connectedBody = robotObject.GetComponent<Rigidbody> ();
			wheelLeft.wheelCollider = wheelLeftObj.GetComponent<CapsuleCollider>();
			wheelLeft.speed = 0;
			wheelLeft.diameter = diameter;
			(robot as LabBot).wheelController.wheels.Add(wheelLeft);

			Wheel wheelRight = wheelRightObj.AddComponent<Wheel> ();
			wheelRight.rigidBody = wheelRightObj.GetComponent<Rigidbody>();
			wheelRight.wheelHingeJoint = wheelRightObj.GetComponent<HingeJoint>();
			wheelRight.GetComponent<HingeJoint>().connectedBody = robotObject.GetComponent<Rigidbody> ();
			wheelRight.wheelCollider = wheelRightObj.GetComponent<CapsuleCollider>();
			wheelRight.speed = 0;
			wheelRight.diameter = diameter;
			(robot as LabBot).wheelController.wheels.Add(wheelRight);

			(robot as LabBot).wheelController.wheelDist = wBase * 2;
		}
	}

	public void addCamera(Vector3 position, float pan, float tilt, int imageWidth, int imageHeight) {
		if(!(robot is ICameras))
		{
			Debug.Log("Trying to add Camera to unsupported robot type");
		}
		else
		{
			Transform CameraContainer = robotObject.transform.Find("CameraContainer");
			if (CameraContainer == null)
				CameraContainer = AddCameraContainer();
            Debug.Log("addeing camear");
			Object cameraPrefab = Resources.Load("CameraPrefab");
			GameObject camera = Object.Instantiate(cameraPrefab) as GameObject;
			camera.transform.SetParent(CameraContainer, false);
			camera.transform.localPosition = position;
			camera.transform.localRotation = Quaternion.Euler (tilt, pan, 0);

			EyeCamera eyecamera = camera.AddComponent<EyeCamera>();
			eyecamera.cameraComponent = camera.GetComponent<Camera>();
			(robot as LabBot).eyeCamController.cameras.Add (eyecamera);
		}
	}

	public Transform AddCameraContainer() {
		GameObject cameraContainer = new GameObject("CameraContainer");
		cameraContainer.transform.SetParent(robotObject.transform, false);
		return cameraContainer.transform;
	}
}
