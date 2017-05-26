using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Object manager handles objects in the scene
// Allows placement of objects at run-time
public class ObjectManager : MonoBehaviour {

    public static ObjectManager instance = null;

    public int totalObjects = 0;

    // Stores reference to the Ground LayerMask, and shaders to use during placement
    public LayerMask groundMask;
    public Material validMat;
    public Material invalidMat;

    public GameObject placeableCylinder;
    public GameObject placeableCube;

    // Specific object currently being placed (one at a time strict)
    public PlaceableObject objectOnMouse;

    private Plane ground;

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        } 
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        ground = new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
    }

    public void AddTestObject()
    {
        GameObject testBot = Resources.Load("TestRobot") as GameObject;
        objectOnMouse = Instantiate(testBot).GetComponent<PlaceableObject>();
    }

    public void AddCylinderToScene()
    {
        PlaceableObject newCyl = Instantiate(placeableCylinder).GetComponent<PlaceableObject>();
        newCyl.PostBuild();
        newCyl.objectID = totalObjects;
        totalObjects++;
        AddObjectToMouse(newCyl);
    }

    public void AddObjectToMouse(PlaceableObject newObject)
    {
        objectOnMouse = newObject;
        newObject.AttachToMouse();
    }

    public void TryPlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool valid = objectOnMouse.updateValidity(Physics.Raycast(ray, out hit, 1000f, groundMask));
        if (valid)
        {
            objectOnMouse.PlaceObject();
            objectOnMouse = null;
        }
    }

    private void Update()
    {
		if (objectOnMouse != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance;
			if(ground.Raycast(ray, out distance)){
				Vector3 hitpoint = ray.GetPoint (distance);
				objectOnMouse.transform.position = new Vector3(hitpoint.x, 0.03f, hitpoint.z);
			}	
			if(Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
        }
    }
}
