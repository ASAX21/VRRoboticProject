using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Object in the scene that can be placed with the ObjectManager
// Robots, cans, cubes, etc.
public abstract class PlaceableObject : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ObjectSelector objectSelector;
    public ObjectManager objectManager;

    public bool isPlaced = false;
    public bool isSelected = false;
    public bool locked = false;
    public int currLayer = 0;
	public int collisionCount = 0;

    public int objectID;

    public Rigidbody rigidBody;
    [SerializeField]
    protected Collider objCollider;

    public GameObject modelContainer;
    [SerializeField]
    private List<Material> defaultMats;
    private Material validMat;
    private Material invalidMat;

    public void PostBuild()
    {
        modelContainer = transform.Find("Model").gameObject;
        rigidBody = gameObject.GetComponent<Rigidbody>();
        objCollider = gameObject.GetComponent<Collider>();       
        validMat = ObjectManager.instance.validMat;
        invalidMat = ObjectManager.instance.invalidMat;
        defaultMats = new List<Material>();
        foreach (Transform child in modelContainer.transform)
        {
            Renderer childRend = child.GetComponent<Renderer>();
            defaultMats.Add(childRend.material);
        }
    }

    private void Start()
    {
        objectSelector = ObjectSelector.instance;
        objectManager = ObjectManager.instance;
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
		foreach (Transform child in modelContainer.transform) {
			child.GetComponent<Renderer> ().material = newMat;
		}
		return valid;
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isPlaced)
            {
                if (isSelected)
                {
                    objectSelector.UnselectObject();
                }
                else
                {
                    Select();
                }
            }
        }
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
        foreach (Transform child in modelContainer.transform)
        {
            child.GetComponent<Renderer>().material = validMat;
        }
		collisionCount = 0;
        objCollider.isTrigger = true;
        rigidBody.isKinematic = true;
        isPlaced = false;
        isSelected = false;
    }

    public void PlaceObject()
    {
        int i = 0;
        foreach (Transform child in modelContainer.transform)
        {
            child.GetComponent<Renderer>().material = defaultMats[i];
            i++;
        }
        objCollider.isTrigger = false;
        rigidBody.isKinematic = false;
        isPlaced = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGING!");
        objectManager.AddObjectToMouse(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("NotDraging!");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}