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

    // FileFinder
    [Header("File Finders")]
    public FileFinder worldFileFinder;
    public FileFinder robotFileFinder;
    public FileFinder controlFileFinder;

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
		worldFileFinder.Initialise("*", worldBuilder);
        robotFileFinder.Initialise("*.robi", robotBuilder);
    }

	public void openWindow(){
		windowOpen = true;
		blockingPanel.SetActive(true);
	}

	public void closeWindow(){
		windowOpen = false;
		blockingPanel.SetActive(false);
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
        controlFileFinder.Initialise("*.exe", robot);
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
