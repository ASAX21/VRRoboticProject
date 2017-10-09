using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Object in the scene that can be placed with the ObjectManager
// Robots, cans, cubes, etc.
public abstract class PlaceableObject : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected ObjectSelector objectSelector;
    protected ObjectManager objectManager;

    public int objectID;
    public string type;

    // Placement variables
    public bool isInit = false;
    public bool isPlaced = true;
    public bool isSelected = false;
    public bool locked = false;
    public bool unpausedLockState = false;
    public bool isValidPlacement = true;

    private LayerMask currLayer = 0;
	public int collisionCount = 0;

    public float defaultVerticalOffset = 0f;
    public float vertPlaceOffset = 0.03f;

    // Physics
    public Vector3 centreOfMass;

    // List of all materials
    public List<MaterialContainer> matContainer;
    private Material validMat;
    private Material invalidMat;

    // Unity Components
    public List<PhysicalContainer> physContainer;

    // UI Variables
    public bool isWindowOpen = false;

    internal virtual void Awake()
    {
        Debug.Log("Awake!");
        if (centreOfMass != Vector3.zero)
        {
            GetComponent<Rigidbody>().centerOfMass = centreOfMass;
        }
        PostBuild();
    }

    private void Start()
    {
        objectSelector = ObjectSelector.instance;
        objectManager = ObjectManager.instance;
        validMat = ObjectManager.instance.validMat;
        invalidMat = ObjectManager.instance.invalidMat;
        SimManager.instance.OnPause += OnSimPaused;
        SimManager.instance.OnResume += OnSimResumed;
        isPlaced = true;
    }

    private void OnDestroy()
    {
        SimManager.instance.OnPause -= OnSimPaused;
        SimManager.instance.OnResume -= OnSimResumed;
    }

    // This function is called when a robot is build from a .robi file
    // Initializes required variables (Prefab vars set in editor)
    public void PostBuild()
    {
        physContainer = new List<PhysicalContainer>();
        matContainer = new List<MaterialContainer>();

        // Get all renderers
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            MaterialContainer m = new MaterialContainer();
            m.modelRend = r;
            m.defaultMats = r.materials;
            matContainer.Add(m);
        }

        // Get all bodies
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            PhysicalContainer p = new PhysicalContainer();
            p.rigidBody = rb;
            p.collider = rb.gameObject.GetComponents<Collider>();
            physContainer.Add(p);
        }
    }

    private void OnSimPaused()
    {
        unpausedLockState = locked;
        locked = false;
    }

    private void OnSimResumed()
    {
        locked = unpausedLockState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced && other.tag != "Marker")
        {
			collisionCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced && other.tag != "Marker")
        {
			collisionCount--;
        }
    }

    // Update whether object is placeable
    // Should never be called on an object not on the mouse
    public bool updateValidity(bool onGround)
    {
        bool valid = onGround && collisionCount == 0;
        if (valid != isValidPlacement || isPlaced)
        {
            Material newMat = valid ? validMat : invalidMat;
            foreach (MaterialContainer mat in matContainer)
            {
                Material[] newMats = Enumerable.Repeat(newMat, mat.modelRend.materials.Length).ToArray();
                mat.modelRend.materials = newMats;
            }
            isValidPlacement = valid;
            isPlaced = false;
        }
        return valid;
	}

    abstract public void OpenInfoWindow();

    virtual public void OnPointerClick(PointerEventData eventData)
    {
        return;
        /*
        if ( eventData.button == PointerEventData.InputButton.Left && 
             eventData.clickCount == 1 &&
             isPlaced)
        {
            if (isSelected)
            {
                objectSelector.UnselectObject();
                Deselect();
            }
            else
            {
                Select();
            }
        }
        */
    }

    protected void Select()
    {
        isSelected = true;
        objectSelector.NewObjectSelected(this);
        currLayer = 11;
        SetHighlightLayer(gameObject, currLayer);
    }

    public void Deselect()
    {
        isSelected = false;
        currLayer = 0;
        SetHighlightLayer(gameObject, currLayer);
    }

    public void SetHighlightLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetHighlightLayer(child.gameObject, layer);
        }
    }

    public void AttachToMouse()
    {
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.material = validMat;
        }

        foreach(PhysicalContainer phys in physContainer)
        {
            phys.rigidBody.isKinematic = true;
            foreach(Collider c in phys.collider)
                c.isTrigger = true;
        }

        collisionCount = 0;
        isSelected = false;
    }

    public virtual void PlaceObject()
    {
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.materials = mat.defaultMats;
        }
        foreach (PhysicalContainer phys in physContainer)
        {
            if(!SimManager.instance.isPaused)
                phys.rigidBody.isKinematic = false;
            foreach (Collider c in phys.collider)
                c.isTrigger = false;
        }
        isPlaced = true;
        isInit = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!locked && !objectManager.isMouseOccupied && eventData.button == PointerEventData.InputButton.Left)
        {
            objectManager.AddObjectToMouse(this, transform.position.y);
            //Deselect();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}

// Container class for rendering components associated with robot
[Serializable]
public class MaterialContainer
{
    [SerializeField]
    internal Material[] defaultMats;
    [SerializeField]
    internal Renderer modelRend;
}

// Container class for physical components associated with robot
[Serializable]
public class PhysicalContainer
{
    [SerializeField]
    internal Collider[] collider;
    [SerializeField]
    internal Rigidbody rigidBody;
}