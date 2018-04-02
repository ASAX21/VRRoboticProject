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
    // Use global or loca values for calculating
    public bool useGlobalError = true;

    private static float pulseTime = 0.5f;
    public bool showRaycast = false;

    // If error enabled is false, no error will be added
    // regardless of useGlobalError value

    public static float globalMean = 0f;
    public static float globalStdDev = 10f;
    
    public float normalMean = 0f;
    public float normalStdDev = 10f;

    private void Start()
    {
        VisualiseAllSensors(SimManager.instance.defaultVis);
    }

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
        showRaycast = val;
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

    // Trigger the visualization of psd sensor
    public void TriggerPSDPulse(int psd)
    {
        if (showRaycast)
            sensors[psd].visTime = pulseTime;
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
