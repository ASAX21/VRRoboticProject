using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Object manager handles objects in the scene
// Allows placement of objects at run-time
public class ObjectManager : MonoBehaviour, IFileReceiver {

    public static ObjectManager instance { get; private set; }

    public int totalObjects = 0;

    // Stores reference to the Ground LayerMask, and shaders to use during placement
    [Header("Placement")]
    public LayerMask groundMask;
    public Material validMat;
    public Material invalidMat;

    //  ----- Placeable object prefabs - spawned from Add Object menu -----
    // World Object Prefabs
    [Header("Prefab World Ojbects")]
    public GameObject cokeCanPrefab;
    public GameObject soccerBallPrefab;
    public GameObject markerPrefab;

    // Robot Prefabs
    [Header("Prefab Robots")]
    public GameObject labBotPrefab;
	public GameObject S4Prefab;

    // Environment Prefabs
    [Header("Prefab Environment Objects")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private Material tempMat;

    // ----- Custom Objects: Loaded at run-time -----
    [Header("Builder References")]
    public ObjectBuilder objectBuilder;
    public List<GameObject> customObjects;

    // ---------------------------------------------------------------------

    [Header("Placement Variables")]
    public bool isMouseOccupied = false;

    // Specific object currently being placed (one at a time strict)
    public PlaceableObject objectOnMouse;
    public float verticalOffset;
    private bool canPlaceObject = false;

    // Enviroment placement variables
    public GameObject testBeacon;
    private GameObject wallBeingPlaced = null;
    private bool isWallBeingPlaced = false;
    private bool wallStarted = false;
    public Vector3 wallStart;
    public Vector3 mousePos;

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

    // ---- Load an OBJ File ----
    public GameObject ReceiveFile(string filepath)
    {
        GameObject newCustomObj = objectBuilder.BuildObjectFromFile(filepath);
        WorldObject newWorldObj = newCustomObj.GetComponent<WorldObject>();

        MenuBarManager.instance.AddCustomObjectToMenu(newCustomObj.name, customObjects.Count);
        customObjects.Add(newCustomObj);
        return null;
    }

    // ---- Add Objects -----
    public void AddObjectToSceneAtPos(PlaceableObject newObj, float x, float y, float phi)
    {
        newObj.objectID = totalObjects++;
        newObj.transform.position = new Vector3(x/1000f, 0.03f, y/1000f);
        newObj.transform.rotation = Quaternion.Euler(new Vector3(0f, phi, 0f));
        if (newObj is Robot)
            SimManager.instance.AddRobotToScene(newObj as Robot);
        else if (newObj is WorldObject)
            SimManager.instance.AddWorldObjectToScene(newObj as WorldObject);
        else if (newObj is Marker)
            newObj.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        else
        {
            Debug.Log("Error adding objects: Unknown type");
            Destroy(newObj.gameObject);
        }
    }

    // Add some object to scene
    private void AddObjectToSceneOnMouse(PlaceableObject newObj)
    {
        newObj.objectID = totalObjects;
        totalObjects++;
        AddObjectToMouse(newObj, newObj.defaultVerticalOffset);
    }

    // ----- Specific object creators - called from Add Object menu
    public void AddCustomObjectToScene(int index, string args)
    {
        if (isMouseOccupied)
            return;
        PlaceableObject newObj = Instantiate(customObjects[index]).GetComponent<PlaceableObject>();
        newObj.gameObject.SetActive(true);
        newObj.name = customObjects[index].name;
        if (args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
                AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }
    public void AddCokeCanToScene(string args)
    {
        if (isMouseOccupied)
            return;

        PlaceableObject newObj = Instantiate(cokeCanPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Coke Can";
        if(args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }

    public void AddSoccerBallToScene(string args)
    {
        if (isMouseOccupied)
            return;

        PlaceableObject newObj = Instantiate(soccerBallPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Soccer Ball";
        if(args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }

    public void AddMarkerToScene(string args)
    {
        if (isMouseOccupied)
            return;

        PlaceableObject newObj = Instantiate(markerPrefab).GetComponent<PlaceableObject>();
        newObj.name = "Marker";
        if (args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            try
            {
                if (pos.Length == 5)
                {
                    float r = float.Parse(pos[2]) / 255;
                    float g = float.Parse(pos[3]) / 255;
                    float b = float.Parse(pos[4]) / 255;
                    newObj.GetComponent<Marker>().SetColor(new Color(r, g, b));
                }
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), 0);
            }
            catch (FormatException e)
            {
                Debug.Log("Object Manager: Format exception whilst parsing Marker input parameters: " + e);
                return;
            }
        }
    }

    public void AddLabBotToScene(string args)
    {
        if (isMouseOccupied)
            return;

        PlaceableObject newObj = Instantiate(labBotPrefab).GetComponent<PlaceableObject>();
        newObj.name = "LabBot";
        if(args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }

	public void AddS4ToScene(string args)
	{
        if (isMouseOccupied)
            return;

        PlaceableObject newObj = Instantiate(S4Prefab).GetComponent<PlaceableObject>();
		newObj.name = "S4";
        if(args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }

    // ----- Add World Elements -----
    public void AddWallToScene()
    {
        if (isMouseOccupied)
            return;

        isMouseOccupied = true;
        isWallBeingPlaced = true;
        StartCoroutine(DelayPlacement());
    }

    public void StartWallPlacement(Vector3 position)
    {
        wallStart = position;
        wallBeingPlaced = Instantiate(wallPrefab, SimManager.instance.world.transform);
        wallBeingPlaced.transform.position = wallStart;
        wallBeingPlaced.transform.localScale = Vector3.zero;
        tempMat = wallBeingPlaced.GetComponent<Renderer>().material;
        wallBeingPlaced.GetComponent<Renderer>().material = validMat;
        wallBeingPlaced.name = "wall";
        wallStarted = true;
    }

    public void FinishWallPlacement()
    {
        wallBeingPlaced.GetComponent<Renderer>().material = tempMat;
        wallStarted = false;
        canPlaceObject = false;
        wallBeingPlaced = null;
        isMouseOccupied = false;
    }

    public void CancelWallPlacement()
    {
        if (!isWallBeingPlaced)
            return;
        Destroy(wallBeingPlaced);
        wallStarted = false;
        canPlaceObject = false;
        wallBeingPlaced = null;
        isMouseOccupied = false;

    }
        
    // ----- Handle placement of object via mouse -----

    public void AddObjectToMouse(PlaceableObject newObject, float vert)
    {
        objectOnMouse = newObject;
        verticalOffset = vert;
        newObject.AttachToMouse();
        isMouseOccupied = true;
        StartCoroutine(DelayPlacement());
    }

    IEnumerator DelayPlacement()
    {
        yield return new WaitForSeconds(0.2f);
        canPlaceObject = true;
    }

    public void TryPlaceObject()
    {
        // Determine final validitiy of placement option
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool valid = objectOnMouse.updateValidity(Physics.Raycast(ray, out hit, 1000f, groundMask));
        if (valid && canPlaceObject)
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
            canPlaceObject = false;
            isMouseOccupied = false;
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
				objectOnMouse.transform.position = new Vector3(hitpoint.x, objectOnMouse.vertPlaceOffset + verticalOffset, hitpoint.z);
                if(Physics.Raycast(ray, 1000f, groundMask))
                    objectOnMouse.updateValidity(true);
                else
                    objectOnMouse.updateValidity(false);
            }
            // Left click
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
            // Escape - If object isn't initalized (has never been placed), destroy it.
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Delete))
            {
                if (!objectOnMouse.isInit)
                {
                    Destroy(objectOnMouse.gameObject);
                    objectOnMouse = null;
                    isMouseOccupied = false;
                }
                else
                {
                    if (objectOnMouse is Robot)
                        SimManager.instance.RemoveRobotFromScene(objectOnMouse as Robot);
                    else if (objectOnMouse is WorldObject)
                        SimManager.instance.RemoveWorldObjectFromScene(objectOnMouse as WorldObject);
                    else
                    {
                        Debug.Log("Delete on mouse failed");
                        return;
                    }
                    objectOnMouse = null;
                    isMouseOccupied = false;
                }
            }
            // Rotate using - and + keys
            else
            {
                objectOnMouse.transform.Rotate(new Vector3(0, Input.GetAxisRaw("Rotate Object") * 2f, 0));
            }
        }
        else if (isWallBeingPlaced)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (ground.Raycast(ray, out distance))
                mousePos = ray.GetPoint(distance);
            // Check for click
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelWallPlacement();
            }
            if (Input.GetMouseButtonDown(0))
            {
                // If this is the first click, set initial location
                if (!wallStarted && canPlaceObject)
                {
                    StartWallPlacement(mousePos);
                }
                else if (wallStarted && canPlaceObject)
                {
                    FinishWallPlacement();
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            AddWallToScene();
                            StartWallPlacement(mousePos);
                        }
                        else
                            AddWallToScene();
                    }
                }
            }
            // If first click done, update wall visualisation
            else if (wallStarted)
            {
                wallBeingPlaced.transform.position = ((mousePos + new Vector3(0, 0.05f, 0)) + wallStart) / 2;
                wallBeingPlaced.transform.localScale = new Vector3(Vector3.Distance(wallStart, mousePos), 0.3f, 0.01f);
                wallBeingPlaced.transform.eulerAngles = new Vector3(0, Mathf.Atan2(mousePos.x - wallStart.x, mousePos.z - wallStart.z) * 180 / Mathf.PI + 90F, 0);
            }
        }
    }
}