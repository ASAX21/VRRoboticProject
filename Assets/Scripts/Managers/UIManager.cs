using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance { get; private set; }

    public GameObject fileFinderPrefab;

    // Front windows must be closed before anything else can be interacted with
    // Front blocking panel prevents opening menus
    public bool windowOpen = false;
    public GameObject blockingPanel;

    // Game Windows
    [Header("Game Windows")]
    [SerializeField]
    ViewRobotsWindow viewRobotsWindow;
    [SerializeField]
    ViewWorldObjectsWindow viewWorldObjectsWindow;

    // Builders
    private WorldBuilder worldBuilder;
    private RobotBuilder robotBuilder;
    private SimReader simBuilder;

    // FileFinder
    [Header("File Finders")]
    public FileFinder worldFileFinder;
    public FileFinder robotFileFinder;
    public FileFinder controlFileFinder;
    public FileFinder simFileFinder;

	private Button loadworld;
	private Button loadrobot;
	private Transform robotList;
	private Transform clientList;

    // Enforce singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(this);
    }

    void Start()
    {
        worldBuilder = WorldBuilder.instance;
        robotBuilder = RobotBuilder.instance;
        simBuilder = SimReader.instance;
		worldFileFinder.Initialise("*", worldBuilder);
        robotFileFinder.Initialise("*.robi", robotBuilder);
        simFileFinder.Initialise("*.sim", simBuilder);
    }

	public void openWindow(){
		windowOpen = true;
		blockingPanel.SetActive(true);
	}

	public void closeWindow(){
		windowOpen = false;
		blockingPanel.SetActive(false);
	}

    public void LoadSimFile()
    {
        simFileFinder.OpenFileSelection();
    }

    public void LoadWorldFile()
    {
        worldFileFinder.OpenFileSelection();
    }

    public void LoadRobotFile()
    {
        robotFileFinder.OpenFileSelection();
    }

    public void LoadControlProgram(Robot robot)
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            controlFileFinder.Initialise("*.exe", robot);
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            controlFileFinder.Initialise("*", robot);
        controlFileFinder.OpenFileSelection();
    }

    public void OpenViewRobotWindow()
    {
        if (viewRobotsWindow.gameObject.activeInHierarchy)
            viewRobotsWindow.transform.SetAsFirstSibling();
        else
            viewRobotsWindow.gameObject.SetActive(true);
    }

    public void OpenViewObjectsWindow()
    {
        if (viewWorldObjectsWindow.gameObject.activeInHierarchy)
            viewWorldObjectsWindow.transform.SetAsFirstSibling();
        else
            viewWorldObjectsWindow.gameObject.SetActive(true);
    }
}
