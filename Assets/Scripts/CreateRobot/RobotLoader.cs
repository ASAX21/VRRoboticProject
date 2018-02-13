using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotLoader: MonoBehaviour, IFileReceiver
{

    public static RobotLoader instance = null;

    // Type of robots
    public enum RobotDriveType { None, Differential, Ackermann, Omni};

    // Current robot being built
    public Robot robot;
    public GameObject robotObject;
    private ConfigureableRobot robotConfig;
    private RobotDriveType driveType = RobotDriveType.None;

    // Filereading 
    public string filepath;
    private IO io;

    // Prefabs
    private bool robotDrive;
    public GameObject diffDriveBase;
    public GameObject ackDriveBase;
    public GameObject baseOmniDrive;
    public PhysicMaterial noFriction;
    public PhysicMaterial highFriction;

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
        io = new IO("#;");

        if (!io.Load(filepath))
            return null;
        string[] args;
        driveType = RobotDriveType.None;
        while ((args = io.ReadNextArguments())[0] != "ENDOFFILE")
        {
            if (!process(args))
            {
                Debug.Log("Error processing.");
                robotConfig = null;
                Destroy(robotObject);
                robotObject = null;
                io = null;
                return null;
            }
        }
        robotObject.GetComponent<PlaceableObject>().PostBuild();
        ObjectManager.instance.StoreCustomRobot(robotObject);
        io = null;
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
    public bool process(string[] args)
    {
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
                                driveType = RobotDriveType.Differential;
                                robotObject = Instantiate(diffDriveBase);
                                Debug.Log("Diff Drive");
                                break;
                            case "ACKERMANN_DRIVE":
                                driveType = RobotDriveType.Ackermann;
                                robotObject = Instantiate(ackDriveBase);
                                Debug.Log("Ackermann Drive");
                                break;
                            case "OMNI_DRIVE":
                                driveType = RobotDriveType.Omni;
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
                        string modelPath = io.SearchForFile(args[1], "");
                        if (modelPath == "")
                        {
                            EyesimLogger.instance.Log("Failed to find model for robot in " + filepath);
                            return false;
                        }
                        
                        GameObject modelObj = OBJLoader.LoadOBJFile(modelPath);
                        if (modelObj == null)
                            return false;

                        if (args.Length == 2)
                            robotConfig.ConfigureModel(modelObj, Vector3.zero, Vector3.zero);
                        else
                        {
                            Vector3 modelPos = new Vector3(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale, float.Parse(args[4]) / Eyesim.Scale);
                            Vector3 modelRot = new Vector3(float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]));
                            robotConfig.ConfigureModel(modelObj, modelPos, modelRot);
                        }
                        return true;
                    }
                // Add a collider to the robot : box, sphere, or capsule
                case "collider":
                    {
                        if(args[1] == "box")
                        {
                            if(!CheckArguments(args, 8, "collider box"))
                                return false;
                            robotConfig.AddBox(new Vector3(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale , float.Parse(args[4]) / Eyesim.Scale), 
                                new Vector3(float.Parse(args[5]) / Eyesim.Scale, float.Parse(args[6]) / Eyesim.Scale, float.Parse(args[7]) / Eyesim.Scale), noFriction);
                        }
                        else if(args[1] == "sphere")
                        {
                            if(!CheckArguments(args, 6, "collider sphere"))
                                return false;
                            robotConfig.AddSphere(float.Parse(args[2]) / Eyesim.Scale, 
                                new Vector3(float.Parse(args[3]) / Eyesim.Scale, float.Parse(args[4]) / Eyesim.Scale, float.Parse(args[5]) / Eyesim.Scale), noFriction);
                        }
                        else if(args[1] == "capsule")
                        {
                            if(!CheckArguments(args, 7, "collider capsule"))
                                return false;
                            robotConfig.AddCapsule(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale,
                                new Vector3(float.Parse(args[4]) / Eyesim.Scale, float.Parse(args[5]) / Eyesim.Scale, float.Parse(args[6]) / Eyesim.Scale), noFriction);
                        }
                        else
                        {
                            EyesimLogger.instance.Log(filepath + "." + io.LineNum + "Bad arguments for collider in ");
                            return false;
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
                        else if(driveType != RobotDriveType.Differential)
                        {
                            Debug.Log("axis on non diff-drive robot");
                            return false;
                        }
                        robotConfig.ConfigureAxel(float.Parse(args[1]) / Eyesim.Scale, float.Parse(args[2]) / Eyesim.Scale, AxelType.None);
                        return true;
                    }
                // Location of an axel, and type (drive / turn)
                case "axel":
                    {
                        if (!CheckArguments(args, 4, "axel"))
                            return false;
                        else if (driveType != RobotDriveType.Ackermann)
                        {
                            Debug.Log("axel on non ackermann-drive robot");
                            return false;
                        }
                        if (args[1] == "turn")
                            robotConfig.ConfigureAxel(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale, AxelType.Turn);
                        else if (args[1] == "drive")
                            robotConfig.ConfigureAxel(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]) / Eyesim.Scale, AxelType.Drive);
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
                        if(driveType == RobotDriveType.Differential)
                        {
                            if(!CheckArguments(args, 5, "wheel"))
                                return false;
                            robotConfig.ConfigureWheels(float.Parse(args[1]) / Eyesim.Scale, float.Parse(args[2]), int.Parse(args[3]), float.Parse(args[4]) / Eyesim.Scale, AxelType.None);
                        }
                        else if (driveType == RobotDriveType.Ackermann)
                        {
                            if(!CheckArguments(args, 6, "wheel"))
                                return false;
                            if(args[1].ToLower() == "drive")
                                robotConfig.ConfigureWheels(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]), int.Parse(args[4]), float.Parse(args[5]) / Eyesim.Scale, AxelType.Drive);
                            else if(args[1].ToLower() == "turn")
                                robotConfig.ConfigureWheels(float.Parse(args[2]) / Eyesim.Scale, float.Parse(args[3]), int.Parse(args[4]), float.Parse(args[5]) / Eyesim.Scale, AxelType.Turn);
                            else
                                EyesimLogger.instance.Log(filepath + "." + io.LineNum + ":" + "Bad arguments for wheel - must specify axel type (turn or drive)");
                        }

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
                case "lidar":
                    {
                        if(!CheckArguments(args, 3, "lidar"))
                            return false;
                        robotConfig.ConfigureLidar(int.Parse(args[1]), int.Parse(args[2]));
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
