/* Sim manager is responsible for maintaining the list of Robots, Objects,
 * and the state of the current World. It also handles launching and termination
 * of the simulation program
 */
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour {

    public static SimManager instance { get; private set; }

    public ServerManager server;

    // Current robots, objects, and world
    public List<Robot> allRobots;
    public List<WorldObject> allWorldObjects;
    public GameObject world;

    [NonSerialized]
    public bool isPaused = false;
    public delegate void PauseAction();
    public PauseAction OnPause;
    public PauseAction OnResume;

    // Record number of total objects, used to assign ID
    private int totalObjects = 0;

    // List of Objects used to save state
    private List<ObjectState> defaultState;
    private int stateID = 0;

    [NonSerialized]
    public bool worldChanged = false;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    // Called by application manager on launch (if no default sim file)
    public void CreateInitialWorld()
    {
        if (SettingsManager.instance.defaultSim != "")
        {
            EyesimLogger.instance.Log("Loading default sim");
            SimReader.instance.ReceiveFile(SettingsManager.instance.defaultSim);
            SaveState();
        }
        else
            CreateNewBox(2000, 2000);
    }

    // Search only Robots
    public Robot GetRobotByID(int id)
    {
        return allRobots.Find(x => x.objectID == id);
    }

    // Search only WorldObjects
    public WorldObject GetWorldObjectByID(int id)
    {
       return allWorldObjects.Find(x => x.objectID == id);
    }
    
    // Find object by ID - Search robots first, then world objects
    public PlaceableObject GetObjectByID(int id)
    {
        PlaceableObject toFind = GetRobotByID(id);
        if(toFind == null)
            toFind = allWorldObjects.Find(x => x.objectID == id);
        return toFind;
    }

    // Get the pose of an object by ID
    public int[] GetObjectPoseByID(int id)
    {
        PlaceableObject obj = GetObjectByID(id);
        if (obj == null)
            return null;
        int[] pose = new int[3];
        pose[0] = (int)Mathf.Round(Eyesim.Scale * obj.transform.position.x);
        pose[1] = (int)Mathf.Round(Eyesim.Scale * obj.transform.position.z);
        pose[2] = (int)obj.transform.eulerAngles.y;
        return pose;
    } 

    // Control simulation speed - BUGGY
    public void SetSimulationSpeed(float simSpeed)
    {
        float newTimeScale = Mathf.Clamp(simSpeed, 0, 2.0f);
        Time.timeScale = newTimeScale;
        Debug.Log(simSpeed);
    }

    // Add a robot
    public void AddRobotToScene(Robot robot)
    {
        robot.objectID = ++totalObjects;
        allRobots.Add(robot);
        ViewRobotsWindow.instance.UpdateRobotList();
        ServerManager.instance.activeRobot = robot;
    }

    // Remove a robot
    public void RemoveRobotFromScene(Robot robot)
    {
        if (!allRobots.Remove(robot))
        {
            Debug.Log("Failed to remove a robot from the scene!");
            return;
        }
        if (robot.myConnection != null)
            ServerManager.instance.CloseConnection(robot.myConnection);
        // Change the active robot
        if (robot == ServerManager.instance.activeRobot)
        {
            if (allRobots.Count > 0)
                ServerManager.instance.activeRobot = allRobots.Last();
            else
                ServerManager.instance.activeRobot = null;
        }
        // Remove info window
        if (robot.myWindow != null)
            Destroy(robot.myWindow.gameObject);
        // Remove the object from the scene
        Destroy(robot.gameObject);
        ViewRobotsWindow.instance.UpdateRobotList();
    }

    // Add a world object
    public void AddWorldObjectToScene(WorldObject worldObj)
    {
        worldObj.objectID = ++totalObjects;
        allWorldObjects.Add(worldObj);
        ViewWorldObjectsWindow.instance.UpdateWorldObjectsList();
    }

    // Remove a world object
    public void RemoveWorldObjectFromScene(WorldObject worldObj)
    {
        if (!allWorldObjects.Remove(worldObj))
            Debug.Log("Failed to remove a world object from the scene!");
        Destroy(worldObj.gameObject);
        ViewWorldObjectsWindow.instance.UpdateWorldObjectsList();
    }

    // Remove all robots from the scene
    public void RemoveAllRobots()
    {
        // Iterate backwards and remove each robot from the list
        for(int i = allRobots.Count - 1;  i >= 0; i--)
        {
            RemoveRobotFromScene(allRobots[i]);
        }
    }

    // Remove all world objects from the scene
    public void RemoveAllWorldObjects()
    {
        for (int i = allWorldObjects.Count - 1; i >= 0; i--)
        {
            RemoveWorldObjectFromScene(allWorldObjects[i]);
        }
    }

    public void RemoveAllMarkers()
    {
        foreach (GameObject mark in GameObject.FindGameObjectsWithTag("Marker"))
        {
            Debug.Log("marker!");
            Destroy(mark);
        }
    }

    public void DestroyWorld()
    {
        worldChanged = true;
        RemoveAllWorldObjects();
        RemoveAllRobots();
        RemoveAllMarkers();
        Destroy(world.gameObject);
    }

    // Remove all objects and load the original world (box)
    public void ResetWorld()
    {
        CreateNewBox((int) (2 * Eyesim.Scale),  (int) (2 * Eyesim.Scale) );
    }

    public void CreateNewBox(int width, int height)
    {
        if(world != null)
            DestroyWorld();
        allRobots = new List<Robot>();
        allWorldObjects = new List<WorldObject>();
        totalObjects = 0;
        WorldBuilder.instance.CreateBox(width, height);
    }

    // Save a State
    public void SaveState()
    {
        defaultState = new List<ObjectState>();
        worldChanged = false;
        stateID = 0;
        foreach(Robot robot in allRobots)
        {
            ObjectState state = new ObjectState();
            state.id = robot.objectID;
            state.type = robot.type;
            state.pos = (robot.transform.position.x * Eyesim.Scale).ToString() + ":" +
                (robot.transform.position.z * Eyesim.Scale).ToString() + ":" +
                robot.transform.rotation.eulerAngles.y.ToString();
            stateID = robot.objectID > stateID ? robot.objectID : stateID;
            defaultState.Add(state);
        }
        foreach(WorldObject wObj in allWorldObjects)
        {
            ObjectState state = new ObjectState();
            state.id = wObj.objectID;
            state.type = wObj.type;
            state.pos = (wObj.transform.position.x * Eyesim.Scale).ToString() + ":" +
                (wObj.transform.position.z * Eyesim.Scale).ToString() + ":" +
                wObj.transform.rotation.eulerAngles.y.ToString();
            stateID = wObj.objectID > stateID ? wObj.objectID : stateID;
            defaultState.Add(state);
        }
    }

    public void RestoreState()
    {
        if(worldChanged)
        {
            Debug.Log("Can't restore state: World changed");
            return;
        }
        ObjectManager.instance.FreeMouse();
        RemoveAllWorldObjects();
        RemoveAllRobots();
        foreach(ObjectState state in defaultState)
        {
            totalObjects = state.id - 1;
            ObjectManager.instance.AddPredefinedObjectToScene(state.type, state.pos);
        }
        totalObjects = stateID;
    }

    // Write the World object to a wld file
    public void SaveWorld()
    {
        FileStream fs = File.Open("SavedWorld.wld", FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
        {
            
            writer.WriteLine("# World file created " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + Environment.NewLine);

            foreach(Transform item in world.transform)
            {
                if(item.name == "floor")
                {
                    int floorWidth = (int) (world.transform.Find("floor").localScale.x * Eyesim.Scale);
                    int floorHeight = (int) (world.transform.Find("floor").localScale.z * Eyesim.Scale);
                    writer.WriteLine("floor " + floorWidth + " " + floorHeight);
                }
                else if(item.name == "wall")
                {
                    float wallLength = item.localScale.x/2;
                    float wallAngle = -item.eulerAngles.y;
                    Vector3 wallCentre = item.position;
                    Vector3 offset = new Vector3(Mathf.Cos(wallAngle * Mathf.PI / 180f) * wallLength, 0f, Mathf.Sin(wallAngle * Mathf.PI / 180f) * wallLength);
                    Vector3 wall1 = (wallCentre + offset) * Eyesim.Scale;
                    Vector3 wall2 = (wallCentre - offset) * Eyesim.Scale;
                    writer.WriteLine((int)wall1.x + " " + (int)wall1.z + " " + (int)wall2.x + " " + (int)wall2.z);
                }           
            }
        }
    }

    // Write scene to a Sim file
    public void SaveSim()
    {
        PauseSimulation();
        SaveWorld();
        FileStream fs = File.Open("SavedSim.sim", FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
        {
            // Save world, and link to default save location
            writer.WriteLine("# Sim file created " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + Environment.NewLine);
            writer.WriteLine("# World File " + Environment.NewLine + "world SavedWorld.wld");
            writer.Write(Environment.NewLine + Environment.NewLine);

            // Save robot locations
            writer.WriteLine("# Robots");
            foreach(Robot rob in allRobots)
                writer.WriteLine(rob.type + " " + (int)Math.Floor(rob.transform.position.x * Eyesim.Scale) + " " + (int)Math.Floor(rob.transform.position.z * Eyesim.Scale) + " " + (int)Math.Floor(rob.transform.eulerAngles.y));
            writer.WriteLine();

            // Save object locations
            writer.WriteLine("# Objects");
            foreach(WorldObject wObj in allWorldObjects)
                writer.WriteLine(wObj.type + " " + (int)Math.Floor(wObj.transform.position.x * Eyesim.Scale) + " " + (int)Math.Floor(wObj.transform.position.z * Eyesim.Scale) + " " + (int)Math.Floor(wObj.transform.eulerAngles.y));
            ResumeSimulation();
        }
    }
    
    // Pause and Resume by setting bodies to kinematic - will not move from applied forces
    public void PauseSimulation()
    {
        if (isPaused)
            return;
        isPaused = true;
        foreach (Robot rob in allRobots)
        {
            foreach (PhysicalContainer phys in rob.physContainer)
            {
                phys.rigidBody.isKinematic = true;
            }
        }

        foreach(WorldObject wObj in allWorldObjects)
        {
            foreach(PhysicalContainer phys in wObj.physContainer)
            {
                phys.rigidBody.isKinematic = true;
            }
        }
        if (OnPause != null)
            OnPause();
    }

    public void ResumeSimulation()
    {
        if (!isPaused)
            return;

        foreach (Robot rob in allRobots)
        {
            foreach (PhysicalContainer phys in rob.physContainer)
            {
                phys.rigidBody.isKinematic = false;
            }
        }

        foreach (WorldObject wObj in allWorldObjects)
        {
            foreach (PhysicalContainer phys in wObj.physContainer)
            {
                phys.rigidBody.isKinematic = false;
            }
        }
        // Check if an object is on the mouse - if so, reapply kinematic property
        PlaceableObject mouseObj = ObjectManager.instance.objectOnMouse;
        if (mouseObj != null)
        {
            foreach (PhysicalContainer phys in mouseObj.physContainer)
            {
                phys.rigidBody.isKinematic = true;
            }
        }
        if(OnResume != null) OnResume();
        isPaused = false;
    }
}

// Helper class to save and restore object states
internal class ObjectState
{
    public int id;
    public string type;
    public string pos;
}
