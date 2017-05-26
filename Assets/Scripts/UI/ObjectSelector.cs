using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    public static ObjectSelector instance;

    public Transform windowContainer;

    // Object currently selected
    public bool isObjectSelected = false;
    public PlaceableObject selectedObject;

    // Create Object Windows
    public RobotInspectorWindow robotWindowPrefab;

    // Enforce the singleton pattern
    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public void NewObjectSelected(PlaceableObject newObject)
    {
        if(selectedObject != null)
            selectedObject.Deselect();

        isObjectSelected = true;
        selectedObject = newObject;
    }

    public void UnselectObject()
    {
        selectedObject.Deselect();
        isObjectSelected = false;
        selectedObject = null;
    }

    public void DisplayRobotInfoWindow(Robot robot)
    {
        Instantiate(robotWindowPrefab, windowContainer, false).robot = robot;
    }
}
