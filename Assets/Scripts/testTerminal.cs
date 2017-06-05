using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class testTerminal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ProcessStartInfo proc = new ProcessStartInfo();
		proc.FileName = "open";
		proc.WorkingDirectory = "/Users/JoelFrewin/Desktop";
		proc.Arguments = "./file";
		Process.Start(proc);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
