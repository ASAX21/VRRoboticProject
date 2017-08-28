using System;
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

    // Placement variables
    public bool isInit = false;
    public bool isPlaced = false;
    public bool isSelected = false;
    public bool locked = false;
    public bool unpausedLockState = false;

    private LayerMask currLayer = 0;
	public int collisionCount = 0;

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

    private void Awake()
    {
        if (centreOfMass != Vector3.zero)
        {
            GetComponent<Rigidbody>().centerOfMass = centreOfMass;
        }
    }

    private void Start()
    {
        objectSelector = ObjectSelector.instance;
        objectManager = ObjectManager.instance;
        validMat = ObjectManager.instance.validMat;
        invalidMat = ObjectManager.instance.invalidMat;
        SimManager.instance.OnPause += OnSimPaused;
        SimManager.instance.OnResume += OnSimResumed;
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
       // rigidBody = gameObject.GetComponent<Rigidbody>();
       // objCollider = gameObject.GetComponent<Collider>();
       // matContainer = new List<MaterialContainer>();
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
        if (!isPlaced)
        {
			collisionCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced)
        {
			collisionCount--;
        }
    }

	public bool updateValidity(bool onGround){
		bool valid = onGround && collisionCount == 0;
		Material newMat = valid ? validMat : invalidMat;
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.material = newMat;
        }
        return valid;
	}

    public virtual void OnPointerClick(PointerEventData eventData)
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
            phys.collider.isTrigger = true;
            phys.rigidBody.isKinematic = true;
        }

        collisionCount = 0;
        isPlaced = false;
        isSelected = false;
    }

    public void PlaceObject()
    {
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.material = mat.defaultMat;
        }
        foreach (PhysicalContainer phys in physContainer)
        {
            phys.collider.isTrigger = false;
            if(!SimManager.instance.isPaused)
                phys.rigidBody.isKinematic = false;
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
    internal Material defaultMat;
    [SerializeField]
    internal Renderer modelRend;
}

// Container class for physical components associated with robot
[Serializable]
public class PhysicalContainer
{
    [SerializeField]
    internal Collider collider;
    [SerializeField]
    internal Rigidbody rigidBody;
}