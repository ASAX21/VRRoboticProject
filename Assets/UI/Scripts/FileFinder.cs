using UnityEngine;

public interface IFileReceiver
{
    GameObject ReceiveFile(string filepath);
}

public class FileFinder : MonoBehaviour
{

    public UIManager uiManager;
    public string m_textPath;

    private string m_selectPattern;
    private string m_buttonText;
    private IFileReceiver m_fileReceiver;
    private float m_x;
    private float m_y;

    protected FileBrowser m_fileBrowser;

    [SerializeField]
    protected Texture2D m_directoryImage,
                        m_fileImage;

    public FileFinder Initialise(string selectPattern, string buttonText, IFileReceiver fileReceiver, float x, float y)
    {
        m_selectPattern = selectPattern;
        m_buttonText = buttonText;
        m_fileReceiver = fileReceiver;
        m_x = x;
        m_y = y;
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