using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour {

    public static SimManager instance { get; private set; }

    public ServerManager server;

    public List<PlaceableObject> allObjects;

    public GameObject world;

    private int totalObjects = 0;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    private void Start() {
        allObjects = new List<PlaceableObject>();
	}
    
    public PlaceableObject GetObjectByID(int id)
    {
        return allObjects.Find(x => x.objectID == id);
    }

    public void SetSimulationSpeed(float simSpeed)
    {
        float newTimeScale = Mathf.Clamp(simSpeed, 0, 2.0f);
        Time.timeScale = simSpeed;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Debug.Log(simSpeed);
    }

    public void PauseSimulation()
    {

    }

    public void ResumeSimulation()
    {

    }

    public void CloseSimulator()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
