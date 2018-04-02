using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class BaseAckDrive : Robot, IMotors,
    IPIDUsable,
    IPSDSensors,
    IServos,
    ICameras,
    IAudio,
    IRadio,
    ILaser,
    ConfigureableRobot
{
    // Components of the robot
    [Header("Physical Components")]
    public List<Collider> robotColliders;
    public Rigidbody robotRigidbody;
    public Transform axelTurn;
    public Transform axelDrive;

    [Header("Model")]
    public Transform modelContainer;
    public GameObject robotModel;

    public Transform PSDContainer;
    public GameObject PSDPrefab;

    // EyeCam position is the top level servo (ServoPan)
    public HingeJoint eyeCamPosition;

    // Controllers
    [Header("Controller References")]
    public AckermannController wheelController;
    public PSDController psdController;
    public ServoController servoController;
    public EyeCameraController eyeCamController;
    public AudioController audioController;
    public RadioController radioController;
    public LaserScanController laserScanController;

    // Enabled parameters
    public bool psdEnabled;
    public bool servoEnabled;
    public bool camEnabled;
    public bool audioEnabled;
    public bool radioEnabled;
    public bool laserEnabled;

    Action<RobotConnection> driveDoneDelegate;
    Action<RobotConnection, byte[]> radioMessageDelegate;

    internal override void Awake()
    {
        psdController.sensors = new List<PSDSensor>();
        base.Awake();
    }

    /* ----- Robot Configuration ----- */

    public void ConfigureMass(float mass, Vector3 com)
    {
        robotRigidbody.mass = mass;
        robotRigidbody.centerOfMass = com;
    }

    // Add a box collider
    public void AddBox(Vector3 size, Vector3 centre, PhysicMaterial friction)
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = size;
        box.center = centre;
        box.material = friction;
        robotColliders.Add(box);
    }

    // Add a sphere collider
    public void AddSphere(float radius, Vector3 centre, PhysicMaterial friction)
    {
        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.radius = radius;
        sphere.center = centre;
        sphere.material = friction;
        robotColliders.Add(sphere);
    }

    // Add a capsule collider
    public void AddCapsule(float radius, float height, Vector3 centre, PhysicMaterial friction)
    {
        CapsuleCollider cap = gameObject.AddComponent<CapsuleCollider>();
        cap.radius = radius;
        cap.height = height;
        cap.center = centre;
        cap.material = friction;
        robotColliders.Add(cap);
    }

    public void ConfigureModel(GameObject newModel, Vector3 pos, Vector3 rot)
    {
        if (robotModel != null)
            Destroy(robotModel);

        // Put model into container
        robotModel = newModel;
        robotModel.transform.parent = modelContainer;
        robotModel.transform.localPosition = Vector3.zero;
        robotModel.transform.rotation = Quaternion.identity;

        // Set container position to offset model if required
        modelContainer.localPosition = pos;
        modelContainer.localRotation = Quaternion.Euler(rot);
    }

    // Configure axel height (vertical into robot) and position along z-axis (forward)
    public void ConfigureAxel(float axelHeight, float axelPos, AxelType type)
    {
        Vector3 axel = new Vector3(0f, axelHeight, axelPos);
        if(type == AxelType.Drive)
            axelDrive.localPosition = axel;
        else if(type == AxelType.Turn)
            axelTurn.localPosition = axel;
        wheelController.wheelDist = Mathf.Abs(axelTurn.localPosition.z - axelDrive.localPosition.z);
    }

    public void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track, AxelType type)
    {
        if(type == AxelType.Drive)
        {
            Wheel leftWheel = wheelController.driveWheels[0];
            leftWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(-track, axelDrive.localPosition.y, axelDrive.localPosition.z);
            leftWheel.transform.localPosition = new Vector3(-track, 0f, 0f);
            leftWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
            leftWheel.encoderRate = ticksPerRev;
            leftWheel.maxSpeed = maxVel;
            leftWheel.diameter = diameter;

            Wheel rightWheel = wheelController.driveWheels[1];
            rightWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(track, axelDrive.localPosition.y, axelDrive.localPosition.z);
            rightWheel.transform.localPosition = new Vector3(track, 0f, 0f);
            rightWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
            rightWheel.encoderRate = ticksPerRev;
            rightWheel.maxSpeed = maxVel;
            rightWheel.diameter = diameter;
        }
        else if(type == AxelType.Turn)
        {
            Wheel leftWheel = wheelController.turnWheels[0];
            leftWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
            leftWheel.SetTrack(-track, axelTurn.localPosition.y, axelTurn.localPosition.z);
            leftWheel.encoderRate = ticksPerRev;
            leftWheel.maxSpeed = maxVel;

            Wheel rightWheel = wheelController.turnWheels[1];
            rightWheel.SetTrack(track, axelTurn.localPosition.y, axelTurn.localPosition.z);
            rightWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
            rightWheel.encoderRate = ticksPerRev;
            rightWheel.maxSpeed = maxVel;

            wheelController.wheelDist = track;
        }
    }

    public bool AddPSDSensor(int id, string name, Vector3 pos, float rot)
    {
        if(id != psdController.sensors.Count + 1)
        {
            Debug.Log("Expected incrementing IDs " + psdController.sensors.Count + "  " + id);
            return false;
        }
        GameObject newPSD = Instantiate(PSDPrefab, PSDContainer);
        newPSD.name = name;
        newPSD.transform.localPosition = pos;
        newPSD.transform.localEulerAngles = new Vector3(0f, -rot, 0f);
        psdController.sensors.Add(newPSD.GetComponent<PSDSensor>());
        psdEnabled = true;
        return true;
    }

    public void ConfigureCamera(Vector3 pos, float pan, float tilt, float maxPan, float maxTilt)
    {
        GameObject cam = eyeCamController.cameras[0].gameObject;
        eyeCamPosition.connectedAnchor = pos;
        eyeCamPosition.transform.localPosition = pos;
        eyeCamPosition.transform.localEulerAngles = new Vector3(tilt, pan, 0f);
        servoController.servos[0].maxAngle = maxPan;
        servoController.servos[0].minAngle = -maxPan;
        servoController.servos[1].maxAngle = maxTilt;
        servoController.servos[1].minAngle = -maxTilt;
        camEnabled = true;
    }

    public void ConfigureLidar(int angRange, int numPoints, int tilt)
    {
        laserScanController.SetAngularRange(angRange, numPoints);
        laserScanController.laserScanner.localRotation = Quaternion.Euler(new Vector3(-tilt, 0, 0));
    }

    /* ----- Driving Functions ----- */

    public void DriveDoneCallback()
    {
        driveDoneDelegate(myConnection);
    }

    public int GetEncoder(int quad)
    {
        return 0;
    }

    public void DriveMotor(int motor, int speed)
    {
        if(motor == 0)
            wheelController.SetDriveSpeed(speed);
        else
            Debug.Log("Only one supported motor for ackermann");
         //wheelController.SetMotorSpeed(motor, speed);
    }

    public void DriveMotorControlled(int motor, int ticks)
    {
        Debug.Log("Motor Controlled not implemented for Ackermann drive");
        return;
    }

    public void SetPID(int motor, int p, int i, int d)
    {
        wheelController.SetPIDParams(motor, p, i, d);
    }

    // Special Case: Servo of 1 (input, interpereter converts to 0) is wheel angle
    public void SetServo(int servo, int angle)
    {
        if(servo == 0)
            wheelController.SetTurnAngle(angle);
        else if(servoEnabled)
            servoController.SetServoPosition(servo - 1, angle);
    }


    public ushort GetPSD(int psd)
    {
        return psdController.GetPSDValue(psd);
    }

    public float MeanError
    {
        get
        {
            return psdController.normalMean;
        }
        set
        {
            psdController.normalMean = value;
        }
    }

    public float StdDevError
    {
        get
        {
            return psdController.normalStdDev;
        }
        set
        {
            psdController.normalStdDev = value;
        }
    }

    // Add error to sensor values
    public bool UseError
    {
        get
        {
            return psdController.errorEnabled;
        }
        set
        {
            psdController.errorEnabled = value;
        }
    }

    // Use global or local values for calculating error
    public bool UseGlobalError
    {
        get
        {
            return psdController.useGlobalError;
        }
        set
        {
            psdController.useGlobalError = value;
        }
    }

    public void SetVisualize(bool val)
    {
        psdController.VisualiseAllSensors(val);
    }

    public byte[] GetCameraOutput(int camera)
    {
        if (camEnabled)
            return eyeCamController.GetBytes(camera);
        else
            return null;
    }

    public void SetCameraResolution(int camera, int width, int height)
    {
        if(camEnabled)
            eyeCamController.SetResolution(camera, width, height);
    }

    public string GetCameraResolution(int camera)
    {
        return eyeCamController.GetResolution(camera);
    }

    public EyeCamera GetCameraComponent(int camera)
    {
        if (camEnabled)
            return eyeCamController.cameras[camera];
        else
            return null;
    }

    public bool SaltPepperNoise
    {
        get
        {
            return eyeCamController.useSNPNoise;
        }
        set
        {
            eyeCamController.useSNPNoise = value;
        }
    }

    public float SPPixelPercent
    {
        get
        {
            return eyeCamController.saltPepperPercent;
        }
        set
        {
            eyeCamController.saltPepperPercent = value;
        }
    }

    public float SPBWRatio
    {
        get
        {
            return eyeCamController.saltPepperRatio;
        }
        set
        {
            eyeCamController.saltPepperRatio = value;
        }
    }

    public bool GaussianNoise
    {
        get
        {
            return eyeCamController.useGaussNoise;
        }
        set
        {
            eyeCamController.useGaussNoise = value;
        }
    }

    public float GaussMean
    {
        get
        {
            return eyeCamController.gaussMean;
        }
        set
        {
            eyeCamController.gaussMean = value;
        }
    }

    public float GaussStdDev
    {
        get
        {
            return eyeCamController.gaussStdDev;
        }
        set
        {
            eyeCamController.gaussStdDev = value;
        }
    }

    public void PlayBeep()
    {
        audioController.PlayBeep();
    }

    public void AUPlay(AudioClip clip)
    {
        audioController.PlayClip(clip);
    }

    public void AddMessageToBuffer(byte[] msg)
    {
        radioController.QueueMessage(msg);
    }

    public byte[] RetrieveMessageFromBuffer()
    {
        return radioController.DequeueMessage();
    }

    public void RadioReceivedCallback(byte[] msg)
    {
        radioMessageDelegate(myConnection, msg);
    }

    public void WaitForRadioMessage(Action<RobotConnection, byte[]> radioDelegate)
    {
        radioMessageDelegate = radioDelegate;
        radioController.receivedCallback = RadioReceivedCallback;
    }

    public int GetNumberOfMessages()
    {
        return radioController.numMessages;
    }

    public int[] LaserScan()
    {
        if (laserEnabled)
            return laserScanController.Scan();
        else
            return null;
    }
}