using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Object manager handles objects in the scene
// Allows placement of objects at run-time
public class ObjectManager : MonoBehaviour {

    public static ObjectManager instance { get; private set; }

    public int totalObjects = 0;

    // Stores reference to the Ground LayerMask, and shaders to use during placement
    public LayerMask groundMask;
    public Material validMat;
    public Material invalidMat;

    //  ----- Placeable object prefabs - spawned from Add Object menu -----
    // World Object Prefabs
    public GameObject cokeCanPrefab;
    public GameObject soccerBallPrefab;

    // Robot Prefabs
    public GameObject labBotPrefab;
    // ---------------------------------------------------------------------

    // Specific object currently being placed (one at a time strict)
    public PlaceableObject objectOnMouse;
    public float verticalOffset;

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

    // Add some object to scene
    private void AddObjectToScene(PlaceableObject newObj)
    {
        newObj.objectID = totalObjects;
        totalObjects++;
        AddObjectToMouse(newObj, 0f);
    }

    // Specific object creators - called from Add Object menu
    public void AddCokeCanToScene()
    {
        PlaceableObject newObj = Instantiate(cokeCanPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Coke Can";
        AddObjectToScene(newObj);
    }

    public void AddSoccerBallToScene()
    {
        PlaceableObject newObj = Instantiate(soccerBallPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Soccer Ball";
        AddObjectToScene(newObj);
    }

    public void AddLabBotToScene()
    {
        PlaceableObject newObj = Instantiate(labBotPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Lab Bot";
        AddObjectToScene(newObj);
    }
        
    // ----- Handle placement of object via mouse -----

    public void AddObjectToMouse(PlaceableObject newObject, float vert)
    {
        objectOnMouse = newObject;
        verticalOffset = vert;
        newObject.AttachToMouse();
    }

    public void TryPlaceObject()
    {
        // Determine final validitiy of placement option
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool valid = objectOnMouse.updateValidity(Physics.Raycast(ray, out hit, 1000f, groundMask));
        if (valid)
        {
            // If it is new object, add to sim manager
            if (!objectOnMouse.isInit)
            {
                if (objectOnMouse is Robot)
                    SimManager.instance.AddRobotToScene(objectOnMouse as Robot);
                else if (objectOnMouse is WorldObject)
                    SimManager.instance.AddWorldObjectToScene(objectOnMouse as WorldObject);
            }
            // Place object physically
            objectOnMouse.PlaceObject();
            objectOnMouse = null;
        }
    }

    private void Update()
    {
		if (objectOnMouse != null)
        {
            // Anchor object to the ground plane
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance;
			if(ground.Raycast(ray, out distance)){
				Vector3 hitpoint = ray.GetPoint (distance);
				objectOnMouse.transform.position = new Vector3(hitpoint.x, 0.03f + verticalOffset, hitpoint.z);
                if(Physics.Raycast(ray, 1000f, groundMask))
                    objectOnMouse.updateValidity(true);
                else
                    objectOnMouse.updateValidity(false);
            }
            // Left click
			if(Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
            // Escape - If object isn't initalized (has never been placed), destroy it.
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!objectOnMouse.isInit)
                {
                    Destroy(objectOnMouse.gameObject);
                    objectOnMouse = null;
                }
            }
            // Rotate using - and + keys
            else if (Input.GetKey(KeyCode.Minus))
            {
                objectOnMouse.transform.Rotate(new Vector3(0f, -2f, 0f));
            }
            else if (Input.GetKey(KeyCode.Equals))
            {
                objectOnMouse.transform.Rotate(new Vector3(0f, 2f, 0f));
            }
        }
    }
 
}