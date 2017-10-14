using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input manager handles key shortcuts

public class InputManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            SimManager.instance.RestoreState();
        }
	}
}
