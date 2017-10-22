using UnityEngine;
using UnityEngine.UI;

public class LogWindow : Window {

    [SerializeField]
    private int maxLogEntries = 100;
    private int currentLogEntries = 0;

    public Text logText;

    public void UpdateLogDisplay(string newEntry)
    {
        if (++currentLogEntries > maxLogEntries)
            RemoveFirstLogEntry();
        logText.text += newEntry + "\n";
    }

    private void RemoveFirstLogEntry()
    {
        int index = logText.text.IndexOf("\n");
        if (index == -1)
            return;
        else
            logText.text = logText.text.Substring(index + 1);
    }

}
