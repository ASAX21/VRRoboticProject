using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotComponents
{
    public class Servo : MonoBehaviour
    {
       public enum axis {x ,y ,z };

        public float minAngle = -90f;
        public float maxAngle = 90f;
        public float desiredPosition = 0f;

        public axis aroundAxis;

        private HingeJoint hinge;
        private JointMotor motor;

        // Function to convert angles in range (0,360) to (-180,180)
        private float NegAngles(float val)
        {
            return val <= 180 ? val : val - 360;
        }

        // Fix the desired position to the minimum and maximum angle
        // Input given 0 - 255, 0 is far left, 255 is far right
        public void SetPosition(int pos)
        {
            desiredPosition = (pos - 128f)/128 * maxAngle;
        }

        private void Start()
        {
            hinge = GetComponent<HingeJoint>();
            motor = hinge.motor;
            motor.force = 100000;
            hinge.useMotor = true;
            motor.targetVelocity = 0;
            motor.freeSpin = true;
        }

        // Could add another check to do nothing if no rotation required AND motor is off
        // depends on performance of modifying hinge motor
        public void FixedUpdate()
        {
            if (NegAngles(transform.localRotation.eulerAngles[(int)aroundAxis]) < desiredPosition - 0.5f)
            {
                motor.targetVelocity = 30;
                hinge.motor = motor;
            }
            else if (NegAngles(transform.localRotation.eulerAngles[(int)aroundAxis]) > desiredPosition + 0.5f)
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