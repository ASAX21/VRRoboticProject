using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class PSDSensor : MonoBehaviour
    {

        public float value = 0;

        // Calculate sensor values at each frame
        void FixedUpdate()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
			Debug.DrawRay (transform.position, forward,Color.green);
            if (Physics.Raycast(transform.position, forward, out hit, 2000))
            {
                value = hit.distance * 1000;
                value = value > 10000f ? 9999f : value;
    			return;
            }
			value = 9999f;
        }
    }
}
