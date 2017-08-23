using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class WheelMotorController : MonoBehaviour
{
    public List<Wheel> wheels; // the information about each individual wheel  
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float wheelDist;
    public float Rot;
    public float w;
    public Vector3 Pos;
    public float v;

    //vw parameters
    [Header("VW Parameters")]
    public float targetDist;
    public float travelledDist;
    public float targetRot;
    public float travelledRot;
    public string checkType;
    public bool checkActive;

    // [Header("Max Speeds")]
    private float maxStraightSpeed = 0.5f;
    private float maxTurnSpeed = 90f;
    private float vSpeed = 0f;
    private float wSpeed = 0f;

    [HideInInspector]
    public Action DriveDoneDelegate;

    private void Awake()
    {
        Pos = new Vector3(0,0,0);
    }

    private void Update()
    {
        if (DriveDoneDelegate != null && !checkActive)
        {
            DriveDoneDelegate();
            DriveDoneDelegate = null;
        }
    }

    // Set the local PID Parameters
    public void SetPIDParams(int motor, int p, int i, int d)
    {
        wheels[motor].SetPIDParams(p, i, d);
    }
    // Set the speed of a single motor
    public void SetMotorSpeed(int motor, float speed)
    {
        wheels[motor].SetMotorSpeed(speed);
    }

    // Set the speed of a single motor (controlled)
    public void SetMotorControlled(int motor, int ticks)
    {
        if (!wheels[motor].pidEnabled)
        {
            SetPIDParams(motor, 4, 1, 1);
        }

        SetMotorSpeed(motor, ticks);
    }
    // Update visual of wheel on each frame
    private void FixedUpdate()
    {
        updatePosition();
        checkDrive();
    }

    // Distance determines direction, always use absolute value of velocity
    public void DriveStraight(float distance, float velocity)
    {
        resetController();
        targetDist = distance;
        checkType = "distance";
        if (distance >= 0)
            SetSpeed(Mathf.Abs(velocity), 0);
        else
            SetSpeed(-Mathf.Abs(velocity), 0);
        checkActive = true;
    }

    // Positive rotation - anti-clockwise
    public void DriveTurn(float rotation, float velocity)
    {
        Debug.Log("TURN: " + rotation + " " + velocity);
        resetController();
        targetRot = rotation;
        checkType = "rotation";
        if(rotation >= 0)
            SetSpeed(0, Mathf.Abs(velocity));
        else
            SetSpeed(0, -Mathf.Abs(velocity));
        checkActive = true;
    }

    public void DriveCurve(float distance, float rotation, float velocity)
    {
        resetController();
        targetDist = distance;
        checkType = "distance";
        SetSpeed(velocity, (float)velocity / distance * rotation); //finds w via ratio
        checkActive = true;
    }

    public int DriveRemaining()
    {
        return Convert.ToInt32(targetDist - travelledDist);
    }

    public bool DriveDone()
    {
        return !checkActive;
    }

    //set translational and rotational target velocities
    public void SetSpeed(float setv, float setw)
    {
        vSpeed = Mathf.Clamp(setv, -maxStraightSpeed, maxStraightSpeed);
        wSpeed = Mathf.Clamp(setw, -maxTurnSpeed, maxTurnSpeed);
        wheels[0].SetSpeed(vSpeed - wSpeed * wheelDist / 2 * Mathf.Deg2Rad);
        wheels[1].SetSpeed(vSpeed + wSpeed * wheelDist / 2 * Mathf.Deg2Rad);
    }

    public Speed GetSpeed()
    {
        return new Speed(v * 1000000, w * 1000);
    }

    public void SetPosition(float x, float y, float phi)
    {
        Pos.x = y;
        Pos.z = x;
        Rot = phi;
    }

    public float[] GetPosition()
    {
        return new float[3] { Pos.z, Pos.x, Rot };
    }

    private void updatePosition()
    {
        float lspeed = wheels[0].GetSpeed();
        float rspeed = wheels[1].GetSpeed();
        float newv = (rspeed + lspeed) / 2;
        float neww = (lspeed - rspeed) / wheelDist * Mathf.Rad2Deg;
        Pos.z += ((newv + v) / 2) * Mathf.Cos(Mathf.Deg2Rad * Rot);
        Pos.x += ((newv + v) / 2) * Mathf.Sin(Mathf.Deg2Rad * Rot);
        Rot += (neww + w) / 2;

        while (Rot > 180)
        {
            Rot -= 360;
        }
        while (Rot < -180)
        {
            Rot += 360;
        }

        v = newv;
        w = neww;
    }

    private void resetController()
    {
        targetDist = 0;
        targetRot = 0;
        travelledDist = 0;
        travelledRot = 0;
    }

    private void checkDrive()
    {
        if (!checkActive)
            return;

        switch (checkType)
        {
            case "distance":
                travelledDist += v;
                if (Mathf.Sign(targetDist) * (targetDist - travelledDist) > 0)
                    return;
                break;
            case "rotation":
                travelledRot -= w;
                if (Mathf.Sign(targetRot) * (targetRot - travelledRot - Mathf.Abs(wSpeed)/20f) > 0)
                    return;
                break;
            default:
                break;
        }

        //journey complete
        Debug.Log("Drive Done");
        SetSpeed(0, 0);
        checkActive = false;
        resetController();
    }
}