using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class MarkerWindow : Window {

    public Marker marker;
    private SpriteRenderer rend;
    private Color myColor;

    public Image colorDisplay;
    public ColorPickerControl colorPicker;

    private bool colPickerOpen = false;

    public void Initialize(Marker newMarker)
    {
        marker = newMarker;
        rend = marker.GetComponent<SpriteRenderer>();
        SetMarkerColor(newMarker.mColor);
    }

    public void CloseColorPicker()
    {
        colPickerOpen = false;
    }

    public void OpenColorPicker()
    {
        if(colPickerOpen)
            colorPicker.transform.SetAsLastSibling();
        else
        {
            colPickerOpen = true;
            colorPicker = Instantiate(UIManager.instance.colorPickerPrefab, UIManager.instance.gameWindowContainer);
            colorPicker.Open(myColor, SetMarkerColor, CloseColorPicker);
        }
    }

    public void SetMarkerColor(Color color)
    {
        myColor = color;
        marker.mColor = color;
        colorDisplay.color = myColor;
        rend.color = myColor;
    }

    override public void Close()
    {
        colPickerOpen = false;
        marker.isWindowOpen = false;
        base.Close();
    }

    public void DeleteMarker()
    {
        Destroy(marker.gameObject);
        Destroy(this.gameObject);
    }
}
