using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScanController : MonoBehaviour {

    public Transform laserScanner;

    // Do a 360 scan , determine distance in one degree increments
    public int[] Scan()
    {
        int[] dists = new int[360];
        laserScanner.rotation = Quaternion.identity;

        for(int i = 0; i < 360; i++)
        {
            laserScanner.rotation = Quaternion.Euler(new Vector3(0, -i, 0));
            Vector3 forward = laserScanner.TransformDirection(Vector3.forward);
            RaycastHit hit;
            Debug.DrawRay(laserScanner.position, forward, Color.green);
            if (Physics.Raycast(transform.position, forward, out hit, 2000))
                dists[i] = Mathf.FloorToInt(hit.distance * 1000);
            else
                dists[i] = -1;
        }
        return dists;
    }
}
