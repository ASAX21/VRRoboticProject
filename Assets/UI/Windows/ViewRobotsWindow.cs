using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRobotsWindow : MonoBehaviour {

    public static ViewRobotsWindow instance { get; private set; }

    private SimManager simManager;

    public Transform robotWindow;
    public GameObject robotButtonPrefab;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        simManager = SimManager.instance;
    }

    public void AddRobotToDisplayList(Robot robot)
    {
        Instantiate(robotButtonPrefab, robotWindow, false);
    }
}
