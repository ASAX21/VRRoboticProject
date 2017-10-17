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
    public GameObject cratePrefab;
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

    // Mouse raycast variables
    Ray ray;
    RaycastHit hit;

    // Enviroment placement variables
    private Wall wallBeingPlaced = null;
    public bool isWallBeingPlaced = false;
    public bool wallStarted = false;
    public Vector3 wallStart;
    public Vector3 mousePos;

    public bool removingWalls = false;
    public delegate void CancelRemoveWallsDelegate();
    public CancelRemoveWallsDelegate cancelRemoveEvent;

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
        if (newCustomObj == null)
            return null;
        WorldObject newWorldObj = newCustomObj.GetComponent<WorldObject>();

        MenuBarManager.instance.AddCustomObjectToMenu(newCustomObj.name, customObjects.Count);
        customObjects.Add(newCustomObj);
        return null;
    }

    // ---- Add Objects -----
    public void AddObjectToSceneAtPos(PlaceableObject newObj, float x, float y, float phi)
    {
        newObj.objectID = totalObjects++;
        newObj.transform.position = new Vector3(x/Eyesim.Scale, newObj.defaultVerticalOffset, y/Eyesim.Scale);
        newObj.transform.rotation = Quaternion.Euler(new Vector3(0f, phi, 0f));
        newObj.isInit = true;
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
            FreeMouse();

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

    // Callback for creating objects of a known type
    public void AddPredefinedObjectToScene(string type, string args)
    {
        if (isMouseOccupied)
            FreeMouse();

        PlaceableObject newObj;
        switch (type.ToLower())
        {
            case "can":
                newObj = Instantiate(cokeCanPrefab).GetComponent<PlaceableObject>();
                newObj.name = "Can";
                break;
            case "soccer":
                newObj = Instantiate(soccerBallPrefab).GetComponent<PlaceableObject>();
                newObj.name = "Soccer Ball";
                break;
            case "crate":
                newObj = Instantiate(cratePrefab).GetComponent<PlaceableObject>();
                newObj.name = "Crate";
                break;
            case "labbot":
                newObj = Instantiate(labBotPrefab).GetComponent<PlaceableObject>();
                newObj.name = "LabBot";
                break;
            case "s4":
                newObj = Instantiate(S4Prefab).GetComponent<PlaceableObject>();
                newObj.name = "S4";
                break;
            default:
                Debug.Log("Unknown object type");
                return;
        }
        
        if (args.Length == 0)
            AddObjectToSceneOnMouse(newObj);
        else
        {
            string[] pos = args.Split(':');
            AddObjectToSceneAtPos(newObj, float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
    }

    // Single argument callbakcs for UI buttons
    public void AddCokeCanToScene(string args)
    {
        AddPredefinedObjectToScene("Can", args);
    }

    public void AddSoccerBallToScene(string args)
    {
        AddPredefinedObjectToScene("Soccer", args);
    }

    public void AddCrateToScene(string args)
    {
        AddPredefinedObjectToScene("Crate", args);
    }

    public void AddMarkerToScene(string args)
    {
        if (isMouseOccupied)
            FreeMouse();

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
            FreeMouse();

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
            FreeMouse();

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

    public void DeleteObjectOnMouse()
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

    // ----- Add World Elements -----
    public void AddWallToScene()
    {
        if (isMouseOccupied)
            FreeMouse();

        isMouseOccupied = true;
        isWallBeingPlaced = true;
        StartCoroutine(DelayPlacement());
    }

    public void StartWallPlacement(Vector3 position)
    {
        wallStart = position;
        wallBeingPlaced = Instantiate(wallPrefab, SimManager.instance.world.transform).GetComponent<Wall>();
        wallBeingPlaced.transform.position = wallStart;
        wallBeingPlaced.transform.localScale = Vector3.zero;
        wallBeingPlaced.BeginPlacement();
        wallStarted = true;
        StartCoroutine(DelayPlacement());
    }

    public void FinishWallPlacement()
    {
        wallBeingPlaced.FinishPlacement();
        wallStarted = false;
        canPlaceObject = false;
        wallBeingPlaced = null;
        isWallBeingPlaced = false;
        isMouseOccupied = false;
        SimManager.instance.worldChanged = true;
    }

    public void CancelWallPlacement()
    {
        if (!isWallBeingPlaced)
            FreeMouse();
        Destroy(wallBeingPlaced);
        wallStarted = false;
        isWallBeingPlaced = false;
        canPlaceObject = false;
        wallBeingPlaced = null;
        isMouseOccupied = false;
    }

    public void RemoveWall()
    {
        if (isMouseOccupied)
            FreeMouse();

        isMouseOccupied = true;
        removingWalls = true;
    }

    public void CancelRemoveWall()
    {
        removingWalls = false;
        isMouseOccupied = false;
        if (cancelRemoveEvent != null)
            cancelRemoveEvent.Invoke();
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
        canPlaceObject = false;
        yield return new WaitForSeconds(0.2f);
        canPlaceObject = true;
    }

    public void TryPlaceObject()
    {
        // Determine final validitiy of placement option
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool valid = objectOnMouse.updateValidity(Physics.Raycast(ray, out hit, Eyesim.Scale, groundMask));
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

    public void FreeMouse()
    {
        if (objectOnMouse != null)
            DeleteObjectOnMouse();
        else if (isWallBeingPlaced)
            CancelWallPlacement();
        else if (removingWalls)
            CancelRemoveWall();
    }

    private void Update()
    {
		if (objectOnMouse != null)
        {
            // Anchor object to the ground plane
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance;
			if(ground.Raycast(ray, out distance)){
				Vector3 hitpoint = ray.GetPoint (distance);
				objectOnMouse.transform.position = new Vector3(hitpoint.x, objectOnMouse.vertPlaceOffset + verticalOffset, hitpoint.z);
                if(Physics.Raycast(ray, Eyesim.Scale, groundMask))
                    objectOnMouse.updateValidity(true);
                else
                    objectOnMouse.updateValidity(false);
            }
            // Left click
            if (Input.GetMouseButtonDown(0))
                TryPlaceObject();
            // Escape - If object isn't initalized (has never been placed), destroy it.
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Delete))
                DeleteObjectOnMouse();
            // Rotate using - and + keys
            else
                objectOnMouse.transform.Rotate(new Vector3(0, Input.GetAxisRaw("Rotate Object") * 2f, 0));
        }
        else if (isWallBeingPlaced)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                    StartWallPlacement(mousePos);
                else if (wallStarted && canPlaceObject && wallBeingPlaced.CanPlace())
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
        else if(removingWalls)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                CancelRemoveWall();
        }
    }
}