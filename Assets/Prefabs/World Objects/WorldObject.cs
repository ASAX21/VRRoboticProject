using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : PlaceableObject, IPointerClickHandler {

    public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            objectSelector.DisplayWorldObjInfoWindow(this);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1 && !isWindowOpen)
        {
            OpenInfoWindow();
        }
        base.OnPointerClick(eventData);
    }
}
