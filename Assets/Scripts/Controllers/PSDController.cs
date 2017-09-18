using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;
using RobotCommands;

public class PSDController : MonoBehaviour {

    public List<PSDSensor> sensors;

    public UInt16 GetPSDValue(int psd)
    {
        if (psd >= sensors.Count)
            return 0;

        return Convert.ToUInt16(sensors[psd].GetSensorValue());
    }
}
