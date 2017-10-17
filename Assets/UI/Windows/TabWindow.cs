using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TabWindow : Window {

    [SerializeField]
    protected Text windowTitle;
    [SerializeField]
    protected Image windowHeader;

    [Header("Contents")]
    public List<Tab> tabs;

    public Tab openTab;

    public void CloseAllContent()
    {
        foreach (Tab go in tabs)
        {
            go.content.SetActive(false);
            go.SetColor(UIManager.instance.windowHeaderTextColor, UIManager.instance.windowHeaderColor);
        }
    }

    public override void Close()
    {
        base.Close();
    }
}
