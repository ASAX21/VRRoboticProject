using System.Collections.Generic;
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

    // Search only robots
    public Robot GetRobotByID(int id)
    {
        Robot toFind = allRobots.Find(x => x.objectID == id);
        return toFind;
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

    // Get the pose of an object by ID
    public int[] GetObjectPoseByID(int id)
    {
        PlaceableObject obj = GetObjectByID(id);
        int[] pose = new int[3];
        pose[0] = (int)Mathf.Round(1000 * obj.transform.position.x);
        pose[1] = (int)Mathf.Round(1000 * obj.transform.position.x);
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
        robot.objectID = totalObjects++;
        allRobots.Add(robot);
        ViewRobotsWindow.instance.AddRobotToDisplayList(robot);
        ServerManager.instance.activeRobot = robot;
    }

    // Add a world object
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
