using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : PlaceableObject, IPointerClickHandler {

    WorldObjInspectorWindow myWindow;
    public Color myColor;

    override internal void Start()
    {
        myColor = Color.white;
        base.Start();
    }

    override public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            if (myWindow == null)
            {
                myWindow = Instantiate(UIManager.instance.worldObjInspectorWindowPrefab, UIManager.instance.gameWindowContainer);
                myWindow.worldObj = this;
                myWindow.Initialize();
            }
            else
                myWindow.Open();
        }
    }

    override public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1 && !isWindowOpen)
        {
            OpenInfoWindow();
        }
        base.OnPointerClick(eventData);
    }

    private void OnDestroy()
    {
        if (myWindow != null)
            Destroy(myWindow.gameObject);
    }

    public void ChangeColor(Color newColor)
    {
        // Set color of each default material
        foreach(MaterialContainer m in matContainer)
        {
            for(int i = 0; i < m.defaultMats.Length; i++)
                m.defaultMats[i].color = newColor;
        }

        myColor = newColor;
        myWindow.colorDisplay.color = myColor;
    }
}
