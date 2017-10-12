using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimReader: MonoBehaviour, IFileReceiver {

	public static SimReader instance;

    // Store each executable in sim file, begin executing after the world is completely loaded
    private List<string> executables;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public GameObject ReceiveFile(string path)
    {
        SimManager.instance.ResetWorld();
        SimManager.instance.PauseSimulation();
        executables = new List<string>();
        IO io = new IO ();
		if (!io.Load (path))
			return null;
		while (true)
        {
			string line = io.readLine ();
			if (line == "ENDOFFILE")
				break;
			//make sure line isnt blank
			if (line.Length > 0) {
				//check for hashtag
				if (line [0] != '#') {
					if(!process(line))
                    {
                        Debug.Log("Error processing Sim file");
                        SimManager.instance.CreateNewBox(2000, 2000);
                        SimManager.instance.ResumeSimulation();
                    }
				}
			}
		}
        // Finished processing - Launch exectuables
        StartCoroutine(LaunchExecutables());
        return null;
	}

	public bool process(string line){
		string[] args = line.Split (new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        switch (args[0].ToLower()) {
            case "robi":
                /*build robot
                RobotBuilder rb = gameObject.AddComponent<RobotBuilder> ();
                GameObject robot = rb.ReceiveFile (ApplicationHelper.localDataPath() + args [1]);
                robot.transform.position = new Vector3 (float.Parse(args[3])/Eyesim.Scale,0,float.Parse(args[4])/Eyesim.Scale);
                /*run client*/
                break;
            case "labbot":
                if (args.Length < 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    return false;
                }
                ObjectManager.instance.AddLabBotToScene(args[1] + ":" + args[2] + ":" + args[3]);
                if (args.Length == 5)
                {
                    string execPath = args[4];
                    if (!(String.IsNullOrEmpty(execPath) || execPath.Trim().Length == 0))
                    {
                        if (execPath[0] == '"')
                        {
                            execPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                            execPath = execPath.Trim('"');
                        }
                        executables.Add(execPath);
                    }

                }
                break;

            case "s4":
                if (args.Length != 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    return false;
                }
                ObjectManager.instance.AddS4ToScene(args[1] + ":" + args[2] + ":" + args[3]);
                if (args.Length == 5)
                {
                    string execPath = args[4];
                    if (!(String.IsNullOrEmpty(execPath) || execPath.Trim().Length == 0))
                    {
                        if (execPath[0] == '"')
                        {
                            execPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                            execPath = execPath.Trim('"');
                        }
                        executables.Add(execPath);
                    }
                }
                break;

            case "can":
                if (args.Length != 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    return false;
                }
                ObjectManager.instance.AddCokeCanToScene(args[1] + ":" + args[2] + ":" + args[3]);
                break;

            case "soccer":
                if (args.Length != 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    return false;
                }
                ObjectManager.instance.AddSoccerBallToScene(args[1] + ":" + args[2] + ":" + args[3]);
                break;

            case "world":
                // Check for quoted expression
                string wldPath;
                // Get the path argument;
                if (args[1][0] == '"')
                {
                    wldPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                    wldPath = wldPath.Trim('"');
                }
                else
                    wldPath = args[1];

                if (Path.GetExtension(wldPath) != ".maz" && Path.GetExtension(wldPath) != ".wld")
                {
                    print(Path.GetExtension(wldPath));
                    Debug.Log("Invalid path to world");
                    return false;
                }

                WorldBuilder.instance.ReceiveFile(Path.GetFullPath(wldPath));
                break;
            case "obj":
                string objPath;
                if (args[1][0] == '"')
                {
                    objPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                    objPath = objPath.Trim('"');
                }
                else
                    objPath = args[1];
                string objName = Path.GetFileNameWithoutExtension(objPath);
                Debug.Log(objName);
                // If object doesn't exist, load it
                if (ObjectManager.instance.customObjects.Find(x => x.name == objName) == null)
                    ObjectManager.instance.ReceiveFile(objPath);
                break;
            case "marker":
                string pos;
                if (args.Length == 3)
                    pos = args[1] + ":" + args[2];
                else if (args.Length == 6)
                    pos = args[1] + ":" + args[2] + ":" + args[3] + ":" + args[4] + ":" + args[5];
                else
                {
                    Debug.Log("Load Sim: Invalid number of arguments for marker: " + args.Length);
                    return false;
                }

                ObjectManager.instance.AddMarkerToScene(pos);       
                break;

            // Check if string is an object name
            default:
                int index = ObjectManager.instance.customObjects.FindIndex(x => x.name == args[0]);
                if(index != -1)
                {
                    if(args.Length == 4)
                    {
                        string objPos = args[1] + ":" + args[2] + ":" + args[3];
                        ObjectManager.instance.AddCustomObjectToScene(index, objPos);
                    }
                    else
                    {
                        Debug.Log("Load Sim: Object argument <" + args[0] + "> requires 3 arguments for position, found " + (args.Length - 1).ToString());
                        break;
                    }
                }
			    break;
		}
        return true;
	}

    // Launch executables
    // Launch process one by one, wait until connection received until continuing
    IEnumerator LaunchExecutables()
    {
        for(int i = 0; i < executables.Count; i++)
        {
            ServerManager.instance.connectionReceived = false;
            SimManager.instance.allRobots[i].ReceiveFile(@executables[i]);
            yield return new WaitUntil(() => ServerManager.instance.connectionReceived);
        }

        SimManager.instance.ResumeSimulation();
    }
}
