﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using RobotComponents;

public interface IMotors
{
    void DriveMotor(int motor, int speed);
}

public interface IPIDUsable
{
    void DriveMotorControlled(int motor, int ticks);
    void SetPID(int motor, int p, int i, int d);
}
    
public interface IVWDrivable
{
    void InitalizeVW(int[] args);
    Int16[] GetPose();
    void SetPose(int x, int y, int phi);
    void VWSetVehicleSpeed(int linear, int angular);
    Speed VWGetVehicleSpeed();
    void VWDriveStraight(int distance, int speed);
    void VWDriveTurn(int rotation, int velocity);
    void VWDriveCurve(int distance, int rotation , int velocity);
    int VWDriveRemaining();
    bool VWDriveDone();
    int VWDriveStalled();
    void VWDriveWait(Action<RobotConnection> doneCallback);
}

public interface IServoSettable
{
    void SetServo(int servo, int angle);
}

public interface IPSDSensors
{
    UInt16 GetPSD(int psd);
}

public interface ICameras
{
    byte[] GetCameraOutput(int camera);
    void SetCameraResolution(int camera, int width, int height);
    EyeCamera GetCameraComponent(int camera);
}
// Class used to pass data back to client
public class Speed
{
    public Int16 linear;
    public Int16 angular;

    public Speed(float lin, float ang)
    {
        linear = Convert.ToInt16(lin);
        angular = Convert.ToInt16(ang);
    }
}
// Abstract robot
// Universal functions
public abstract class Robot : PlaceableObject, IPointerClickHandler
{
    public int axels = 0;
    public RobotConnection myConnection = null;

    override public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount > 1 && !isWindowOpen)
        { 
            isWindowOpen = true;
            objectSelector.DisplayRobotInfoWindow(this);
        }
        base.OnPointerClick(eventData);
    }
}
