using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class PSDSensor : MonoBehaviour
    {
        public float value = 0;
        public LineRenderer lineRend;
        public bool showRaycast = false;

        public void EnableVisualise(bool val)
        {
            showRaycast = val;
            lineRend.enabled = val;
        }

        // Calculate sensor values at each frame
        void FixedUpdate()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
			Debug.DrawRay (transform.position, forward,Color.green);
            if (Physics.Raycast(transform.position, forward, out hit, 2000))
            {
                value = hit.distance * 1000;
                if (showRaycast)
                    lineRend.SetPosition(1, Vector3.forward * hit.distance);
            }
            else
            {
                value = 9999f;
                lineRend.SetPosition(1, Vector3.forward * 10);
            }
        }
    }
}
