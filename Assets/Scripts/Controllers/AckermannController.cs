using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;

public class AckermannController : MonoBehaviour
{
    public Robot mainBot;
    public List<Wheel> driveWheels;
    public List<Wheel> turnWheels;  
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float wheelDist;
    public float Rot;
    public float w;
    public Vector3 Pos;
    public float v;

    // Latest SetPosition data
    public bool realCoords = false;

    // [Header("Max Speeds")]
    private float maxStraightSpeed = 0.5f;
    private float maxTurnSpeed = 90f;
    private float vSpeed = 0f;

    public float turnRate = 15f;
    private float turnAngle = 0f;
    private float targetTurnAngle = 0f;

    public int GetEncoderValue(int motor)
    {
        if (motor < 0)
        {
            Debug.Log("Get Encoder: Bad value");
            return 0;
        }
        else if (motor < driveWheels.Count)
            return (int)driveWheels[motor].ticks;
        else if (driveWheels.Count <= motor && (motor - driveWheels.Count) < turnWheels.Count)
            return (int)turnWheels[motor - driveWheels.Count].ticks;
        else
        {
            Debug.Log("Encoder: Bad motor value");
            return 0;
        }
    }

    // Set the local PID Parameters
    public void SetPIDParams(int motor, int p, int i, int d)
    {
        // None
    }

    public void SetTurnAngle(int angle)
    {
        targetTurnAngle = angle;
    }
    // Set the speed of a single motor
    public void SetMotorSpeed(int motor, int speed)
    {
        int factor = Eyesim.ClampInt(speed, -100, 100);
        float vSpeed = factor / 100f * maxMotorTorque;
        if (motor > driveWheels.Count || motor < 0)
        {
            Debug.Log("SetMotorSpeed: Bad motor input");
            return;
        }
        driveWheels[motor].SetSpeed(vSpeed);
    }

    // Update visual of wheel on each frame
    private void FixedUpdate()
    {
        updatePosition();

        //if(turnAngle < targetTurnAngle - 0.1f || turnAngle > targetTurnAngle + 0.1f)
        //{
        //    float step = turnRate * Time.fixedDeltaTime;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        turnWheels[i].transform.rotation = Quaternion.RotateTowards(turnWheels[i].transform.rotation, 
        //    }
        //}
    }

    //set translational and rotational target velocities
    public void SetDriveSpeed(float vel)
    {
        vSpeed = Mathf.Clamp(vel, -maxStraightSpeed, maxStraightSpeed);
        driveWheels[0].SetSpeed(vSpeed);
        driveWheels[1].SetSpeed(vSpeed);
    }

    public void SetTurnAngle(float angle)
    {
        turnAngle = Mathf.Clamp(angle, -maxSteeringAngle, maxSteeringAngle);
        turnWheels[0].SetAngle(angle);
        turnWheels[1].SetAngle(angle);
    }

    private void updatePosition()
    {
        float lspeed = driveWheels[0].GetSpeed();
        float rspeed = driveWheels[1].GetSpeed();
        float newv = (rspeed + lspeed) / 2;
        float neww = (lspeed - rspeed) / wheelDist * Mathf.Rad2Deg;
        Pos.z += ((newv + v) / 2) * Mathf.Cos(Mathf.Deg2Rad * Rot);
        Pos.x += ((newv + v) / 2) * Mathf.Sin(Mathf.Deg2Rad * Rot);
        Rot -= (neww + w) / 2;

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
}