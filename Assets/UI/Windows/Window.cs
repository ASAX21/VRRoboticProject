using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class Window : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private bool mouseOverWindow = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.preventMouseZoom++;
        mouseOverWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.preventMouseZoom--;
        mouseOverWindow = false;
    }

    private void OnDisable()
    {
        if (mouseOverWindow)
            UIManager.instance.preventMouseZoom--;
    }
}

