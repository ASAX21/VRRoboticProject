using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using RobotComponents;

public class VRControl : MonoBehaviour {

	public GameObject NormalCamera;
	public GameObject VRCamera;

	public Transform CameraTarget;
	public Transform VrCameraParent;

	public float speed=1;

	void Start () {
		ChangeCamera0();
        string[] controllers = Input.GetJoystickNames();
        Debug.Log(controllers.Length + " Joysticks connected");
        foreach (string ctr in controllers)
            Debug.Log(ctr);
	}	

	void Update () {
		if(Input.GetKeyDown(KeyCode.B))
		{
			ChangeCamera0();
		}
		else if(Input.GetKeyDown(KeyCode.N))
		{
			ChangeCamera1();
		}
		else if(Input.GetKeyDown(KeyCode.M))
		{
			ChangeCamera2();
		}

		VrCameraParent.Translate((VrCameraParent.forward*Input.GetAxis("Vertical")+VrCameraParent.right*Input.GetAxis("Horizontal")+VrCameraParent.up*Input.GetAxis("Updown"))*speed,Space.Self);
	}

    // Change to normal camera
	public void ChangeCamera0()
	{
		VRSettings.enabled=false;
		VRCamera.SetActive(false);
		NormalCamera.SetActive(true);
	}

    // Change to freeview VR Camera
	public void ChangeCamera1()
	{
		VRSettings.enabled=true;
		VrCameraParent.SetParent(NormalCamera.transform.parent);
		VrCameraParent.localPosition=Vector3.zero;
		VrCameraParent.localRotation=Quaternion.identity;

		VRCamera.transform.SetParent(VrCameraParent);
		VRCamera.transform.localPosition = Vector3.zero;
		VRCamera.SetActive(true); 
		NormalCamera.SetActive(false);
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
}
