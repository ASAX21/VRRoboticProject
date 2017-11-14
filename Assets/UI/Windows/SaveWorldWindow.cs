﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveWorldWindow : Window, IFileReceiver
{
    [SerializeField]
    private InputField directoryInput, fileNameInput;

    [SerializeField]
    private Button saveButton;

    void Start()
    {
        directoryInput.text = SettingsManager.instance.GetSetting("simdir", "");
    }

    void OnEnable()
    {
        ValidatePathInput();
    }

    // Sim Dir Directory
    public void OpenDirectoryButton()
    {
        UIManager.instance.directoryFinder.Initialise("*", "Select Directory", FileBrowserType.Directory, this);
        UIManager.instance.directoryFinder.OpenFileSelection(SettingsManager.instance.GetSetting("worlddir", ""));
    }

    public void InputDirectory(string dirpath)
    {
        if(Directory.Exists(dirpath))
        {
            directoryInput.text = dirpath;
        }
        else
            Debug.Log("Bad Dirpath");
        ValidatePathInput();
    }

    // Check if path is valid
    public void ValidatePathInput()
    {
        string filename = Path.Combine(directoryInput.text, fileNameInput.text);
        FileInfo fi = null;
        try
        {
            fi = new System.IO.FileInfo(filename);
        }
        catch(ArgumentException) { }
        catch(System.IO.PathTooLongException) { }
        catch(NotSupportedException) { }
        if(ReferenceEquals(fi, null) || fileNameInput.text == "")
        {
            saveButton.interactable = false;
        }
        else
        {
            saveButton.interactable = true;
        }
    }

    // Save the file
    public void Save()
    {
        string filename = fileNameInput.text;
        if(Path.GetExtension(filename) != ".wld")
            filename += ".wld";
        SimManager.instance.SaveWorld(Path.Combine(directoryInput.text, filename));
    }

    // Callback for select directory
    public GameObject ReceiveFile(string dirpath)
    {
        InputDirectory(dirpath);
        return null;
    }
}