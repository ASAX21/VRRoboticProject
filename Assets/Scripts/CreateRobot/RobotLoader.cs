using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLoader: MonoBehaviour, IFileReceiver {

    public static RobotLoader instance = null;
    public Robot robot;
    public GameObject robotObject;
    private ConfigureableRobot robotConfig;
    public string filepath;

    private bool robotDrive;
    public GameObject diffDriveBase;
    public GameObject ackDriveBase;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    public void Test(string filepath)
    {
        GameObject test =ReceiveFile(filepath);
        test.GetComponent<Rigidbody>().isKinematic = true;
        test.transform.position = Vector3.zero;
    }

    public GameObject ReceiveFile(string filepath)
    {
        this.filepath = filepath;
        IO io = new IO();
        if (!io.Load(filepath))
            return null;
        string line;

        while ((line = io.readLine()) != "ENDOFFILE")
        {
            if (line.Length > 0 && line[0] != '#')
            {
                if (!process(line))
                {
                    Debug.Log("Error processing.");
                    Debug.Log(line);
                    robotConfig = null;
                    Destroy(robotObject);
                    robotObject = null;
                    return null;
                }
            }
        }
        robotObject.GetComponent<PlaceableObject>().PostBuild();
        ObjectManager.instance.StoreCustomRobot(robotObject);
        return robotObject;
    }

    private bool CheckArguments(string[] args, int numArgs, string id)
    {
        if (!robotDrive)
        {
            Debug.Log("Robot drive type must be specified first");
            return false;
        }
        if (args.Length < numArgs || (String.IsNullOrEmpty(args[1]) || args[1].Trim().Length == 0))
        {
            Debug.Log("Missing argument after" + id);
            return false;
        }

        for(int i = 1; i < numArgs; i++)
        {
            if ((String.IsNullOrEmpty(args[1]) || args[1].Trim().Length == 0))
            {
                Debug.Log("Missing argument after" + id);
                return false;
            }
        }
       
        return true;
    }

    // Run each line - return true if worked, false on failure
    public bool process(string line)
    {

        string[] args = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        // Line processing in try block: Catch only FormatException from bad float parsing
        try
        {
            switch (args[0])
            {
                // Type of robot
                case "drive":
                    {
                        robotDrive = true;
                        if (!CheckArguments(args, 2, "drive"))
                            return false;
                        switch (args[1])
                        {
                            case "DIFFERENTIAL_DRIVE":
                                robotObject = Instantiate(diffDriveBase);
                                break;
                            case "ACKERMANN_DRIVE":
                                robotObject = Instantiate(ackDriveBase);
                                break;
                            case "OMNI_DRIVE":
                                Debug.Log("Omni drive not implemented");
                                return false;
                            default:
                                Debug.Log("Unknown Drive received: " + args[1]);
                                return false;
                        }
                        robotConfig = robotObject.GetComponent<ConfigureableRobot>();
                        robotObject.name = "Custom Robot " + ObjectManager.instance.customRobots.Count + 1;
                        robotObject.SetActive(false);
                        return true;
                    }
                case "name":
                    {
                        if (!CheckArguments(args, 2, "name"))
                            return false;
                        // Check if name already exists
                        if (args[1].ToLower() == "labbot" || args[1].ToLower() == "s4")
                        {
                            EyesimLogger.instance.Log("Error loading robot - Robot with name " + args[1] + " already exists");
                            return false;
                        }

                        foreach (GameObject obj in ObjectManager.instance.customRobots)
                        {
                            if (obj.name.ToLower() == args[1].ToLower())
                            {
                                EyesimLogger.instance.Log("Error loading robot - Robot with name " + args[1] + " already exists");
                                return false;
                            }
                        }
                        robotObject.name = args[1];
                        return true;
                    }
                // Path to robot model (.obj file)
                case "model":
                    {
                        if (! (CheckArguments(args, 2, "model") || CheckArguments(args, 8, "model")))
                            return false;
                        // Load object model
                        string modelPath = null;
                        int argOffset = 0;
                        if (args[1][0] == '"')
                        {
                            modelPath = Regex.Matches(line, "\"[^\"]*\"")[0].ToString();
                            modelPath = modelPath.Trim('"');
                            argOffset = 1;
                        }
                        else
                            modelPath = args[1];
                        GameObject modelObj = OBJLoader.LoadOBJFile(modelPath);
                        if (modelObj == null)
                            return false;

                        if (args.Length == 2)
                            robotConfig.ConfigureModel(modelObj, Vector3.zero, Vector3.zero);
                        else
                        {
                            Vector3 modelPos = new Vector3(float.Parse(args[2 + argOffset]) / Eyesim.Scale, float.Parse(args[3 + argOffset]) / Eyesim.Scale, float.Parse(args[4 + argOffset]) / Eyesim.Scale);
                            Vector3 modelRot = new Vector3(float.Parse(args[5 + argOffset]), float.Parse(args[6 + argOffset]), float.Parse(args[7 + argOffset]));
                            robotConfig.ConfigureModel(modelObj, modelPos, modelRot);
                        }
                        return true;
                    }
                // Centre of mass, and mass in kg
                case "mass":
                    {
                        if (!CheckArguments(args, 5, "mass"))
                            return false;
                        Vector3 com = new Vector3(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale, float.Parse(args[4]) / Eyesim.Scale);
                        robotConfig.ConfigureMass(float.Parse(args[1]), com);
                        return true;
                    }
                // Location of robot's axis
                case "axis":
                    {
                        if (!CheckArguments(args, 3, "axis"))
                            return false;
                        else if(!(robotObject.GetComponent<Robot>() is BaseDiffDrive))
                        {
                            Debug.Log("axis on non diff-drive robot");
                            return false;
                        }
                        robotConfig.ConfigureAxel(float.Parse(args[1]), float.Parse(args[2]), AxelType.None);
                        return true;
                    }
                // Location of an axel, and type (drive / turn)
                case "axel":
                    {
                        if (!CheckArguments(args, 4, "axel"))
                            return false;
                        else if (!(robotObject.GetComponent<Robot>() is BaseAckDrive))
                        {
                            Debug.Log("axel on non ackermann-drive robot");
                            return false;
                        }
                        if (args[1] == "turn")
                            robotConfig.ConfigureAxel(float.Parse(args[2]), float.Parse(args[3]), AxelType.Turn);
                        else if (args[1] == "drive")
                            robotConfig.ConfigureAxel(float.Parse(args[2]), float.Parse(args[3]), AxelType.Drive);
                        else
                        {
                            Debug.Log("Bad axel designation (" + args[1] + "). Must be turn or drive.");
                            return false;
                        }
                        return true;
                    }
                // Add a PSD sensor
                case "psd":
                    {
                        if (!CheckArguments(args, 7, "psd"))
                            return false;
                        Vector3 pos = new Vector3(float.Parse(args[3]) / Eyesim.Scale, float.Parse(args[4]) / Eyesim.Scale, float.Parse(args[5]) / Eyesim.Scale);
                        robotConfig.AddPSDSensor(int.Parse(args[2]), args[1], pos, float.Parse(args[6]));
                    }
                    return true;
                // Configure wheels
                case "wheel":
                    {
                        if (!CheckArguments(args, 5, "wheel"))
                            return false;
                        robotConfig.ConfigureWheels(float.Parse(args[1]), float.Parse(args[2]), int.Parse(args[3]), float.Parse(args[4]));
                    }
                    return true;
                // Add camera
                case "camera":
                    {
                        if (!CheckArguments(args, 8, "camera"))
                            return false;
                        Vector3 pos = new Vector3(float.Parse(args[1]) / Eyesim.Scale, float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale);
                        robotConfig.ConfigureCamera(pos, float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]));
                    }
                    return true;
                default:
                    Debug.Log("Unknown argument: " + args[0]);
                    return true;
            }
        }
        // Catch parsing errors
        catch (FormatException e)
        {
            Debug.Log("Error parsing arguments for " + args[0] + ": " + e.Message);
            foreach (string s in args)
                Debug.Log(s);
            return false;
        }
    }
}
