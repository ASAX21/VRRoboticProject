﻿/* Sim manager is responsible for maintaining the list of Robots, Objects,
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
    public OSManager osManager;

    // Current robots, objects, and world
    public List<Robot> allRobots;
    public List<WorldObject> allWorldObjects;
    public GameObject world;

    public bool isPaused = false;
    public delegate void PauseAction();
    public PauseAction OnPause;
    public PauseAction OnResume;

    public GameObject testBeacon;

    // Record number of total objects, used to assign ID
    private int totalObjects = 0;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        // To platform specific things in Awake - Ready for other scripts in Start
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            osManager = new WindowsOSManager();
        }
        else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            osManager = new MacOSManager();
        }
    }

    private void Start() {

        allRobots = new List<Robot>();
        allWorldObjects = new List<WorldObject>();
        world = WorldBuilder.instance.CreateBox();
    }

    private void OnApplicationQuit()
    {
        if (osManager != null)
            osManager.Terminate();
    }

    public void LaunchTerminal()
    {
        // If windows launch CYGWIN
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            osManager.LaunchTerminal();
        }
        // Else launch default terminal
        else
        {
            osManager.LaunchTerminal();
        }
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
        pose[0] = (int)Mathf.Round(1000 * obj.transform.position.x);
        pose[1] = (int)Mathf.Round(1000 * obj.transform.position.z);
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
        // Remove the object from the scene
        Destroy(robot.gameObject);
        ViewRobotsWindow.instance.UpdateRobotList();
    }

    // Add a world object
    public void AddWorldObjectToScene(WorldObject worldObj)
    {
        Debug.Log("New Object");
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

    public void DestroyWorld()
    {
        RemoveAllWorldObjects();
        RemoveAllRobots();
        Destroy(world.gameObject);
    }

    // Remove all objects and load the original world (box)
    public void ResetWorld()
    {
        DestroyWorld();
        world = WorldBuilder.instance.CreateBox();
    }

    // Write the World object to a wld file
    public void SaveWorld()
    {
        FileStream fs = File.Open("test.wld", FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.ASCII))
        {
            
            writer.WriteLine("# World file created " + DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") + Environment.NewLine);

            foreach(Transform item in world.transform)
            {
                if(item.name == "floor")
                {
                    int floorWidth = (int)world.transform.Find("floor").localScale.x * 1000;
                    int floorHeight = (int)world.transform.Find("floor").localScale.z * 1000;
                    writer.WriteLine("floor " + floorWidth + " " + floorHeight);
                }
                else if(item.name == "wall")
                {
                    float wallLength = item.localScale.x/2;
                    float wallAngle = -item.eulerAngles.y;
                    Vector3 wallCentre = item.position;
                    Vector3 offset = new Vector3(Mathf.Cos(wallAngle * Mathf.PI / 180f) * wallLength, 0f, Mathf.Sin(wallAngle * Mathf.PI / 180f) * wallLength);
                    Vector3 wall1 = (wallCentre + offset) * 1000;
                    Vector3 wall2 = (wallCentre - offset) * 1000;
                    writer.WriteLine((int)wall1.x + " " + (int)wall1.z + " " + (int)wall2.x + " " + (int)wall2.z);
                }           
            }
        }
    }

    public void SaveObjects()
    {

    }
    
    // Pause and Resume by setting bodies to kinematic - will not move from applied forces
    public void PauseSimulation()
    {
        if (isPaused)
            return;

        isPaused = true;
        if(OnPause != null) OnPause();
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
