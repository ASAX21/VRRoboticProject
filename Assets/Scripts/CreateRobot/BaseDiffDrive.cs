using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class BaseDiffDrive : Robot, IMotors,
    IPIDUsable,
    IPSDSensors,
    IServoSettable,
    IVWDrivable,
    ICameras,
    IAudio,
    IRadio,
    ILaser
{
    // Components of the robot
    public BoxCollider robotBody;
    public CapsuleCollider robotBunt;
    public GameObject robotModel;

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

    public void TEST()
    {
        //   PSD_FRONT 1 30 0 80 0
        Vector3 p = new Vector3(30 / 1000f, 50/1000f, 80 / 1000f);
        AddPSDSensor(1, "PSD_FRONT", p, 0f);
    }

    public void TEST2()
    {
        ConfigureCamera(new Vector3(0, 50/1000f, 70/ 1000f), 0f, 0f, 90f, 90f);
    }

    // Configure size of robot - single box collider, and position of the slider located at the back
    public void ConfigureBotSize(float length, float width, float height)
    {
        robotBody.size = new Vector3(width / 1000f, height / 1000f, length / 1000f);
        robotBunt.center = new Vector3(0f, 0.025f, 0.5f * length / 1000f * 0.6f);
    }

    // Configure axel height (vertical into robot) and position along z-axis (forward)
    public void ConfigureAxel(float axelHeight, float axelPos)
    {
        axel.localPosition = new Vector3(0f, axelHeight / 1000f, axelPos / 1000f);
    }

    public void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track)
    {
        Wheel leftWheel = wheelController.wheels[0];
        leftWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(-track, axel.localPosition.y, axel.localPosition.z);
        leftWheel.transform.localPosition = new Vector3(-track, 0f, 0f);
        leftWheel.transform.localScale = new Vector3(diameter / 1000f, diameter / 1000f, diameter / 1000f);
        leftWheel.encoderRate = ticksPerRev;
        leftWheel.maxSpeed = maxVel;

        Wheel rightWheel = wheelController.wheels[1];
        rightWheel.GetComponent<HingeJoint>().connectedAnchor = new Vector3(track, axel.localPosition.y, axel.localPosition.z);
        rightWheel.transform.localPosition = new Vector3(track, 0f, 0f);
        rightWheel.transform.localScale = new Vector3(diameter / 1000f, diameter / 1000f, diameter / 1000f);
        rightWheel.encoderRate = ticksPerRev;
        rightWheel.maxSpeed = maxVel;
    }

    public bool AddPSDSensor(int id, string name, Vector3 pos, float rot)
    {
        if(id != psdController.sensors.Count + 1)
        {
            Debug.Log("Expected incrementing IDs");
            return false;
        }
        GameObject newPSD = Instantiate(PSDPrefab, PSDContainer);
        newPSD.name = name;
        newPSD.transform.localPosition = pos;
        newPSD.transform.localEulerAngles = new Vector3(0f, -rot, 0f);
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
        pos[0] = Convert.ToInt16(Math.Round(robPos[0] * 1000));
        pos[1] = Convert.ToInt16(Math.Round(robPos[1] * 1000));
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

    public void VWSetVehicleSpeed(int linear, int angular)
    {
        wheelController.SetSpeedManual(linear / 1000.0f, angular);
    }

    public Speed VWGetVehicleSpeed()
    {
        return wheelController.GetSpeed();
    }

    public void VWDriveStraight(int distance, int speed)
    {
        wheelController.DriveStraight((float)distance / 1000, (float)speed / 1000);
    }

    public void VWDriveTurn(int rotation, int velocity)
    {
        wheelController.DriveTurn(rotation, velocity);
    }

    public void VWDriveCurve(int distance, int rotation, int velocity)
    {
        wheelController.DriveCurve((float)distance / 1000, rotation, (float)velocity / 1000);
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

    public void SetCameraResolution(int camera, int width, int height)
    {
        if(camEnabled)
            eyeCamController.SetResolution(camera, width, height);
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
