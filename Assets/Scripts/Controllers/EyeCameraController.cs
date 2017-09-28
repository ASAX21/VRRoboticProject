using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;
using RobotCommands;

public class EyeCameraController : MonoBehaviour {

    public List<EyeCamera> cameras;

    public bool useSNPNoise;
    public bool useGaussNoise;
    public float saltPepperPercent;
    public float saltPepperRatio;
    public float gaussMean;
    public float gaussStdDev;

	public byte[] GetBytes(int camera)
    {   
		return cameras [camera].GetBytes ();
    }

    public void SetResolution(int camera, int width, int height)
    {
        Debug.Log("Set Resolution: " + width + " " + height);
        cameras[camera].SetResolution(width, height);
    }

    // Returns a string of Width x Height
    public string GetResolution(int camera)
    {
        return cameras[camera].GetResolution();
    }
}
