using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class LabBot : Robot, 
    IMotors,
    IPIDUsable,
    IPSDSensors,
    IServoSettable,
    IVWDrivable,
    ICameras,
    IAudio,
    IRadio,
    ILaser
{
    // Controllers
    public WheelMotorController wheelController;
    public PSDController psdController;
    public ServoController servoController;
    public EyeCameraController eyeCamController;
    public AudioController audioController;
    public RadioController radioController;
    public LaserScanController laserScanController;

    Action<RobotConnection> driveDoneDelegate;
    Action<RobotConnection, byte[]> radioMessageDelegate;

    // This function sets the controllers for a newly created LabBot object
    // Used when a robot is created from file
    public void Initialize()
    {
        wheelController = gameObject.AddComponent<WheelMotorController>();
        psdController = gameObject.AddComponent<PSDController>();
        servoController = gameObject.AddComponent<ServoController>();
        eyeCamController = gameObject.AddComponent<EyeCameraController>();
        audioController = gameObject.AddComponent<AudioController>();
        radioController = gameObject.AddComponent<RadioController>();
        laserScanController = gameObject.AddComponent<LaserScanController>();

        wheelController.wheels = new List<Wheel>();
        psdController.sensors = new List<PSDSensor>();
        servoController.servos = new List<Servo>();
        eyeCamController.cameras = new List<EyeCamera>();
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
        wheelController.SetPosition(x, y, phi);
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
        return psdController.GetPSDValue(psd);
    }

    public void VWSetVehicleSpeed(int linear, int angular)
    {
        wheelController.SetSpeed(linear/1000.0f, angular);
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
		wheelController.DriveTurn (rotation, velocity);
    }

    public void VWDriveCurve(int distance, int rotation, int velocity)
    {
		wheelController.DriveCurve ((float) distance/1000, rotation, (float) velocity/1000);
    }

    public int VWDriveRemaining()
    {
        return wheelController.DriveRemaining();
    }

    public bool VWDriveDone()
    {
		return wheelController.DriveDone ();
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
        return eyeCamController.GetBytes(camera);
    }

    public void SetCameraResolution(int camera, int width, int height)
    {
        eyeCamController.SetResolution(camera, width, height);
    }

    public EyeCamera GetCameraComponent(int camera)
    {
        return eyeCamController.cameras[camera];
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
        return laserScanController.Scan();
    }
}