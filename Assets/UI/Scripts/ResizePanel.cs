using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler {
	
	public Vector2 minSize = new Vector2 (100, 100);
	public Vector2 maxSize = new Vector2 (400, 400);

    public Texture2D resizeCursor;
    private Vector2 cursorHotspot;
    private bool isCursorOutsideResizeArea = true;
    private bool isResizing = false;
	
	private RectTransform panelRectTransform;
	private Vector2 originalLocalPointerPosition;
	private Vector2 originalSizeDelta;
	
	void Awake () {
		panelRectTransform = transform.parent.GetComponent<RectTransform> ();
	}

    private void Start()
    {
        cursorHotspot = new Vector2(resizeCursor.width / 2, resizeCursor.height / 2);
    }
    	
	public void OnPointerDown (PointerEventData data) {
        isResizing = true;
        originalSizeDelta = panelRectTransform.sizeDelta;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}

    public void OnPointerUp(PointerEventData eventData)
    {
        isResizing = false;
        if(isCursorOutsideResizeArea)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null)
			return;
		
		Vector2 localPointerPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
		Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
		
		Vector2 sizeDelta = originalSizeDelta + new Vector2 (offsetToOriginal.x, -offsetToOriginal.y);
		sizeDelta = new Vector2 (
			Mathf.Clamp (sizeDelta.x, minSize.x, maxSize.x),
			Mathf.Clamp (sizeDelta.y, minSize.y, maxSize.y)
		);
		
		panelRectTransform.sizeDelta = sizeDelta;
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorOutsideResizeArea = false;
        Cursor.SetCursor(resizeCursor, cursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorOutsideResizeArea = true;
        if(!isResizing)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}