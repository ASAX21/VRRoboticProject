using UnityEngine;
using UnityEngine.UI;

// Controller for the View Robots window; displays all robots currently in the scene
public class ViewRobotsWindow : MonoBehaviour {

    public static ViewRobotsWindow instance { get; private set; }

    public Transform content;
    public GameObject robotButtonPrefab;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
    }

    public void UpdateRobotList()
    {
        // Clear out existing buttons
        foreach(Transform button in content)
        {
            Destroy(button.gameObject);
        }
        // Add button for each robot
        foreach(Robot robot in SimManager.instance.allRobots)
        {
            GameObject newbutton = Instantiate(robotButtonPrefab, content, false);
            newbutton.transform.GetChild(0).GetComponent<Text>().text = robot.name + " #" + robot.objectID;
            newbutton.GetComponent<Button>().onClick.AddListener(robot.OpenInfoWindow);
        }
    }
}
