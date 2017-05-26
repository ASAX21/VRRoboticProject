using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBarManager : MonoBehaviour {

    public static MenuBarManager instance = null;

    public bool isMenuOpen = false;
    public MenuBarButton currentActive = null;

    public Color defaultColor;
    public Color hoverColor;
    public Color selectedColor;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public void BeginOpenMenu(MenuBarButton button)
    {
        currentActive = button;
    }

    public void HoverOverButton(MenuBarButton button)
    {
        currentActive.CloseMenu();
        button.OpenMenu();
        currentActive = button;     
    }
}
