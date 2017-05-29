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
    private void AddObjectToScene(GameObject prefab)
    {
        PlaceableObject newObj = Instantiate(prefab).GetComponent<PlaceableObject>();
        newObj.objectID = totalObjects;
        totalObjects++;
        if (newObj is Robot)
            SimManager.instance.AddRobotToList(newObj as Robot);
        else if (newObj is WorldObject)
            SimManager.instance.AddWorldObjectToScene(newObj as WorldObject);
        AddObjectToMouse(newObj);
    }

    // Specific object creators - called from Add Object menu
    public void AddCokeCanToScene()
    {
        AddObjectToScene(cokeCanPrefab);
    }

    public void AddSoccerBallToScene()
    {
        AddObjectToScene(soccerBallPrefab);
    }

    public void AddLabBotToScene()
    {
        AddObjectToScene(labBotPrefab);

    }
        
    // ----- Handle placement of object via mouse -----

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
                if(Physics.Raycast(ray, 1000f, groundMask))
                    objectOnMouse.updateValidity(true);
                else
                    objectOnMouse.updateValidity(false);
            }	
			if(Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!objectOnMouse.isInit)
                {
                    Destroy(objectOnMouse.gameObject);
                    objectOnMouse = null;
                }
            }
        }
    }
 
}