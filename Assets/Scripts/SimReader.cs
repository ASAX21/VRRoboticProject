using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimReader: MonoBehaviour, IFileReceiver
{
	public static SimReader instance;


    public bool readingSimFile = false;
    private string simPath;
    private IO io = null;

    private Eyesim.WorldType worldType = Eyesim.WorldType.None;

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
        EyesimLogger.instance.Log("Loading sim file: " + path);
        simPath = path;
        executables = new List<string>();

        io = new IO (";#");
		if (!io.Load (path))
			return null;

        readingSimFile = true;
        string[] args;
        while ((args = io.ReadNextArguments())[0] != "ENDOFFILE")
        {
			if(!process(args))
            {
                Debug.Log("Error processing Sim file");
                SimManager.instance.CreateNewBox(2000, 2000);
                SimManager.instance.ResumeSimulation();
                io = null;
                readingSimFile = false;
                return null;
            }
		}
        // Finished processing - Launch exectuables
        StartCoroutine(LaunchExecutables());
        io = null;
        readingSimFile = false;
        return null;
	}

    private bool CheckRobotArguments(string[] args)
    {
        // Check 's' for maze start
        if(args.Length < 2 || args.Length > 5)
        {
            EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for robot " + args[0]);
            return false;
        }
        else if(args.Length == 2 || args.Length == 3)
        {
            if(worldType != Eyesim.WorldType.Maze)
            {
                EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Maze start position valid only for .maz world files");
            }
            else if(args[1].ToLower() != "s")
            {
                EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Bad robot line");
            }
        }
        // Check Position arguments
        else if(args.Length == 4 || args.Length == 5)
        {
            try
            {
                float.Parse(args[1]);
                float.Parse(args[2]);
                float.Parse(args[3]);
            }
            catch (FormatException)
            {
                EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Unexpected characters in position arguments");
                return false;
            }
        }

        // Position and executable
        if(args.Length == 3 || args.Length == 5)
        {
            string execPath = io.SearchForFile(args[args.Length - 1], "");
            if(execPath == "")
            {
                EyesimLogger.instance.Log("Failed to find robot executable for " + args[args.Length - 1] + " on line " + io.LineNum);
                return false;
            }
            else
            {
                executables.Add(execPath);
            }
        }

        return true;
    }

	public bool process(string[] args)
    {
        switch (args[0].ToLower())
        {
            case "robi":
                string robiPath = io.SearchForFile(args[1], "");
                RobotLoader.instance.ReceiveFile(robiPath);
                break;
            case "labbot":
                if(CheckRobotArguments(args))
                {
                    // Start specified by maze
                    if(args.Length == 2 || args.Length == 3)
                    {
                        if(WorldBuilder.instance.isStartSpecified)
                            ObjectManager.instance.AddLabBotToScene((WorldBuilder.instance.robotStartX * Eyesim.Scale).ToString() + ":" + ((WorldBuilder.instance.robotStartY + WorldBuilder.instance.floorMazeOffset) * Eyesim.Scale).ToString() + ":0");
                        else
                            EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": No starting position indicated in .maz file");
                    }
                    else if(args.Length >= 4)
                    {
                        ObjectManager.instance.AddLabBotToScene(args[1] + ":" + args[2] + ":" + args[3]);
                    }
                }
                break;

            case "s4":
                if(CheckRobotArguments(args))
                {
                    // Start specified by maze
                    if(args.Length == 2 || args.Length == 3)
                    {
                        if(WorldBuilder.instance.isStartSpecified)
                            ObjectManager.instance.AddS4ToScene((WorldBuilder.instance.robotStartX * Eyesim.Scale).ToString() + ":" + ((WorldBuilder.instance.robotStartY + WorldBuilder.instance.floorMazeOffset) * Eyesim.Scale).ToString() + ":0");
                        else
                            EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": No starting position indicated in .maz file");
                    }
                    else if(args.Length >= 4)
                    {
                        ObjectManager.instance.AddS4ToScene(args[1] + ":" + args[2] + ":" + args[3]);
                    }
                }
                break;

            case "can":
                if (args.Length != 4)
                {
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for Can - " + args.Length);
                    return false;
                }
                ObjectManager.instance.AddCokeCanToScene(args[1] + ":" + args[2] + ":" + args[3]);
                break;

            case "soccer":
                if (args.Length != 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for Soccer - " + args.Length);
                    return false;
                }
                ObjectManager.instance.AddSoccerBallToScene(args[1] + ":" + args[2] + ":" + args[3]);
                break;

            case "crate":
                if (args.Length != 4)
                {
                    Debug.Log("Incorrect number of arguments");
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for Crate - " + args.Length);
                    return false;
                }
                ObjectManager.instance.AddBoxToScene(args[1] + ":" + args[2] + ":" + args[3]);
                break;

			case "golf":
				if (args.Length != 4)
				{
					EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for Golf - " + args.Length);
					return false;
				}
				ObjectManager.instance.AddGolfBallToScene(args[1] + ":" + args[2] + ":" + args[3]);
				break;

            case "world":
                string wldPath = io.SearchForFile(args[1], SettingsManager.instance.GetSetting("worlddir", ""));
                if(wldPath == "")
                {
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Unable to locate world file");
                    return false;
                }

                if(Path.GetExtension(args[1]) == ".maz")
                    worldType = Eyesim.WorldType.Maze;
                else if(Path.GetExtension(args[1]) == ".wld")
                    worldType = Eyesim.WorldType.World;
                else
                {
                    print(Path.GetExtension(args[1]));
                    Debug.Log("Invalid path to world");
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Bad path to world file");
                    return false;
                }

                WorldBuilder.instance.ReceiveFile(wldPath);
                break;

            case "obj":
                string objName = Path.GetFileNameWithoutExtension(args[1]);
                if (ObjectManager.instance.customObjects.Find(x => x.name == objName) == null)
                {
                    string objPath = io.SearchForFile(args[1], "");
                    if (ObjectManager.instance.ReceiveFile(objPath) == null)
                        return false;
                }
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
                    EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for Marker - " + args.Length);
                    return false;
                }
                ObjectManager.instance.AddMarkerToScene(pos);       
                break;

            // Check if string is a custom object or robot
            default:
                // Check for object
                int index = ObjectManager.instance.customObjects.FindIndex(x => x.name == args[0]);
                if(index != -1)
                {
                    if(args.Length == 4)
                    {
                        string objPos = args[1] + ":" + args[2] + ":" + args[3];
                        ObjectManager.instance.AddCustomObjectToScene(index, objPos);
                        break;
                    }
                    else
                    {
                        EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for " + args[0] + " - " + args.Length);
                        break;
                    }
                }
                // check for robot
                index = ObjectManager.instance.customRobots.FindIndex(x => x.name == args[0]);
                if(index != -1)
                {
                    if (args.Length == 5)
                    {
                        string robPos = args[1] + ":" + args[2] + ":" + args[3];
                        ObjectManager.instance.AddCustomRobotToScene(index, robPos);
                        string execPath = io.SearchForFile(args[4], "");
                        if (execPath != "")
                            executables.Add(execPath);
                        else
                            EyesimLogger.instance.Log("Failed to find robot executable for " + args[5] + " on line " + io.LineNum);
                        break;
                    }
                    else
                    {
                        EyesimLogger.instance.Log("Error parsing sim file line " + io.LineNum + ": Incorrect number of arguments for " + args[0] + " - " + args.Length);
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
        EyesimLogger.instance.Log("Launching control programs");
        for (int i = 0; i < executables.Count; i++)
        {
            ServerManager.instance.connectionReceived = false;
            SimManager.instance.allRobots[i].ReceiveFile(@executables[i]);
            yield return new WaitUntil(() => ServerManager.instance.connectionReceived);
        }

        SimManager.instance.ResumeSimulation();
        EyesimLogger.instance.Log("Finsihed loading sim file");
    }
}
