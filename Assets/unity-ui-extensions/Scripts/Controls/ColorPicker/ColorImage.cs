﻿///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/


namespace UnityEngine.UI.Extensions.ColorPicker
{
    [RequireComponent(typeof(Image))]
public class ColorImage : MonoBehaviour
{
    public ColorPickerControl picker;
    [SerializeField]
    private Image image;

    private void OnDestroy()
    {
        picker.onValueChanged.RemoveListener(ColorChanged);
    }

    public void ColorChanged(Color newColor)
    {
        image.color = newColor;
    }
}
}