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
    public BoxCollider robotBody;
    public CapsuleCollider robotBunt;
    public GameObject robotModel;
    public Rigidbody robotRigidbody;

    public Transform axel;

    public Transform PSDContainer;
    public GameObject PSDPrefab;

    // EyeCam position is the top level servo (ServoPan)
    public HingeJoint eyeCamPosition;

    // Controllers
    public WheelMotorController wheelController;
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
    public void TEST()
    {
        //   PSD_FRONT 1 30 0 80 0
        Vector3 p = new Vector3(30 / Eyesim.Scale, 50/Eyesim.Scale, 80 / Eyesim.Scale);
        AddPSDSensor(1, "PSD_FRONT", p, 0f);
    }

    public void TEST2()
    {
        ConfigureCamera(new Vector3(0, 50/Eyesim.Scale, 70/ Eyesim.Scale), 0f, 0f, 90f, 90f);
    }

    // Configure size of robot - single box collider, and position of the slider located at the back
    public void ConfigureSize(float length, float width, float height)
    {
        robotBody.size = new Vector3(width / Eyesim.Scale, height / Eyesim.Scale, length / Eyesim.Scale);
        robotBunt.center = new Vector3(0f, 0.025f, 0.5f * length / Eyesim.Scale * 0.6f);
    }

    public void ConfigureMass(float mass, Vector3 com)
    {
        robotRigidbody.mass = mass;
        robotRigidbody.centerOfMass = com;
    }

    // Configure axel height (vertical into robot) and position along z-axis (forward)
    public void ConfigureAxel(float axelHeight, float axelPos, AxelType type)
    {
        axel.localPosition = new Vector3(0f, axelHeight / Eyesim.Scale, axelPos / Eyesim.Scale);
    }

    public void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track)
    {
        Wheel leftWheel = wheelController.wheels[0];
        leftWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(-track / Eyesim.Scale, axel.localPosition.y, axel.localPosition.z);
        leftWheel.transform.localPosition = new Vector3(-track / Eyesim.Scale, 0f, 0f);
        leftWheel.transform.localScale = new Vector3(diameter / Eyesim.Scale, diameter / Eyesim.Scale, diameter / Eyesim.Scale);
        leftWheel.encoderRate = ticksPerRev;
        leftWheel.maxSpeed = maxVel;

        Wheel rightWheel = wheelController.wheels[1];
        rightWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(track / Eyesim.Scale, axel.localPosition.y, axel.localPosition.z);
        rightWheel.transform.localPosition = new Vector3(track / Eyesim.Scale, 0f, 0f);
        rightWheel.transform.localScale = new Vector3(diameter / Eyesim.Scale, diameter / Eyesim.Scale, diameter / Eyesim.Scale);
        rightWheel.encoderRate = ticksPerRev;
        rightWheel.maxSpeed = maxVel;
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
            return null;
    }
}