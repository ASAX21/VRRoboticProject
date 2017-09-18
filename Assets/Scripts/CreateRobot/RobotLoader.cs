using System;
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

    public bool process(string line)
    {

        string[] args = line.Split(' ');
        switch (args[0])
        {
            // Type of robot
            case "drive":
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
                return true;
            case "name":
                if (!CheckArguments(args, 2, "name"))
                    return false;
                robotObject.name = args[1];
                return true;
            case "model":
                if (!CheckArguments(args, 2, "model"))
                    return false;
                return true;
            case "mass":
                if (!CheckArguments(args, 5, "mass"))
                    return false;
                try
                {
                    Vector3 com = new Vector3(float.Parse(args[2]) / 1000f, float.Parse(args[3]) / 1000f, float.Parse(args[4]) / 1000f);
                    robotConfig.ConfigureMass(float.Parse(args[1]), com);
                }
                catch (FormatException e)
                {
                    Debug.Log("Error parsing mass arguments: " + e.Message);
                    return false;
                }
                return true;
            case "axis":
                if (!CheckArguments(args, 3, "axis"))
                    return false;
                try
                {
                    robotConfig.ConfigureAxel(float.Parse(args[1]), float.Parse(args[2]));
                }
                catch (FormatException e)
                {
                    Debug.Log("Error parsing axis arguments: " + e.Message);
                    return false;
                }
                return true;
            case "psd":
                if (!CheckArguments(args, 7, "psd"))
                    return false;
                try
                {
                    Vector3 pos = new Vector3(float.Parse(args[3]) / 1000f, float.Parse(args[4]) / 1000f, float.Parse(args[5]) / 1000f);
                    robotConfig.AddPSDSensor(int.Parse(args[2]), args[1], pos, float.Parse(args[6]));
                }
                catch (FormatException e)
                {
                    Debug.Log("Error parsing psd arguments: " + e.Message);
                    return false;
                }
                return true;
            case "wheel":
                if (!CheckArguments(args, 5, "wheel"))
                    return false;
                try
                {
                    robotConfig.ConfigureWheels(float.Parse(args[1]), float.Parse(args[2]), int.Parse(args[3]), float.Parse(args[4]));
                }
                catch (FormatException e)
                {
                    Debug.Log("Error parsing wheel arguments: " + e.Message);
                    return false;
                }
                return true;
            case "camera":
                if (!CheckArguments(args, 8, "camera"))
                    return false;
                try
                {
                    Vector3 pos = new Vector3(float.Parse(args[1])/1000f, float.Parse(args[2]) / 1000f, float.Parse(args[3]) / 1000f);
                    robotConfig.ConfigureCamera(pos, float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]));
                }
                catch (FormatException e)
                {
                    Debug.Log("Error parsing camera arguments: " + e.Message);
                    return false;
                }
                return true;
            default:
                Debug.Log("Unknown argument: " + args[0]);
                return true;
        }
        return false;
    }
}
