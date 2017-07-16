using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controller for the View Robots window; displays all robots currently in the scene
public class ViewRobotsWindow : MonoBehaviour {

    public static ViewRobotsWindow instance { get; private set; }

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

    public void AddRobotToDisplayList(Robot robot)
    {
        GameObject newbutton = Instantiate(robotButtonPrefab, robotWindow, false);
        newbutton.GetComponent<Button>().onClick.AddListener(robot.OpenInfoWindow);
    }
}
