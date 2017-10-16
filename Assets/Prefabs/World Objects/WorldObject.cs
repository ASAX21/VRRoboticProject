using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : PlaceableObject, IPointerClickHandler {

    WorldObjInspectorWindow myWindow;

    override public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            if (myWindow == null)
            {
                myWindow = Instantiate(UIManager.instance.worldObjInspectorWindowPrefab, UIManager.instance.gameWindowContainer);
                myWindow.worldObj = this;
                myWindow.Initialize();
            }
            else
                myWindow.Open();
        }
    }

    override public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1 && !isWindowOpen)
        {
            OpenInfoWindow();
        }
        base.OnPointerClick(eventData);
    }

    private void OnDestroy()
    {
        if (myWindow != null)
            Destroy(myWindow.gameObject);
    }
}
