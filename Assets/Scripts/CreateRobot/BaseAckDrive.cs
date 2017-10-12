﻿using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class BaseAckDrive : Robot, IMotors,
    IPIDUsable,
    IPSDSensors,
    IServoSettable,
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

    public Transform axelTurn;
    public Transform axelDrive;

    public Transform PSDContainer;
    public GameObject PSDPrefab;

    // EyeCam position is the top level servo (ServoPan)
    public HingeJoint eyeCamPosition;

    // Controllers
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

    public void TEST()
    {
        SetServo(0, 0);
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

    }

    public void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track)
    {

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
        return 0;
    }

    public void DriveMotor(int motor, int speed)
    {
        wheelController.SetMotorSpeed(motor, speed);
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
        if (servo == 0)
            wheelController.SetTurnAngle(angle);
        else
            servoController.SetServoPosition(servo-1, angle);
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