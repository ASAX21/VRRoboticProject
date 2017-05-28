using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuBarItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public static MenuBarItem currentOpenSubmenu;

    // Variables for colors
    private MenuBarManager menuBarManager;
    private Color hoverColor;
    private Color defaultColor; 
    private Image image;

    // Does this item have a submenu
    public bool hasSubmenu;
    public bool isSubmenuOpen = false;
    public GameObject submenu;

    //  Is this item inside a submenu
    public bool isSubmenuItem;
    private MenuBarItem parentItem;

    // Main parent menu 
    private MenuBarButton parentMenu;

    // Callback function
    public UnityEvent callback;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (isSubmenuItem)
        {
            parentItem = transform.parent.parent.GetComponent<MenuBarItem>();
            parentMenu = parentItem.transform.parent.parent.GetComponent<MenuBarButton>();
        }
        else
            parentMenu = transform.parent.parent.GetComponent<MenuBarButton>();
    }

    private void Start()
    {
        if (menuBarManager == null)
            menuBarManager = MenuBarManager.instance;
        hoverColor = menuBarManager.selectedColor;
        defaultColor = menuBarManager.menuItemColor;
    }

    public void OpenSubMenu()
    {
        submenu.SetActive(true);
        isSubmenuOpen = true;
        image.color = hoverColor;
    }

    public void CloseSubMenu()
    {
        submenu.SetActive(false);
        isSubmenuOpen = false;
        image.color = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (hasSubmenu)
        {
            OpenSubMenu();
            currentOpenSubmenu = this;
        }
        else
        {
            if (isSubmenuItem)
            {
                parentItem.CloseSubMenu();
                parentMenu.CloseMenu();
                
            }
            image.color = defaultColor;
            callback.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSubmenuItem)
        {
            if(currentOpenSubmenu != null)
                currentOpenSubmenu.CloseSubMenu();
        }
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSubmenuOpen)
            image.color = defaultColor;
    }
}