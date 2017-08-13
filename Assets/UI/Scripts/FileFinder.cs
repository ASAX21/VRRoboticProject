using UnityEngine;

public interface IFileReceiver
{
    GameObject ReceiveFile(string filepath);
}

public class FileFinder : MonoBehaviour
{

    public UIManager uiManager;
    public string m_textPath;

    public string m_selectPattern;
    public IFileReceiver m_fileReceiver;

    protected FileBrowser m_fileBrowser;

    [SerializeField]
    protected Texture2D m_directoryImage,
                        m_fileImage;

    public FileFinder Initialise(string selectPattern, IFileReceiver fileReceiver)
    {
        m_selectPattern = selectPattern;
        m_fileReceiver = fileReceiver;
        uiManager = UIManager.instance;
        return this;
    }

    protected void OnGUI()
    {
        if (m_fileBrowser != null)
        {
            m_fileBrowser.OnGUI();
        }
    }

	public void OpenFileSelection(){
		uiManager.openWindow();
		m_fileBrowser = new FileBrowser(
			new Rect(100, 100, 600, 500),
			"Select a File",
			FileSelectedCallback
		);
		m_fileBrowser.SelectionPattern = m_selectPattern;
		m_fileBrowser.DirectoryImage = m_directoryImage;
		m_fileBrowser.FileImage = m_fileImage;
        Debug.Log("Opened file browser");
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
		uiManager.closeWindow();
    }
}