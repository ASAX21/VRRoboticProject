using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class Servo : MonoBehaviour
    {

        public float minAngle = -90f;
        public float maxAngle = 90f;
        public float desiredPosition = 0f;

        private HingeJoint hinge;
        private JointMotor motor;

        // Function to convert angles in range (0,360) to (-180,180)
        private float NegAngles(float val)
        {
            return val <= 180 ? val : val - 360;
        }

        // Fix the desired position to the minimum and maximum angle
        // This could be done with the hinge joint component itself
        public void SetPosition(int pos)
        {
            Debug.Log("Setting servo " + pos);
            desiredPosition = Mathf.Clamp((float)pos, minAngle, maxAngle);
        }

        private void Start()
        {
            hinge = GetComponent<HingeJoint>();
            motor = hinge.motor;
            motor.force = 100000;
            hinge.useMotor = true;
            motor.targetVelocity = 0;
            motor.freeSpin = false;
        }

        // Could add another check to do nothing if no rotation required AND motor is off
        // depends on performance of modifying hinge motor
        public void FixedUpdate()
        {
            if (NegAngles(transform.localRotation.eulerAngles.y) < desiredPosition - 0.5f)
            {
                motor.targetVelocity = 30;
                hinge.motor = motor;
            }
            else if (NegAngles(transform.localRotation.eulerAngles.y) > desiredPosition + 0.5f)
            {
                motor.targetVelocity = -30;
                hinge.motor = motor;
            }
            else
            {
                motor.targetVelocity = 0;
                hinge.motor = motor;
            }
        }
    }
}