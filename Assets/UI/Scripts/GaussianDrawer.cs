using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class GaussianDrawer : MonoBehaviour {

    public UILineRenderer rend;
    public int numPoints;

    // Calculate value of gaussian
    private float Gaussian(float mean, float sdev, float x)
    {
        float val = (1 / Mathf.Sqrt(2 * Mathf.PI * sdev * sdev)) * Mathf.Exp(-(x - mean) * (x - mean) / (2 * sdev * sdev));
        Debug.Log("x: " + x + "   val: " + val);
        return val;
    }

    // Calculate 2d points (Uses relative positioning)
    public void DrawPSDGaussian(float mean, float sdev)
    {
        Vector2[] pts = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float xpos = i / (float)(numPoints - 1);
            float xval = i * (8f * sdev / (numPoints - 1f)) - 4f * sdev;
            pts[i] = new Vector2(xpos, Gaussian(mean, sdev, xval));
        }

        rend.Points = pts;
    }
}
