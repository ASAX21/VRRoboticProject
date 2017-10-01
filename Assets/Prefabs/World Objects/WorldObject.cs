using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : PlaceableObject, IPointerClickHandler {

    override public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            objectSelector.DisplayWorldObjInfoWindow(this);
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
}
