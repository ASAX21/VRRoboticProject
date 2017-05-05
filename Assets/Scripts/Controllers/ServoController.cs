﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;
using RobotCommands;

public class ServoController : MonoBehaviour {

    public List<Servo> servos;

    public void SetServoPosition(int servo, int pos)
    {
        servos[servo].SetPosition(pos);
    }
}
