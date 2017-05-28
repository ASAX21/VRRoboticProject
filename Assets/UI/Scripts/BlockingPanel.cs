using UnityEngine;
using UnityEngine.EventSystems;

public class BlockingPanel : MonoBehaviour, IPointerDownHandler {

    public static BlockingPanel instance { get; private set; }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MenuBarItem.currentOpenSubmenu != null)
            MenuBarItem.currentOpenSubmenu.CloseSubMenu();
        if (MenuBarButton.currentOpenMenu != null)
            MenuBarButton.currentOpenMenu.CloseMenu();
        gameObject.SetActive(false);
    }
}
