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

    public int objectID;

    // Placement variables
    public bool isPlaced = false;
    public bool isSelected = false;
    public bool locked = false;
    public int currLayer = 0;
	public int collisionCount = 0;
    [SerializeField]
    private List<MaterialContainer> matContainer;
    private Material validMat;
    private Material invalidMat;
    
    // Unity Components
    [SerializeField]
    protected Collider objCollider;
    public Rigidbody rigidBody;

    // UI Variables
    public bool isWindowOpen = false;

    public void PostBuild()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        objCollider = gameObject.GetComponent<Collider>();       
    }

    private void Start()
    {
        objectSelector = ObjectSelector.instance;
        objectManager = ObjectManager.instance;
        validMat = ObjectManager.instance.validMat;
        invalidMat = ObjectManager.instance.invalidMat;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Single or First click
            if (eventData.clickCount == 1)
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
            // Double click (or more)
            else if (!isWindowOpen)
            {
                Select();
                isWindowOpen = true;
                if(this is Robot)
                    objectSelector.DisplayRobotInfoWindow(this as Robot);
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
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.material = validMat;
        }

        collisionCount = 0;
        objCollider.isTrigger = true;
        rigidBody.isKinematic = true;
        isPlaced = false;
        isSelected = false;
    }

    public void PlaceObject()
    {
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.material = mat.defaultMat;
        }
        objCollider.isTrigger = false;
        rigidBody.isKinematic = false;
        isPlaced = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!locked)
        {
            objectManager.AddObjectToMouse(this);
            Deselect();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}

[Serializable]
internal class MaterialContainer
{
    [SerializeField]
    internal Material defaultMat;
    [SerializeField]
    internal Renderer modelRend;
}