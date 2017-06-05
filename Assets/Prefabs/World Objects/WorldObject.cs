using UnityEngine;
using UnityEngine.EventSystems;

public class WorldObject : PlaceableObject, IPointerClickHandler {

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1 && !isWindowOpen)
        {
            isWindowOpen = true;
            objectSelector.DisplayWorldObjInfoWindow(this);
        }
        base.OnPointerClick(eventData);
    }

}
