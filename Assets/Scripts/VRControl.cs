using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using RobotComponents;

public class VRControl : MonoBehaviour {

    public static VRControl instance;

	public GameObject NormalCamera;
	public GameObject VRCamera;

	public Transform CameraTarget;
	public Transform VrCameraParent;

    private bool VREnabled = false;

    [SerializeField]
    CharacterController VRCharacter;
	public float speed=1;

    void Awake()
    {
        if (instance == this || instance == null)
            instance = this;
        else
            Destroy(this);
    }

	void Start ()
    {
		ChangeCamera0();
        string[] controllers = Input.GetJoystickNames();
        Debug.Log(controllers.Length + " Joysticks connected");
        foreach (string ctr in controllers)
            Debug.Log(ctr);
        VRCharacter.enabled = false;
        VRCharacter.gameObject.SetActive(false);
        VRCharacter.transform.position = new Vector3(0.2f, 0.2f, 0.2f);
    }	

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ChangeCamera0();
        else if (Input.GetKeyDown(KeyCode.N))
        {
            if (VREnabled)
                ResetVRCharacter();
            else
                ChangeCamera1();
        }
        /*
		else if(Input.GetKeyDown(KeyCode.M))
		{
			ChangeCamera2();
		}
        */

        if (VREnabled)
        {
            Vector3 forwardsMove = VRCamera.transform.forward * Input.GetAxis("Vertical");
            Vector3 horizontalMove = VRCamera.transform.right * Input.GetAxis("Horizontal");
            VRCharacter.SimpleMove(forwardsMove + horizontalMove);
        }
	}

    // Reset the position of the VR Character
    public void ResetVRCharacter()
    {
        VRCharacter.transform.position = new Vector3(0.2f, 0.2f, 0.2f);
    }

    // Change to normal camera
	public void ChangeCamera0()
	{
		VRSettings.enabled=false;
		VRCamera.SetActive(false);
		NormalCamera.SetActive(true);
        VREnabled = false;
        VRCharacter.enabled = false;
        VRCharacter.gameObject.SetActive(false);
    }

    // Change to freeview VR Camera
	public void ChangeCamera1()
	{
        // Enable VR
		VRSettings.enabled=true;

        // Set parent back to VR Camera Position (incase moved to robot)
		VRCamera.transform.SetParent(VrCameraParent);
		VRCamera.transform.localPosition = Vector3.zero;
        // Set Active
		VRCamera.SetActive(true); 
		NormalCamera.SetActive(false);
        VREnabled = true;
        VRCharacter.gameObject.SetActive(true);
        VRCharacter.enabled = true;
	}

    // Change to VR camera attached to robot
	public void ChangeCamera2()
	{
		VRSettings.enabled=true;
		LabBot lab=GameObject.FindObjectOfType<LabBot>();
		if(lab)
		{
			EyeCamera eye=lab.GetComponentInChildren<EyeCamera>();
			if(eye)
			{
				VrCameraParent.SetParent(eye.transform.parent);
				VrCameraParent.localPosition=Vector3.zero;
				VrCameraParent.localRotation=Quaternion.identity;

				VRCamera.transform.SetParent(VrCameraParent.transform.parent);
				VRCamera.transform.localPosition = Vector3.zero;
			}
		}

		VRCamera.SetActive(true);
		NormalCamera.SetActive(false);
	}

    public void ViewFromRobotPerspective(Robot robot)
    {
        if (!(robot is ICameras))
            return;

        EyeCamera eyeCam = (robot as ICameras).GetCameraComponent(0);
        VRCamera.transform.SetParent(eyeCam.transform);
        VRCamera.transform.localPosition = Vector3.zero;
        VRCamera.transform.localRotation = Quaternion.identity;
        VRSettings.enabled = true;
        VRCamera.SetActive(true);
        NormalCamera.SetActive(false);
    }
}
