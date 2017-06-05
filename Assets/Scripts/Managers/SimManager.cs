﻿using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour {

    public static SimManager instance { get; private set; }

    public ServerManager server;

    public List<Robot> allRobots;
    public List<WorldObject> allWorldObjects;

    public GameObject world;

    private int totalObjects = 1;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    private void Start() {
        allRobots = new List<Robot>();
        allWorldObjects = new List<WorldObject>();

    }
    
    // Find object by ID - Search robots first, then world objects
    public PlaceableObject GetObjectByID(int id)
    {
        PlaceableObject toFind = allRobots.Find(x => x.objectID == id);
        if(toFind == null)
        {
            toFind = allWorldObjects.Find(x => x.objectID == id);
        }

        return toFind;
    }

    public void SetSimulationSpeed(float simSpeed)
    {
        float newTimeScale = Mathf.Clamp(simSpeed, 0, 2.0f);
        Time.timeScale = newTimeScale;
        //Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Debug.Log(simSpeed);
    }

    public void AddRobotToList(Robot robot)
    {
        robot.objectID = totalObjects;
        allRobots.Add(robot);
        ViewRobotsWindow.instance.AddRobotToDisplayList(robot);
    }

    public void AddWorldObjectToScene(WorldObject worldObj)
    {
        allWorldObjects.Add(worldObj);
    }
    
    // TODO: Pause and Resume
    public void PauseSimulation()
    {

    }

    public void ResumeSimulation()
    {

    }
}
