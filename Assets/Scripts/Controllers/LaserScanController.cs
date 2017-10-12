using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScanController : MonoBehaviour {

    public Transform laserScanner;
    public Transform robot;

    // Do a 360 scan , determine distance in one degree increments
    // 0 degrees is in the direction of the robot, then rotate LEFT
    public int[] Scan()
    {
        int[] dists = new int[360];
        laserScanner.rotation = Quaternion.Euler(0, robot.eulerAngles.y, 0);

        for(int i = 0; i < 360; i++)
        {
            laserScanner.Rotate(laserScanner.up, -1.0f);
            Vector3 forward = laserScanner.TransformDirection(Vector3.forward);
            RaycastHit hit;
            Debug.DrawRay(laserScanner.position, forward, Color.green);
            if (Physics.Raycast(transform.position, forward, out hit, 2000))
            {
                dists[i] = Mathf.FloorToInt(hit.distance * Eyesim.Scale);
                dists[i] = dists[i] > 9999 ? 9999 : dists[i];
            }
            else
                dists[i] = 9999;
        }
        return dists;
    }
}
