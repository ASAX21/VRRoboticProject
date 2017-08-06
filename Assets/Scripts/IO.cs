using System.Text;
using System.IO;  
using UnityEngine;
using System.Collections.Generic; 

public class IO{

	StreamReader theReader;

	public List<string> getFileNames(string directory, string extension) {
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/" + directory);
		FileInfo[] files = info.GetFiles(extension);
		List<string> filenames = new List<string> ();
		foreach (FileInfo fi in files) {
			filenames.Add(fi.Name);
		}
		return filenames;
	}

	public bool Load(string filePath) {
		Debug.Log(filePath);
		try {
			//theReader = new StreamReader(Application.persistentDataPath + "/" + fileName, Encoding.Default);
			theReader = new StreamReader(filePath, Encoding.Default);
			return true;
		} catch (System.Exception){
			return false;
		}
	}

	public string readLine() {
		if (theReader.EndOfStream) {
			theReader.Close();
			return "ENDOFFILE";
		}
		return theReader.ReadLine();
	}

	public string extension(string filePath) {
		return Path.GetExtension (filePath);
	}

	public void printToCVS(string line){
		
		System.IO.File.AppendAllText("/Users/JoelFrewin/Documents/RobotVR/outputcsv.csv", "\n" + line);
	}
}