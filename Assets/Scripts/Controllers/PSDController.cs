using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;
using RobotCommands;

public class PSDController : MonoBehaviour {

    public List<PSDSensor> sensors;

    // Values for error
    public bool errorEnabled = false;
    public bool useGlobalError = true;

    public static float globalMean = 0f;
    public static float globalStdDev = 10f;
    
    public float normalMean = 0f;
    public float normalStdDev = 10f;

    // Accessors for the error variables
    public void SetErrorMean(float mean)
    {
        normalMean = mean;
    }

    public void SetErrorStdDev(float dev)
    {
        normalStdDev = dev;
    }

    public void VisualiseAllSensors(bool val)
    {
        foreach (PSDSensor psd in sensors)
            psd.EnableVisualise(val);
    }

    // Box-Muller Implementation for Random Normal number
    private float GetRandomError()
    {
        float u1 = 1f - UnityEngine.Random.value;
        float u2 = 1f - UnityEngine.Random.value;
        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        if (useGlobalError)
            return globalMean + globalStdDev * randStdNormal;
        else
            return normalMean + normalStdDev * randStdNormal;
    }

    public UInt16 GetPSDValue(int psd)
    {
        if (psd >= sensors.Count)
            return 0;

        float val = sensors[psd].value;
        if (errorEnabled)
            val += GetRandomError();
        return Convert.ToUInt16(Mathf.Clamp(val, 0.1f, 9999f));
    }
}
