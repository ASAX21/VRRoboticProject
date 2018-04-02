using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScanController : MonoBehaviour
{
    public Transform laserScanner;
    public Transform robot;

    [SerializeField]
    private int numPoints = 360;
    [SerializeField]
    private int angRange = 360;
    [SerializeField]
    private float rot = 1.0f;
    [SerializeField]
    private float startRot = -180.0f;
    public LayerMask mask;

    public LineRenderer lineRend;
    public bool showRaycast = false;
    private float visTime = 0f;

    private void Start()
    {
        showRaycast = SimManager.instance.defaultVis;
    }

    private void Update()
    {
        if (visTime > 0 && !lineRend.enabled)
            lineRend.enabled = true;
        else if (visTime <= float.Epsilon && lineRend.enabled)
            lineRend.enabled = false;
        else if (visTime > float.Epsilon)
            visTime -= Time.deltaTime;
    }

    // Centre is always middle point
    public void SetAngularRange(int range, int points)
    {
        angRange = range;
        numPoints = points;
        rot = (float) angRange / numPoints;
        startRot = -(angRange / 2f);
        lineRend.positionCount = numPoints * 2;
    }
    
    // Scan from left to right, with dists[numPoints/2] being directly infront of the robot
    public int[] Scan()
    {
        if(showRaycast)
            visTime = 0.5f;

        int[] dists = new int[numPoints];
        laserScanner.rotation = Quaternion.Euler(0, startRot, 0);
        for(int i = 0; i < numPoints; i++)
        {
            laserScanner.Rotate(laserScanner.up, rot);
            Vector3 fwd = laserScanner.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(laserScanner.position, fwd, out hit, 2000, mask))
            {
                dists[i] = Mathf.FloorToInt(hit.distance * Eyesim.Scale);
                dists[i] = dists[i] > 9999 ? 9999 : dists[i];
                if (showRaycast)
                    lineRend.SetPosition(2 * i + 1, laserScanner.forward * hit.distance);
            }
            else
            {
                dists[i] = 9999;
                lineRend.SetPosition(2 * i + 1, laserScanner.forward * 10);
            }
        }
        return dists;
    }
}
