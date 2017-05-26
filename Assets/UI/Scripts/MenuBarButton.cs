using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MenuBarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    private MenuBarManager menuBarManager;

    [SerializeField]
    private Image image;
    public Color defaultColor;
    public Color hoverColor;
    public Color selectedColor;

    public GameObject menu;

    public bool isSelected = false;

    // Use this for initialization
    void Start () {
        menuBarManager = MenuBarManager.instance;
	}

    public void OpenMenu()
    {     
        menu.SetActive(true);
        isSelected = true;
        image.color = selectedColor;
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        isSelected = false;
        image.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuBarManager.isMenuOpen)
        {
            menuBarManager.HoverOverButton(this);
        }
        else
        {
            image.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!menuBarManager.isMenuOpen)
            image.color = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSelected)
        {
            CloseMenu();               
            menuBarManager.isMenuOpen = false;
        }
        else
        {
            OpenMenu();
            menuBarManager.currentActive = this;         
            menuBarManager.isMenuOpen = true;
        }  
    }
}
