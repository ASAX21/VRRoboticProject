using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class MarkerWindow : MonoBehaviour {

    public Marker marker;
    private SpriteRenderer rend;
    private Color myColor;

    public Image colorDisplay;
    public ColorPickerControl colorPicker;

    public void Initialize(Marker newMarker)
    {
        colorPicker = ColorPickerControl.instance;
        marker = newMarker;
        rend = marker.GetComponent<SpriteRenderer>();
        myColor = rend.color;
    }

    public void OpenColorPicker()
    {
        colorPicker.Open(marker, myColor, SetMarkerColor);
    }

    public void SetMarkerColor(Color color)
    {
        myColor = color;
        colorDisplay.color = myColor;
        rend.color = myColor;
    }

    public void CloseWindow()
    {
        marker.isWindowOpen = false;
        gameObject.SetActive(false);
    }

    public void DeleteMarker()
    {
        Destroy(marker.gameObject);
        Destroy(this.gameObject);
    }
}
