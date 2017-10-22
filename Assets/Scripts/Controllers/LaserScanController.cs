using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScanController : MonoBehaviour {

    public Transform laserScanner;
    public Transform robot;

    public LayerMask mask;

    public LineRenderer lineRend;

    public bool showRaycast = false;
    private float visTime = 0f;

    private void Update()
    {
        if (visTime > 0 && !lineRend.enabled)
            lineRend.enabled = true;
        else if (visTime <= float.Epsilon && lineRend.enabled)
            lineRend.enabled = false;
        else if (visTime > float.Epsilon)
            visTime -= Time.deltaTime;
    }

    // Do a 360 scan , determine distance in one degree increments
    // 0 degrees is in the direction of the robot, then rotate LEFT
    public int[] Scan()
    {
        if(showRaycast)
            visTime = 0.5f;

        int[] dists = new int[360];
        laserScanner.rotation = Quaternion.Euler(0, robot.eulerAngles.y, 0);
        for(int i = 0; i < 360; i++)
        {
            laserScanner.Rotate(laserScanner.up, -1.0f);
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
