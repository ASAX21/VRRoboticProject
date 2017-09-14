using UnityEngine;
using UnityEngine.UI;

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

    // Manipulateable Windows
    [Header("Submenus")]
    public Transform objectsSubmenu;
    public MenuBarItem objectMenuButton;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public void AddCustomObjectToMenu(string name, int index)
    {
        MenuBarItem newObjectButton = Instantiate(objectMenuButton, objectsSubmenu);
        newObjectButton.GetComponentInChildren<Text>().text = "Add " + name;
        newObjectButton.callbackIndex = index;
        newObjectButton.callback.AddListener(newObjectButton.CustomObjectCallback);
    }
}
