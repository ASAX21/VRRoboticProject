using UnityEngine;

// Interface for a class that can receive a file (filepath)
public interface IFileReceiver
{
    GameObject ReceiveFile(string filepath);
}

public class FileFinder : MonoBehaviour
{

    public UIManager uiManager;
    public string m_textPath;

    public string windowTitle;

    public string m_selectPattern;
    public IFileReceiver m_fileReceiver;

    protected FileBrowser m_fileBrowser;

    private FileBrowserType m_type;


    [SerializeField]
    protected Texture2D m_directoryImage,
                        m_fileImage;

    public void Initialise(string selectPattern, string title, FileBrowserType type, IFileReceiver fileReceiver)
    {
        m_selectPattern = selectPattern;
        m_fileReceiver = fileReceiver;
        m_type = type;
        windowTitle = title;
        uiManager = UIManager.instance;
    }

    protected void OnGUI()
    {
        if (m_fileBrowser != null)
        {
            m_fileBrowser.OnGUI();
        }
    }

	public void OpenFileSelection(string dirpath){
		uiManager.OpenWindow(BlockingType.UI);
		m_fileBrowser = new FileBrowser(
			new Rect(100, 100, 600, 500),
            windowTitle,
            m_type,
            FileSelectedCallback,
            dirpath
        );
		m_fileBrowser.SelectionPattern = m_selectPattern;
		m_fileBrowser.DirectoryImage = m_directoryImage;
		m_fileBrowser.FileImage = m_fileImage;
	}

    protected void FileSelectedCallback(string path)
    {
        if(m_fileReceiver == null)
        {
            Debug.Log("Null file receiver");
        }
        m_fileBrowser = null;
        m_textPath = path;
        if(m_textPath != null)
            m_fileReceiver.ReceiveFile(m_textPath);
		uiManager.CloseWindow();
    }
}