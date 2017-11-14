using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

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
    SaveSimWindow saveSimWindow;
    [SerializeField]
    SaveWorldWindow saveWorldWindow;
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
    public ColorPickerControl colorPickerPrefab;

    // Color Scheme
    [Header("Colors")]
    public Color windowHeaderColor;
    public Color windowHeaderTextColor;

    // FileFinder
    [Header("File Finders")]
    public FileFinder fileFinder;
    public FileFinder directoryFinder;

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
        fileFinder.Initialise("*", "Select Sim File", FileBrowserType.File, SimReader.instance);
        fileFinder.OpenFileSelection(SettingsManager.instance.simDirectory);
    }

    public void LoadWorldFile()
    {
        fileFinder.Initialise("*", "Select Control Program", FileBrowserType.File, WorldBuilder.instance);
        fileFinder.OpenFileSelection(SettingsManager.instance.worldDirectory);
    }

    public void CreateWorld()
    {
        createWorldWindow.gameObject.SetActive(true);
        createWorldWindow.transform.SetAsLastSibling();
    }

    public void LoadRobotFile()
    {
        fileFinder.Initialise("*", "Select Sim File", FileBrowserType.File, RobotLoader.instance);
        fileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void LoadControlProgram(Robot robot)
    {
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            fileFinder.Initialise("*", "Select Control Program", FileBrowserType.File, robot);
        else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            fileFinder.Initialise("*", "Select Control Program", FileBrowserType.File, robot);
        else
            return;
        fileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void LoadCustomObject()
    {
        fileFinder.Initialise("*.esObj", "Select Custom Object", FileBrowserType.File, ObjectManager.instance);
        fileFinder.OpenFileSelection(SettingsManager.instance.homeDirectory);
    }

    public void SaveSimFile()
    {
        saveSimWindow.Open();
        saveSimWindow.transform.SetAsLastSibling();
    }
    
    public void SaveWorldFile()
    {
        saveWorldWindow.Open();
        saveWorldWindow.transform.SetAsLastSibling();
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
