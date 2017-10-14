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
        byte[] image = cameras[camera].GetBytes();
        if (useSNPNoise)
        {
            for (int i = 0; i < cameras[camera].resWidth; i++)
            {
                for (int j = 0; j < cameras[camera].resHeight; j++)
                {
                    if (UnityEngine.Random.value < saltPepperPercent)
                    {
                        if (UnityEngine.Random.value > saltPepperRatio)
                        {
                            image[(j * cameras[camera].resWidth + i) * 3] = 0xFF;
                            image[(j * cameras[camera].resWidth + i) * 3 + 1] = 0xFF;
                            image[(j * cameras[camera].resWidth + i) * 3 + 2] = 0xFF;
                        }
                        else
                        {
                            image[(j * cameras[camera].resWidth + i) * 3] = 0x00;
                            image[(j * cameras[camera].resWidth + i) * 3 + 1] = 0x00;
                            image[(j * cameras[camera].resWidth + i) * 3 + 2] = 0x00;
                        }
                    }
                }
            }
        }
        return image;
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
