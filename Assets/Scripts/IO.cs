using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic; 

public class IO{

	StreamReader theReader;
    private string commentChars;
    private string fileDir;

    private int lineNum = 0;

    public int LineNum
    {
        get
        {
            return lineNum;
        }
    }

    /*  ----- Constructors ----- */
    public IO()
    {
        // do nothing
    }

    // Load an IO, specify characters to indicate comment lines
    public IO(string commentCharacters)
    {
        commentChars = commentCharacters;
    }

    // Destructor
    ~IO()
    {
        theReader.Dispose();
    }

    /* ----- Main functions ----- */

    // Get files in a directory
	public List<string> getFileNames(string directory, string extension)
    {
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/" + directory);
		FileInfo[] files = info.GetFiles(extension);
		List<string> filenames = new List<string> ();
		foreach (FileInfo fi in files) {
			filenames.Add(fi.Name);
		}
		return filenames;
	}

    // Load a file
	public bool Load(string filePath)
    {
        fileDir = Path.GetDirectoryName(filePath);
		try
        {
			theReader = new StreamReader(filePath, Encoding.Default);
			return true;
		}
        catch (System.Exception)
        {
			return false;
		}
	}

    // Read the next line
	public string readLine()
    {
		if (theReader.EndOfStream)
        {
			theReader.Close();
			return "ENDOFFILE";
		}
        lineNum++;
        return theReader.ReadLine();
	}

    // Read the next arguments
    public string[] ReadNextArguments()
    {
        string input = "";
        // Order of these predicates is important: Lazy evaluation ensures input[0] never checked
        // in case of length 0 input[0] == '#' || input[0] == ';'
        while (input.Length == 0 || commentChars.Contains(input[0]))
        {
            if(theReader.EndOfStream)
            {
                theReader.Close();
                return new string[] { "ENDOFFILE" };
            }
            input = theReader.ReadLine();
            input = input.Trim(new char[] { '\t', ' ' });
            lineNum++;
        }
        // Extract arguments from quotations
        string[] args = Regex.Matches(input, "[^\\s\"']+|\"([^\"]*)\"|'([^']*)'")
            .Cast<Match>()
            .Select(m => m.Value)
            .ToArray();
        // Remove leading and trailing whitespace
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = args[i].Trim('"');
            args[i] = args[i].Trim(new char[] { '\n', '\t', ' ' });
        }

        return args;
    }

    // Get the extension of a file
    public string extension(string filePath)
    {
		return Path.GetExtension (filePath);
	}

    // Search for a FILE
    // Check the input file as absolute path first
    // Else check input file and given start path (directory of sim file loaded from)
    // Else check from the home directory
    public string SearchForFile(string filename, string searchDir)
    {
        if (File.Exists(Path.GetFullPath(filename)))
        {
            return Path.GetFullPath(filename);
        }
        else if (File.Exists(Path.Combine(fileDir, filename)))
        {
            return Path.Combine(fileDir, filename);
        }
        else if(searchDir != "" && File.Exists(Path.Combine(searchDir, filename)))
        {
            return Path.Combine(searchDir, filename);
        }
        else if (File.Exists(Path.Combine(SettingsManager.instance.GetSetting("homedir", ""), filename)))
        {
            return Path.Combine(SettingsManager.instance.GetSetting("homedir", ""), filename);
        }
        else
            return "";
    }
}