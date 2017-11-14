﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Marker : PlaceableObject, IPointerClickHandler
{
    public MarkerWindow myWindow;

    [SerializeField]
    private SpriteRenderer rend;

    override public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            if (myWindow == null)
            {
                myWindow = Instantiate(UIManager.instance.markerWindowPrefab, UIManager.instance.gameWindowContainer);
                myWindow.Initialize(this);
            }
            else
                myWindow.gameObject.SetActive(true);
        }
    }

    override public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("hello");
        if (eventData.clickCount > 1 && !isWindowOpen)
            OpenInfoWindow();
        base.OnPointerClick(eventData);
    }

    override public void PlaceObject()
    {
        foreach (MaterialContainer mat in matContainer)
        {
            mat.modelRend.materials = mat.defaultMats;
        }
        isPlaced = true;
        isInit = true;
    }

    public void SetColor(Color color)
    {
        rend.color = color;
    }
}
