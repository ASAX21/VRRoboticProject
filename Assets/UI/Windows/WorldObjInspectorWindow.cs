using UnityEngine;
using UnityEngine.UI;

public class WorldObjInspectorWindow : MonoBehaviour {

    public WorldObject worldObj;

    // Object identifiers
    [Header("Identifier Text Display")]
    [SerializeField]
    private Text objNumber;
    [SerializeField]
    private Text objName;

    // Position variables
    [Header("Position Text Displays")]
    [SerializeField]
    private InputField objXValue;
    [SerializeField]
    private InputField objYValue, objPhiValue;

    [Header("Icons")]
    public Image lockButtonImage;
    public Sprite lockedImage;

    // Use this for initialization
    void Start () {
        lockButtonImage.sprite = lockedImage;
        objNumber.text = "ID # " + worldObj.objectID.ToString();
        objName.text = worldObj.name;

        objXValue.interactable = SimManager.instance.isPaused;
        objYValue.interactable = SimManager.instance.isPaused;
        objPhiValue.interactable = SimManager.instance.isPaused;

        lockButtonImage.color = worldObj.locked ? Color.white : Color.grey;

        SimManager.instance.OnPause += OnSimPaused;
        SimManager.instance.OnResume += OnSimResumed;
    }

    void OnDestroy()
    {
        SimManager.instance.OnPause -= OnSimPaused;
        SimManager.instance.OnResume -= OnSimResumed;
    }

    // Update is called once per frame
    void Update () {
        if (!SimManager.instance.isPaused)
        {
            objXValue.text = (1000f * worldObj.transform.position.x).ToString("0.##");
            objYValue.text = (1000f * worldObj.transform.position.z).ToString("N2");
            objPhiValue.text = worldObj.transform.rotation.eulerAngles.y.ToString("0.##");
        }
    }

    public void SetXPosition(string x)
    {
        Vector3 pos = worldObj.transform.position;
        pos.x = float.Parse(x) / 1000f;
        worldObj.transform.position = pos;
    }

    public void SetYPosition(string y)
    {
        Vector3 pos = worldObj.transform.position;
        pos.z = float.Parse(y) / 1000f;
        worldObj.transform.position = pos;
    }

    public void SetPhiPosition(string phi)
    {
        worldObj.transform.rotation = Quaternion.Euler(0, float.Parse(phi), 0);
    }

    public void OnSimPaused()
    {
        objXValue.interactable = true;
        objYValue.interactable = true;
        objPhiValue.interactable = true;
    }

    public void OnSimResumed()
    {
        objXValue.interactable = false;
        objYValue.interactable = false;
        objPhiValue.interactable = false;
    }

    public void LockButton()
    {
        worldObj.locked = !worldObj.locked;
        lockButtonImage.color = worldObj.locked ? Color.white : Color.grey;
    }

    public void DeleteButton()
    {
        SimManager.instance.RemoveWorldObjectFromScene(worldObj);
        Destroy(gameObject);
    }

    public void CloseWindow()
    {
        worldObj.isWindowOpen = false;
        Destroy(gameObject);
    }
}
