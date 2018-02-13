using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using RobotComponents;

// Control invidiual motors at low level
public interface IMotors
{
    void DriveMotor(int motor, int speed);
    int GetEncoder(int quad);
}

// Set PID Controller values for a motor, and drive using 
public interface IPIDUsable
{
    void DriveMotorControlled(int motor, int ticks);
    void SetPID(int motor, int p, int i, int d);
}
    
// VW Drive interface for RoBIOS commands
public interface IVWDrive
{
    // Initalize VW Parameters (mostly unused)
    void InitalizeVW(int[] args);
    // Get robots internal position
    Int16[] GetPose();
    // Set robots internal position
    void SetPose(int x, int y, int phi);
    // Set vehicle speed manually
    void VWSetVehicleSpeed(int linear, int angular);
    // Get current speed
    Speed VWGetVehicleSpeed();
    // Drive a straight line
    void VWDriveStraight(int distance, int speed);
    // Turn on the spot
    void VWDriveTurn(int rotation, int velocity);
    // Drive an arc of a circle
    void VWDriveCurve(int distance, int rotation , int velocity);
    // Return remaining distance to drive
    int VWDriveRemaining();
    // Return whether or not a controlled drive is being executed
    bool VWDriveDone();
    // Return whether or not a motor has stalled
    int VWDriveStalled();
    // Send a reply when the current drive has finished
    void VWDriveWait(Action<RobotConnection> doneCallback);
    // Clear any current VWWait command (used when control is terminated whilst a VWWait is pending)
    void ClearVWWait();
    // Use accurate positioning
    bool VWAccurate { get; set; }
}

// Controling mechanical servos
public interface IServos
{
    void SetServo(int servo, int angle);
}

// Using position sensitive devices
public interface IPSDSensors
{
    UInt16 GetPSD(int psd);
    float MeanError { get; set; }
    float StdDevError { get; set; }
    bool UseError { get; set; }
    bool UseGlobalError { get; set; }
    void SetVisualize(bool val);
}

// Using cameras
public interface ICameras
{
    byte[] GetCameraOutput(int camera);
    void SetCameraResolution(int camera, int width, int height);
    string GetCameraResolution(int camera);
    EyeCamera GetCameraComponent(int camera);
    // Salt and Pepper noise parameters
    bool SaltPepperNoise { get; set; }
    // Salt and Pepper noise % of pixels to modify (average)
    float SPPixelPercent { get; set; }
    // Salt and Pepper ratio of black to white pixels (average)
    float SPBWRatio { get; set; }
    // Gaussian noise parameters
    bool GaussianNoise { get; set; }
    float GaussMean { get; set; }
    float GaussStdDev { get; set; }

}

// Playing audio (note AUClip is handled entirely in the executing control program)
public interface IAudio
{
    void PlayBeep();
}

// Sending/Receiving radio messages
public interface IRadio
{
    void AddMessageToBuffer(byte[] msg);
    byte[] RetrieveMessageFromBuffer();
    void WaitForRadioMessage(Action<RobotConnection, byte[]> radioDelegate);
    int GetNumberOfMessages();
}

// using LIDAR Scanner
public interface ILaser
{
    int[] LaserScan();
    void SetVisualize(bool val);
}
// Class used to pass data back to client
public class Speed
{
    public Int16 linear;
    public Int16 angular;

    public Speed(float lin, float ang)
    {
        linear = Convert.ToInt16(lin);
        angular = Convert.ToInt16(ang);
    }
}
// Interface for building robot from robi file
public enum AxelType { None, Drive, Turn, Omni };

public interface ConfigureableRobot
{
    void AddBox(Vector3 size, Vector3 centre, PhysicMaterial friction);
    void AddSphere(float radius, Vector3 centre, PhysicMaterial friction);
    void AddCapsule(float radius, float height, Vector3 centre, PhysicMaterial friction);
    void ConfigureMass(float mass, Vector3 com);
    void ConfigureAxel(float axelHeight, float axelPos, AxelType type);
    void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track, AxelType type);
    bool AddPSDSensor(int id, string name, Vector3 pos, float rot);
    void ConfigureCamera(Vector3 pos, float pan, float tilt, float maxPan, float maxTilt);
    void ConfigureModel(GameObject model, Vector3 pos, Vector3 rot);
    void ConfigureLidar(int numPoints, int tilt);
}

// Abstract robot
// Universal functions
public abstract class Robot : PlaceableObject, IPointerClickHandler, IFileReceiver
{
    [Space(10)]
    [Header("Robot Settings")]
    public int axels = 0;

    public RobotConnection myConnection = null;
    private Process controlBinary = null;
    [HideInInspector]
    public string controlBinaryName = "";
    [Header("Object References")]
    public TrailRenderer trail;

    // Robot Info Window
    public RobotInspectorWindow myWindow;

    // Set the robots absolute position along the ground plane
    // Dangerous: Could result in extreme behaviour with physics engine
    public void SetRobotPosition(int x, int z, int phi)
    {
        transform.SetPositionAndRotation(new Vector3(x / Eyesim.Scale, 0.02f, z / Eyesim.Scale), Quaternion.Euler(new Vector3(0f, phi, 0f)));
    }

    public void ToggleTrail()
    {
        if(trail != null)
        {
            trail.enabled = !trail.enabled;
        }
    }

    // Open the info window for this robot
    override public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            // If no window exists, make a new one
            if (myWindow == null)
            {
                myWindow = Instantiate(UIManager.instance.robotInspectorWindowPrefab, UIManager.instance.gameWindowContainer);
                myWindow.robot = this;
                myWindow.gameObject.SetActive(true);
            }
            // Else activate the old one
            else
                myWindow.gameObject.SetActive(true);
        }
    }

    // Exectue a control program - Receives path to control
    public GameObject ReceiveFile(string filepath)
    {
        if(controlBinary != null || myConnection != null)
        {
            UnityEngine.Debug.Log("Already executing control");
            return null;
        }
        controlBinary = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            startInfo.WorkingDirectory = @"cygwin\bin";
            startInfo.EnvironmentVariables["DISPLAY"] = ":0";
        }
        else
        {
            startInfo.WorkingDirectory = Path.GetDirectoryName(filepath);
        }

        startInfo.FileName = filepath;
        controlBinary.StartInfo = startInfo;
        ServerManager.instance.activeRobot = this;
        try
        {
            controlBinary.Start();
            controlBinaryName = Path.GetFileName(filepath);
            if (myWindow != null)
                myWindow.controlName.text = controlBinaryName;
        }
        catch (Win32Exception w)
        {
            UnityEngine.Debug.Log(w);
        }
        return null;
    }

    // Callback to ServerManager Disconnect with this robot's connection
    public void DisconnectRobot()
    {
        if (myConnection != null)
        {
            ServerManager.instance.CloseConnection(myConnection);
        }
    }

    // Remove control binary - invoked from ServerManager on disconnect
    public void TerminateControlBinary()
    {
        if(this is IVWDrive)
        {
            (this as IVWDrive).ClearVWWait();
        }
        if(controlBinary != null)
        {
            try
            {
                controlBinary.CloseMainWindow();
                controlBinary.Close();
            }
            catch
            {
                UnityEngine.Debug.Log("Already Closed");
            }
            controlBinary = null;
            controlBinaryName = "";
            if (myWindow != null)
                myWindow.controlName.text = controlBinaryName;
        }
        else
        {
            UnityEngine.Debug.Log("Terminate Control Binary: Process not managed by eyesim");
        }
    }

    // Handle OnClick event
    override public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount > 1 && !isWindowOpen)
            OpenInfoWindow();
        base.OnPointerClick(eventData);
    }
    
    override public void PlaceObject()
    {
        if (isInit == false)
        {
            if (this is IVWDrive)
                (this as IVWDrive).SetPose(0, 0, (int) transform.eulerAngles.y);
        }
        base.PlaceObject();
    }

    private void OnDestroy()
    {
        if (myWindow != null)
            Destroy(myWindow.gameObject);
    }
}
