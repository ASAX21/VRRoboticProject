using UnityEngine;
using UnityEngine.UI;

public class ViewWorldObjectsWindow : MonoBehaviour {

    public static ViewWorldObjectsWindow instance { get; private set; }

    public Transform content;
    public GameObject objectButtonPrefab;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        gameObject.SetActive(false);
    }

    public void UpdateWorldObjectsList()
    {
        foreach (Transform button in content)
        {
            Destroy(button.gameObject);
        }

        foreach(WorldObject wObj in SimManager.instance.allWorldObjects)
        {
            GameObject newbutton = Instantiate(objectButtonPrefab, content, false);
            newbutton.transform.GetChild(0).GetComponent<Text>().text = "Object " + wObj.objectID;
            newbutton.GetComponent<Button>().onClick.AddListener(wObj.OpenInfoWindow);
        }
    }
}