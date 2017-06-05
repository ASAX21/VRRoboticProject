using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationControlWindow : MonoBehaviour {

    private SimManager simManager;

    public Slider simSpeedSlider;
    public InputField simSpeedInput;

    private void Start()
    {
        simManager = SimManager.instance;
    }

    public void SetSimSpeedSlider(float simSpeed)
    {
        simManager.SetSimulationSpeed(simSpeed);
        simSpeedInput.text = simSpeed.ToString();
    }

    public void SetSimSpeedInput(string simSpeed)
    {
        float newSimSpeed;
        if(float.TryParse(simSpeed, out newSimSpeed))
        {
            newSimSpeed = Mathf.Clamp(newSimSpeed, 0, 2f);
            simManager.SetSimulationSpeed(newSimSpeed);
            simSpeedSlider.value = newSimSpeed;
        }
    }
}
