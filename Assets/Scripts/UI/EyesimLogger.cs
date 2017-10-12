using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EyesimLogger : MonoBehaviour
{
    // Storage for log messages displayed in-app
    public int maxLogEntries;
    public string[] currentLog;
    public int currentFront;

    // Event to fire when log is changed
    public delegate void LogUpdated();
    event LogUpdated logUpdatedEvent;

    // File access
    bool logfileOpen = false;
    FileStream logFile;
    StreamWriter logWriter;

    public void TEST()
    {
        CreateNewLogFile("test.txt");
        WriteToLogFile("hi there man!");
        WriteToLogFile("hi there man 2!!");
        CloseLogFile();
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
        currentLog[currentFront] = text;
        currentFront = (currentFront + 1) % maxLogEntries;
        if (logfileOpen)
            WriteToLogFile(text);
        if(logUpdatedEvent != null)
            logUpdatedEvent.Invoke();
    }
}
