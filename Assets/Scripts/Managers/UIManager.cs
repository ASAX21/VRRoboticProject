using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockingType { UI, Scene }

public class UIManager : MonoBehaviour {

    public static UIManager instance { get; private set; }

    public GameObject fileFinderPrefab;

    // Front windows must be closed before anything else can be interacted with
    // Front blocking panel prevents opening menus
    public int preventMouseZoom = 0;
    public bool windowOpen = false;
    public GameObject blockScenePanel;
    public GameObject blockUIPanel;

    [Header("Window Containers")]
    public Transform gameWindowContainer;
    public Transform frontWindowContainer;

    // Game Windows
    [Header("Game Windows")]
    [SerializeField]
    ViewRobotsWindow viewRobotsWindow;
    [SerializeField]
    ViewWorldObjectsWindow viewWorldObjectsWindow;
    [SerializeField]
    Window aboutWindow;
    [SerializeField]
    SettingsWindow settingsWindow;
    [SerializeField]
    CreateWorldWindow createWorldWindow;
    [SerializeField]
    LogWindow logWindow;

    [Header("Window Prefabs")]
    public RobotInspectorWindow robotInspectorWindowPrefab;
    public WorldObjInspectorWindow worldObjInspectorWindowPrefab;
    public MarkerWindow markerWindowPrefab;

    // Color Scheme
    [Header("Colors")]
    public Color windowHeaderColor;
    public Color windowHeaderTextColor;

    // FileFinder
    [Header("File Finders")]
    public FileFinder worldFileFinder;
    public FileFinder robotFileFinder;
    public FileFinder controlFileFinder;
    public FileFinder simFileFinder;
    public FileFinder scriptFileFinder;
    public FileFinder customObjFileFinder;

    // Images of control buttons
    [Header("Sim Control Buttons")]
    public Image pauseButton;
    public Image playButton;
    public Image ffButton;

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
        EyesimLogger.instance.logUpdatedEvent += logWindow.UpdateLogDisplay;
		worldFileFinder.Initialise("*.*", FileBrowserType.File, WorldBuilder.instance);
        robotFileFinder.Initialise("*.robi", FileBrowserType.File, RobotLoader.instance);
        simFileFinder.Initialise("*.sim", FileBrowserType.File, SimReader.instance);
        //scriptFileFinder.Initialise("*.c", FileBrowserType.File, SimManager.instance.osManager);
        customObjFileFinder.Initialise("*.esObj", FileBrowserType.File, ObjectManager.instance);
    }

	public void openWindow(BlockingType type){
		windowOpen = true;
        if (type == BlockingType.UI)
            blockUIPanel.SetActive(true);
        else if (type == BlockingType.Scene)
            blockScenePanel.SetActive(true);
	}

	public void closeWindow(){
		windowOpen = false;
        blockUIPanel.SetActive(false);
		blockScenePanel.SetActive(false);
	}

    // File Menu

    public void LoadSimFile()
    {
        simFileFinder.OpenFileSelection(SettingsManager.instance.simDirectory);
    }

    public void LoadWorldFile()
    {
        worldFileFinder.OpenFileSelection(SettingsManager.instance.worldDirectory);
    }

    public void CreateWorld()
    {
        createWorldWindow.gameObject.SetActive(true);
        createWorldWindow.transform.SetAsLastSibling();
    }

    public void LoadRobotFile()
    {
        robotFileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void LoadControlProgram(Robot robot)
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            controlFileFinder.Initialise("*.exe", FileBrowserType.File, robot);
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            controlFileFinder.Initialise("*", FileBrowserType.File, robot);
        controlFileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void LoadScriptFile()
    {
        scriptFileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void LoadCustomObject()
    {
        customObjFileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void OpenSettings()
    {
        settingsWindow.Open();
        openWindow(BlockingType.Scene);
    }

    // Simulation Menu
    public void OpenViewRobotWindow()
    {
        viewRobotsWindow.Open();
    }

    public void OpenViewObjectsWindow()
    {
        viewWorldObjectsWindow.Open();
    }

    // Help Menu Buttons
    public void OpenUserManual()
    {

    }

    public void OpenAPI()
    {
        Application.OpenURL(@"http://robotics.ee.uwa.edu.au/eyebot7/Robios7.html");
    }

    public void OpenLog()
    {
        logWindow.Open();
    }

    public void OpenAbout()
    {
        aboutWindow.Open();
    }

    // Pause/Resume/FastForward UI Buttons
    public void PauseButton()
    {
        SimManager.instance.PauseSimulation();
        pauseButton.color = new Color(0, 0, 0, 1f);
        playButton.color = new Color(0, 0, 0, 0.2f);
        ffButton.color = new Color(0, 0, 0, 0.2f);
    }

    public void PlayButton()
    {
        SimManager.instance.ResumeSimulation();
        Time.timeScale = 1f;
        pauseButton.color = new Color(0, 0, 0, 0.2f);
        playButton.color = new Color(0, 0, 0, 1f);
        ffButton.color = new Color(0, 0, 0, 0.2f);
    } 

    public void FastForwardButton()
    {
        SimManager.instance.ResumeSimulation();
        Time.timeScale = 2f;
        pauseButton.color = new Color(0, 0, 0, 0.2f);
        playButton.color = new Color(0, 0, 0, 0.2f);
        ffButton.color = new Color(0, 0, 0, 1f);
    }
}
