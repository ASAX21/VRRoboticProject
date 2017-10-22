using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EyesimLogger : MonoBehaviour
{
    public static EyesimLogger instance;

    // Storage for log messages displayed in-app
    public int maxLogEntries = 100;
    public string[] currentLog;
    public int currentFront;
    public Text uiLog;

    // Event to fire when log is changed
    public delegate void LogUpdated(string logText);
    public event LogUpdated logUpdatedEvent;

    // File access
    bool logfileOpen = false;
    FileStream logFile;
    StreamWriter logWriter;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        currentLog = new string[maxLogEntries];
    }

    public void CreateNewLogFile(string filename)
    {
        string path = Path.Combine(SettingsManager.instance.homeDirectory, "log");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path = Path.Combine(path, filename);
        Debug.Log(path);
        logFile = File.Open(path, FileMode.Create);
        logWriter = new StreamWriter(logFile, System.Text.Encoding.ASCII);
        logWriter.Write("Log File Created " + DateTime.Now.ToString());
        logfileOpen = true;
    }

    public void WriteToLogFile(string text)
    {
        if (!logFile.CanWrite)
            return;
        logWriter.WriteLine(text);
    }

    public void CloseLogFile()
    {
        if (logFile == null)
            return;
        logWriter.Close();
        logFile.Close();
        logfileOpen = false;
    }

    public void Log(string text)
    {
        string time = "[" + ((int) Time.time/3600).ToString("00") + 
            ":" + ((int) Time.time%3600/60).ToString("00") +
            ":" + ((int) Time.time % 60).ToString("00") + "] ";
        currentLog[currentFront] = time + text;
        currentFront = (currentFront + 1) % maxLogEntries;
        if (logfileOpen)
            WriteToLogFile(time + text);
        if(logUpdatedEvent != null)
            logUpdatedEvent.Invoke(time + text);
    }
}
