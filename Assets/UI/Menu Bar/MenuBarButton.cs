using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuBarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    public static MenuBarButton currentOpenMenu;
    private static bool isMenuOpen = false;

    public Image image;

    // Colors retreived from Manager 
    private Color defaultColor;
    private Color hoverColor;
    private Color selectedColor;

    public GameObject menu;

    public bool isSelected = false;

    // Initialize values
    private void Start ()
    {
        defaultColor = MenuBarManager.instance.menuBarColor;
        hoverColor = MenuBarManager.instance.hoverColor;
        selectedColor = MenuBarManager.instance.selectedColor;
    }

    public void OpenMenu()
    {     
        menu.SetActive(true);
        isSelected = true;
        isMenuOpen = true;
        currentOpenMenu = this;
        image.color = selectedColor;
        BlockingPanel.instance.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
        isSelected = false;
        isMenuOpen = false;
        image.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
            return;

        if (isMenuOpen)
        {
            currentOpenMenu.CloseMenu();
            OpenMenu();
            currentOpenMenu = this;
        }
        else
        {
            image.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isMenuOpen)
            image.color = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSelected)
        {
            CloseMenu();               
            isMenuOpen = false;
        }
        else
        {
            OpenMenu();
            isMenuOpen = true;
            currentOpenMenu = this;
        }  
    }
}
