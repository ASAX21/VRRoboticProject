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
}
