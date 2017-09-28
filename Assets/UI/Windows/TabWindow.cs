using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TabWindow : MonoBehaviour {

    [SerializeField]
    protected Text windowTitle;
    [SerializeField]
    protected Image windowHeader;

    [Header("Contents")]
    public List<Tab> tabs;

    public Tab openTab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CloseAllContent()
    {
        foreach (Tab go in tabs)
        {
            go.content.SetActive(false);
            go.SetColor(UIManager.instance.windowHeaderTextColor, UIManager.instance.windowHeaderColor);
        }
    }
}
