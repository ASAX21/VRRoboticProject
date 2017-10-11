using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class PSDSensor : MonoBehaviour
    {
        public float value = 0;
        public float visTime = 0f;
        public LineRenderer lineRend;
        public LayerMask mask;

        private void Update()
        {
            if (visTime > 0 && !lineRend.enabled)
                lineRend.enabled = true;
            else if (visTime <= float.Epsilon && lineRend.enabled)
                lineRend.enabled = false;
            else if (visTime > float.Epsilon)
                visTime -= Time.deltaTime;
        }

        // Calculate sensor values at each frame
        void FixedUpdate()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, forward, out hit, 2000, mask))
            {
                value = hit.distance * 1000;
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
