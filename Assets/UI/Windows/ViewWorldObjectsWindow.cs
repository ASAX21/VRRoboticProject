using UnityEngine;
using UnityEngine.UI;

public class ViewWorldObjectsWindow : MonoBehaviour {

    public static ViewWorldObjectsWindow instance { get; private set; }

    public Transform contentWindow;
    public GameObject objectButtonPrefab;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
    }

    public void AddRobotToDisplayList(WorldObject worldObj)
    {
        GameObject newbutton = Instantiate(objectButtonPrefab, contentWindow, false);
        newbutton.transform.GetChild(0).GetComponent<Text>().text = "Object " + worldObj.objectID;
        newbutton.GetComponent<Button>().onClick.AddListener(worldObj.OpenInfoWindow);
    }
}