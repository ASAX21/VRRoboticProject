using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWorldWindow : MonoBehaviour {

    [SerializeField]
    private InputField widthInput, heightInput;

	public void ConfirmCreateBox()
    {
        int width, height;
        if (!int.TryParse(widthInput.text, out width)) {
            Debug.Log("Create New Box Dialogue: Failed to parse width");
            return;
        }
        if (!int.TryParse(heightInput.text, out height))
        {
            Debug.Log("Create New Box Dialogue: Failed to parse height");
            return;
        }
        EyesimLogger.instance.Log("Creating new emtpy world " + width + " x " + height);
        SimManager.instance.CreateNewBox(width, height);
        Cancel();
    }

    public void Cancel()
    {
        widthInput.text = "";
        heightInput.text = "";
        gameObject.SetActive(false);
    }
}
