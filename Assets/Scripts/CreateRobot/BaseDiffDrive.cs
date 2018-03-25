using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class BaseDiffDrive : Robot, IMotors,
    IPIDUsable,
    IPSDSensors,
    IServos,
    IVWDrive,
    ICameras,
    IAudio,
    IRadio,
    ILaser,
    ConfigureableRobot
{
    // Components of the robot
    [Header("Physical Components")]
    public List<Collider> robotColliders;
    public CapsuleCollider robotBunt;
    public Rigidbody robotRigidbody;
    public Transform axel;

    [Header("Model")]
    public Transform modelContainer;
    public GameObject robotModel;

    [Header("PSD")]
    public Transform PSDContainer;
    public GameObject PSDPrefab;

    // EyeCam position is the top level servo (ServoPan)
    [Header("Camera")]
    public HingeJoint eyeCamPosition;

    // Controllers
    [Header("Controller References")]
    public WheelMotorController wheelController;
    public PSDController psdController;
    public ServoController servoController;
    public EyeCameraController eyeCamController;
    public AudioController audioController;
    public RadioController radioController;
    public LaserScanController laserScanController;

    // Enabled parameters
    [Header("Enabled Systems")]
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

    public void TEST()
    {
        ConfigureCamera(new Vector3(0, 50/Eyesim.Scale, 70/ Eyesim.Scale), 0f, 0f, 90f, 90f);
    }

    /* ----- Robot Configuration ----- */

    public void ConfigureModel(GameObject newModel, Vector3 pos, Vector3 rot)
    {
        if (robotModel != null)
            Destroy(robotModel);

        // Set container position to offset model if required
        modelContainer.localPosition = pos;
        modelContainer.localRotation = Quaternion.Euler(rot);
        // Put model into container
        robotModel = newModel;
        robotModel.transform.parent = modelContainer;
        robotModel.transform.localPosition = Vector3.zero;
        robotModel.transform.localRotation = Quaternion.identity;
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

    public void ConfigureMass(float mass, Vector3 com)
    {
        robotRigidbody.mass = mass;
        robotRigidbody.centerOfMass = com;
        centreOfMass = com;
    }

    // Configure axel height (vertical into robot) and position along z-axis (forward)
    public void ConfigureAxel(float axelHeight, float axelPos, AxelType type)
    {
        axel.localPosition = new Vector3(0f, axelHeight, axelPos);
    }

    public void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track, AxelType type)
    {
        Wheel leftWheel = wheelController.wheels[0];
        leftWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(-track, axel.localPosition.y, axel.localPosition.z);
        leftWheel.transform.localPosition = new Vector3(-track, 0f, 0f);
        leftWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
        leftWheel.encoderRate = ticksPerRev;
        leftWheel.maxSpeed = maxVel;
        leftWheel.diameter = diameter;

        Wheel rightWheel = wheelController.wheels[1];
        rightWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(track, axel.localPosition.y, axel.localPosition.z);
        rightWheel.transform.localPosition = new Vector3(track, 0f, 0f);
        rightWheel.transform.localScale = new Vector3(diameter, diameter, diameter);
        rightWheel.encoderRate = ticksPerRev;
        rightWheel.maxSpeed = maxVel;
        rightWheel.diameter = diameter;

        wheelController.wheelDist = Mathf.Abs(track * 2f);
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
        servoEnabled = true;
    }

    public void ConfigureLidar(int numPoints, int tilt)
    {
        laserScanController.numPoints = numPoints;
        laserScanController.rot = (float) -360.0 / numPoints;
        laserScanController.laserScanner.localRotation = Quaternion.Euler(new Vector3(-tilt, 0, 0));
    }

    /* ----- Driving Commands ----- */

    public void DriveDoneCallback()
    {
        driveDoneDelegate(myConnection);
    }

    public int GetEncoder(int quad)
    {
        return wheelController.GetEncoderTicks(quad);
    }

    public void DriveMotor(int motor, int speed)
    {
        wheelController.SetMotorSpeed(motor, speed);
    }

    public void DriveMotorControlled(int motor, int ticks)
    {
        wheelController.SetMotorControlled(motor, ticks);
    }

    public void SetPID(int motor, int p, int i, int d)
    {
        wheelController.SetPIDParams(motor, p, i, d);
    }

    public void SetServo(int servo, int angle)
    {
        servoController.SetServoPosition(servo, angle);
    }

    public void SetPose(int x, int y, int phi)
    {
        wheelController.SetPosition((int)x, (int)y, (int)phi);
    }

    public Int16[] GetPose()
    {
        Int16[] pos = new Int16[3];
        float[] robPos = wheelController.GetPosition();
        pos[0] = Convert.ToInt16(Math.Round(robPos[0] * Eyesim.Scale));
        pos[1] = Convert.ToInt16(Math.Round(robPos[1] * Eyesim.Scale));
        pos[2] = Convert.ToInt16(Math.Round(robPos[2]));
        return pos;
    }

    public UInt16 GetPSD(int psd)
    {
        if (psdEnabled)
            return psdController.GetPSDValue(psd);
        else
            return 0;
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

    public void VWSetVehicleSpeed(int linear, int angular)
    {
        wheelController.SetSpeedManual(linear / Eyesim.Scale, angular);
    }

    public Speed VWGetVehicleSpeed()
    {
        return wheelController.GetSpeed();
    }

    public void VWDriveStraight(int distance, int speed)
    {
        wheelController.DriveStraight((float)distance / Eyesim.Scale, (float)speed / Eyesim.Scale);
    }

    public void VWDriveTurn(int rotation, int velocity)
    {
        wheelController.DriveTurn(rotation, velocity);
    }

    public void VWDriveCurve(int distance, int rotation, int velocity)
    {
        wheelController.DriveCurve((float)distance / Eyesim.Scale, rotation, (float)velocity / Eyesim.Scale);
    }

    public int VWDriveRemaining()
    {
        return wheelController.DriveRemaining();
    }

    public bool VWDriveDone()
    {
        return wheelController.DriveDone();
    }

    public int VWDriveStalled()
    {
        throw new NotImplementedException();
    }

    public void VWDriveWait(Action<RobotConnection> doneCallback)
    {
        driveDoneDelegate = doneCallback;
        wheelController.DriveDoneDelegate = DriveDoneCallback;
    }

    public void ClearVWWait()
    {
        driveDoneDelegate = null;
        wheelController.DriveDoneDelegate = null;
    }

    public void RemoveVWOrigin()
    {
        Destroy(wheelController.VWOrigin.gameObject);
    }

    public void InitalizeVW(int[] args)
    {
        throw new NotImplementedException();
    }

    public bool VWAccurate
    {
        get
        {
            return wheelController.realCoords;
        }
        set
        {
            wheelController.realCoords = value;
        }
    }

    public byte[] GetCameraOutput(int camera)
    {
        if (camEnabled)
            return eyeCamController.GetBytes(camera);
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
            return new int[360];
    }
}