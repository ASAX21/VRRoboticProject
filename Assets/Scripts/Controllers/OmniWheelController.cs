using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniWheelController : MonoBehaviour
{

    public float wheelTrack;    // Length of the axel
    public float wheelBase;     // Distance between axels
    public float wheelRadius;
    private Matrix4x4 omniMatrix;

    public int ticksPerRev;

    // Wheel rotational velocities
    public float FL, FR, BL, BR;
    // Wheel encoder values;
    public int EFL, EFR, EBL, EBR;
    // Maximum motor power
    [SerializeField]
    private float maxSpeed = 200f;
    // Multiplier for translational force
    public float transForceMulti = 10f;
    // Multiplier for rotational force
    public float rotForceMulti = 0.5f;

    Vector4 velocity;

    public Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        Initialize();
    }

    // Construct the matrix used to calculate direction
    public void Initialize()
    {
        float v = 2 * (wheelBase + wheelTrack);
        float p = 2 * Mathf.PI * wheelRadius;
        omniMatrix.SetRow(0, new Vector4(p / 4f, p / 4f, p / 4f, p / 4f));
        omniMatrix.SetRow(1, new Vector4(-p / 4f, p / 4f, p / 4f, -p / 4f));
        omniMatrix.SetRow(2, new Vector4(-p / v, p / v, -p / v, p / v));
        omniMatrix.SetRow(3, Vector4.zero);
    }

    // Set speed for one wheel (input -100 to 100 scaled to maxForce)
    // 0 - FL
    // 1 - FR
    // 2 - BL
    // 3 - BR
    public void SetMotorSpeed(int motor, int speed)
    {
        float mSpeed = Mathf.Clamp(speed, -100f, 100f) / 100f * maxSpeed;
        switch(motor)
        {
            case 0:
                FL = mSpeed;
                break;
            case 1:
                FR = mSpeed;
                break;
            case 2:
                BL = mSpeed;
                break;
            case 3:
                BR = mSpeed;
                break;
        }
        velocity = omniMatrix * new Vector4(FL, FR, BL, BR);
    }

    void Update ()
    {   
        rb.AddForce(transForceMulti * ((transform.forward * velocity[0]) - (transform.right * velocity[1])));
        rb.AddTorque(rotForceMulti * transform.up * velocity[2]);
	}
}
