using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniWheelController : MonoBehaviour
{

    public float wheelTrack;    // Length of the axel
    public float wheelBase;     // Distance between axels
    public float wheelRadius;
    private Matrix4x4 omniMatrix;

    // Wheel rotational velocities
    public float FL, FR, BL, BR;
    [SerializeField]
    private float maxPosForce, maxRotForce;
    private float maxSpeed = 200f;
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
        Debug.Log("SET MOTOR SPEED: " + motor + "  " + speed);
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
    }

    // Update is called once per frame
    void Update ()
    {
        Vector4 velocity = omniMatrix * new Vector4(FL, FR, BL, BR);
        rb.AddForce((transform.forward * velocity[0]) - (transform.right * velocity[1]));
        rb.AddTorque(transform.up * velocity[2]);
	}

    public void TestDirection()
    {
        Vector4 result = omniMatrix * new Vector4(FL, FR, BL, BR);
        Debug.Log(result);
    }
}
