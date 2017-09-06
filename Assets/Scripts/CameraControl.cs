﻿using UnityEngine;

public class CameraControl: MonoBehaviour
{
    public bool isOrtho = false;
    public float orthoPanSens = 800f;
    public float orthoZoomSens = 0.4f;

    public float horizontalLookSens = 5.0f;
    public float verticalLookSens = 5.0f;

    public float sidewaysPanSens = 1.0f;
    public float forwardsPanSens = 1.0f;

    public float verticalKeyboardSens = 0.005f;
    public float horizontalKeyboardSens = 0.005f;

    public float mouseZoomSens = 1.0f;
    public float keyboardZoomSens = 5.0f;

    Vector3 mousePos;
	Plane backPlane;

    public void ResetCamera()
    {
        if (isOrtho)
            ToggleCameraOrtho();
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(40f, 0f, 0f));
        Camera.main.transform.parent.position = new Vector3(0f, 1.35f, -0.7f);
    }

    public void ToggleCameraOrtho()
    {
        if (isOrtho)
        {
            isOrtho = false;
            Camera.main.orthographic = false;
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(45f, 0f, 0f));
        }
        else
        {
            isOrtho = true;
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 2;
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        }
    }

    private void Update()
    {
        if (UIManager.instance.windowOpen == false)
        {
            if (isOrtho)
            {
                // middle click : startPan 
                if (Input.GetMouseButtonDown(1))
                    mousePos = Input.mousePosition;

                // middle held : Pan 
                if (Input.GetMouseButton(1))
                {
                    Vector3 newMousePos = Input.mousePosition;
                    if (mousePos == newMousePos)
                        return;
                    Camera.main.transform.parent.Translate(new Vector3(mousePos.x - newMousePos.x, 0, mousePos.y - newMousePos.y) / orthoPanSens);
                    mousePos = newMousePos;
                }
                // Pan with Keyboard
                else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                {
                    Camera.main.transform.parent.Translate(new Vector3(Input.GetAxis("Horizontal") * sidewaysPanSens / 10f, 0, Input.GetAxis("Vertical") * forwardsPanSens / 10f));
                }
                // Zoom
                Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * orthoZoomSens;
                if (Input.GetAxis("Keyboard Zoom") != 0)
                {
                    Camera.main.orthographicSize += Input.GetAxis("Keyboard Zoom") * orthoZoomSens;
                }
            }
            else
            {
                // middle click : startPan 
                if (Input.GetMouseButtonDown(2))
                {
                    mousePos = Input.mousePosition;
                    backPlane = new Plane(-1 * Camera.main.transform.forward, getPlanePos(mousePos, new Plane(Vector3.up, Vector3.zero)));
                }

                // middle held : Pan 
                if (Input.GetMouseButton(2))
                {
                    Vector3 newMousePos = Input.mousePosition;
                    if (mousePos == newMousePos)
                        return;
                    Camera.main.transform.parent.Translate(getPlanePos(mousePos, backPlane) - getPlanePos(newMousePos, backPlane));
                    mousePos = newMousePos;
                }

                // Right Click : Free look
                else if (Input.GetMouseButton(1))
                {
                    float lookH = transform.localEulerAngles.y + horizontalLookSens * Input.GetAxis("Mouse X");
                    float lookV = transform.localEulerAngles.x - verticalLookSens * Input.GetAxis("Mouse Y");
                    lookV = Mathf.Clamp(lookV, 0.1f, 89.9f);
                    transform.localEulerAngles = new Vector3(lookV, lookH);
                }


                // If camera not being controlled by mouse, keyboard inputs
                else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                {
                    Vector3 forward = Input.GetAxis("Vertical") * verticalKeyboardSens * Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                    Vector3 sideways = Input.GetAxis("Horizontal") * horizontalKeyboardSens * transform.right.normalized;
                    Vector3 finalMove = forward + sideways;
                    transform.Translate(finalMove, Space.World);
                }

                else if (Input.GetAxis("Tilt Horizontal") != 0 || Input.GetAxis("Tilt Vertical") != 0)
                {
                    float lookH = transform.localEulerAngles.y + Input.GetAxis("Tilt Horizontal") * 10f;
                    float lookV = transform.localEulerAngles.x + Input.GetAxis("Tilt Vertical") * 10f;
                    lookV = Mathf.Clamp(lookV, 0.1f, 89.9f);
                    transform.localEulerAngles = new Vector3(lookV, lookH);
                }

                // Zoom
                transform.position += transform.forward * mouseZoomSens * Input.GetAxis("Mouse ScrollWheel");
                if (Input.GetAxis("Keyboard Zoom") != 0)
                {
                    transform.position += transform.forward * mouseZoomSens * Input.GetAxis("Keyboard Zoom");
                }

                // Make sure camera doens't move below ground
                if (transform.position.y < 0)
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
    }

	Vector3 getPlanePos(Vector3 mousepos, Plane plane){
		Ray ray = Camera.main.ScreenPointToRay(mousepos);
		float distance = 0; 
		if (plane.Raycast(ray, out distance)){
			return ray.GetPoint(distance);
		}
		return Vector3.zero;
	}
}