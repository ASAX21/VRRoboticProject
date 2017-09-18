using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class PSDSensor : MonoBehaviour
    {
        public float value = 0;

        // Values for error
        public bool errorEnabled = false;
        public float normalMean = 0f;
        public float normalStdDev = 10f;

        // Accessors for the error variables
        public void SetErrorMean(float mean)
        {
            normalMean = mean;
        }

        public void SetErrorStdDev(float dev)
        {
            normalStdDev = dev;
        }

        // Box-Muller Implementation for Random Normal number
        private float GetRandomError()
        {
            float u1 = 1f - Random.value;
            float u2 = 1f - Random.value;
            float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
            return normalMean + normalStdDev * randStdNormal;
        }
        
        public float GetSensorValue()
        {
            float val = value;
            if (errorEnabled)
                val += GetRandomError();
            return Mathf.Clamp(val, 0.1f, 9999f);
        }

        // Calculate sensor values at each frame
        void FixedUpdate()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
			Debug.DrawRay (transform.position, forward,Color.green);
            if (Physics.Raycast(transform.position, forward, out hit, 2000))
                value = hit.distance * 1000;
            else
			    value = 9999f;
        }
    }
}
