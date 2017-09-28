using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tab : MonoBehaviour, IPointerDownHandler
{
    public TabWindow myWindow;

    public Text tabText;
    public Image tabBack;

    public GameObject content;

    // Use this for initialization
    void Start () {
        if (myWindow.openTab != this)
        {
            tabBack.color = UIManager.instance.windowHeaderColor;
            tabText.color = UIManager.instance.windowHeaderTextColor;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetColor(Color text, Color back)
    {
        tabText.color = text;
        tabBack.color = back;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (myWindow.openTab == this)
            return;

        myWindow.CloseAllContent();
        myWindow.openTab = this;
        content.SetActive(true);
        SetColor(Color.black, Color.white);
    }
}
