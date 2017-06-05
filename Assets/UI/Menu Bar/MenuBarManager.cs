using UnityEngine;

public class MenuBarManager : MonoBehaviour {

    public static MenuBarManager instance { get; private set; }

    // Back blocking panel permits access to the menu bar
    public bool isMenuOpen = false;
    public MenuBarButton currentActive = null;

    public MenuBarItem currentSubmenu;

    public GameObject blockingPanel;

    public Color menuBarColor;
    public Color menuItemColor;
    public Color hoverColor;
    public Color selectedColor;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    /*
    public void BeginOpenMenu(MenuBarButton button)
    {
        currentActive = button;
        blockingPanel.SetActive(true);
    }

    public void OpenSubMenu(MenuBarItem submenu)
    {
        if (currentSubmenu != null)
            currentSubmenu.CloseSubMenu();
        currentSubmenu = submenu;
    }

    public void CloseSubMenu()
    {
        if (currentSubmenu != null)
            currentSubmenu.CloseSubMenu();
     }

    public void CloseAllMenus()
    {
        currentActive.CloseMenu();
        isMenuOpen = false;
        currentActive = null;
        currentSubmenu = null;
        blockingPanel.SetActive(false);
    }

    public void HoverOverButton(MenuBarButton button)
    {
        currentActive.CloseMenu();
        button.OpenMenu();
        currentActive = button;     
    }
    */
}
