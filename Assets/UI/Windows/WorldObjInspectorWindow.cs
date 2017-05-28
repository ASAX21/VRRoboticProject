using UnityEngine;
using UnityEngine.UI;

public class WorldObjInspectorWindow : MonoBehaviour {

    public WorldObject worldObj;

    // Position variables
    [SerializeField]
    private InputField objXValue, objzValue, objPhiValue;

    public Image lockButtonImage;
    public Sprite lockedImage;
    public Sprite unlockedImage;

    // Use this for initialization
    void Start () {
        lockButtonImage.sprite = worldObj.locked ? lockedImage : unlockedImage;
    }
	
	// Update is called once per frame
	void Update () {
        if (worldObj.locked)
        {
            objXValue.text = worldObj.transform.position.x.ToString();
            objzValue.text = worldObj.transform.position.z.ToString();
            objPhiValue.text = worldObj.transform.rotation.eulerAngles.y.ToString();
        }
    }

    public void LockButton()
    {
        worldObj.locked = !worldObj.locked;
        lockButtonImage.sprite = worldObj.locked ? lockedImage : unlockedImage;
        objXValue.readOnly = worldObj.locked;
        objzValue.readOnly = worldObj.locked;
        objPhiValue.readOnly = worldObj.locked;
    }

    public void CloseWindow()
    {
        worldObj.isWindowOpen = false;
        Destroy(gameObject);
    }
}
