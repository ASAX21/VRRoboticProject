using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RobotComponents;
using RobotCommands;

public class EyeCameraController : MonoBehaviour {

    public List<EyeCamera> cameras;

	public byte[] GetBytes(int camera)
    {   
		return cameras [camera].GetBytes ();
    }

    public void SetResolution(int camera, int width, int height)
    {
        Debug.Log("Set Resolution: " + width + " " + height);
        cameras[camera].SetResolution(width, height);
    }
}
