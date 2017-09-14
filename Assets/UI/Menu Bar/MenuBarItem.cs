using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuBarItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public static MenuBarItem currentOpenSubmenu;

    // Can the button be pressed
    public bool isDisabled;

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
    public int callbackIndex;
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
        if (isDisabled)
        {
            transform.GetChild(0).GetComponent<Text>().color = Color.gray;
        }
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
        if (isDisabled || hasSubmenu)
            return;

		if (isSubmenuItem)
        {
            parentItem.CloseSubMenu();
        }
        parentMenu.CloseMenu();
        image.color = defaultColor;
        callback.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDisabled)
            return;

        image.color = hoverColor;
        if (hasSubmenu)
		{
			OpenSubMenu();
			currentOpenSubmenu = this;
		}
        else if (!isSubmenuItem)
        {
            if(currentOpenSubmenu != null)
                currentOpenSubmenu.CloseSubMenu();
        }
     
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisabled)
            return;

		if (hasSubmenu)
		{
			CloseSubMenu();
			currentOpenSubmenu = null;
		}
        if (!isSubmenuOpen)
            image.color = defaultColor;
    }

    private void OnDisable()
    {
        image.color = defaultColor;
    }

    public void CustomObjectCallback()
    {
        ObjectManager.instance.AddCustomObjectToScene(callbackIndex, "");
    }
}