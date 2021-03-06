﻿using System;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class DiffDriveRoBIOS : Robot, 
    IMotors,
    IPIDUsable,
    IPSDSensors,
    IServos,
    IVWDrive,
    ICameras,
    IAudio,
    IRadio,
    ILaser
{
    // Controllers
    [Header("Controller References")]
    public WheelMotorController wheelController;
    public PSDController psdController;
    public ServoController servoController;
    public EyeCameraController eyeCamController;
    public AudioController audioController;
    public RadioController radioController;
    public LaserScanController laserScanController;

    Action<RobotConnection> driveDoneDelegate;
    Action<RobotConnection, byte[]> radioMessageDelegate;

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
        psdController.TriggerPSDPulse(psd);
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
        laserScanController.showRaycast = val;
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
        wheelController.DriveStraight( distance / Eyesim.Scale, speed / Eyesim.Scale);     
    }

    public void VWDriveTurn(int rotation, int velocity)
    {
		wheelController.DriveTurn (rotation, velocity);
    }

    public void VWDriveCurve(int distance, int rotation, int velocity)
    {
		wheelController.DriveCurve (distance/Eyesim.Scale, rotation, velocity/Eyesim.Scale);
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
        return;
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
        return eyeCamController.GetBytes(camera);
    }

    public void SetCameraResolution(int camera, int width, int height)
    {
        eyeCamController.SetResolution(camera, width, height);
        if(myWindow != null)
            myWindow.UpdateCameraTarget();
    }

    public string GetCameraResolution(int camera)
    {
        return eyeCamController.GetResolution(camera);
    }

    public EyeCamera GetCameraComponent(int camera)
    {
        return eyeCamController.cameras[camera];
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
        return laserScanController.Scan();
    }
}