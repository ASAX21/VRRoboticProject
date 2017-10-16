using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

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

    public virtual void Open()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}

