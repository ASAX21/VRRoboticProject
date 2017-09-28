using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using RobotComponents;

public interface IMotors
{
    void DriveMotor(int motor, int speed);
}

public interface IPIDUsable
{
    void DriveMotorControlled(int motor, int ticks);
    void SetPID(int motor, int p, int i, int d);
}
    
public interface IVWDrivable
{
    void InitalizeVW(int[] args);
    Int16[] GetPose();
    void SetPose(int x, int y, int phi);
    void VWSetVehicleSpeed(int linear, int angular);
    Speed VWGetVehicleSpeed();
    void VWDriveStraight(int distance, int speed);
    void VWDriveTurn(int rotation, int velocity);
    void VWDriveCurve(int distance, int rotation , int velocity);
    int VWDriveRemaining();
    bool VWDriveDone();
    int VWDriveStalled();
    void VWDriveWait(Action<RobotConnection> doneCallback);
    void ClearVWWait();
}

public interface IServoSettable
{
    void SetServo(int servo, int angle);
}

public interface IPSDSensors
{
    UInt16 GetPSD(int psd);
    float MeanError { get; set; }
    float StdDevError { get; set; }
    bool UseGlobalError { get; set; }
}

public interface ICameras
{
    byte[] GetCameraOutput(int camera);
    void SetCameraResolution(int camera, int width, int height);
    string GetCameraResolution(int camera);
    EyeCamera GetCameraComponent(int camera);
    bool SaltPepperNoise { get; set; }
    // Salt and Pepper noise % of pixels to modify (average)
    float SPPixelPercent { get; set; }
    // Salt and Pepper ratio of black to white pixels (average)
    float SPBWRatio { get; set; }

    bool GaussianNoise { get; set; }
    float GaussMean { get; set; }
    float GaussStdDev { get; set; }

}

public interface IAudio
{
    void PlayBeep();
}

public interface IRadio
{
    void AddMessageToBuffer(byte[] msg);
    byte[] RetrieveMessageFromBuffer();
    void WaitForRadioMessage(Action<RobotConnection, byte[]> radioDelegate);
    int GetNumberOfMessages();
}

public interface ILaser
{
    int[] LaserScan();
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
public enum AxelType { None, Drive, Turn };

public interface ConfigureableRobot
{
    void ConfigureSize(float length, float width, float height);
    void ConfigureMass(float mass, Vector3 com);
    void ConfigureAxel(float axelHeight, float axelPos, AxelType type);
    void ConfigureWheels(float diameter, float maxVel, int ticksPerRev, float track);
    bool AddPSDSensor(int id, string name, Vector3 pos, float rot);
    void ConfigureCamera(Vector3 pos, float pan, float tilt, float maxPan, float maxTilt);
}

// Abstract robot
// Universal functions
public abstract class Robot : PlaceableObject, IPointerClickHandler, IFileReceiver
{
    public int axels = 0;

    public RobotConnection myConnection = null;
    private Process controlBinary = null;
    public string controlBinaryPath = "";
    public TrailRenderer trail;

    // Robot Info Window
    public RobotInspectorWindow myWindow;

    // Set the robots absolute position along the ground plane
    // Dangerous: Could result in extreme behaviour with physics engine
    public void SetRobotPosition(int x, int z, int phi)
    {
        transform.SetPositionAndRotation(new Vector3(x, 0.1f, z), Quaternion.Euler(new Vector3(0f, phi, 0f)));
    }

    public void ToggleTrail()
    {
        if(trail != null)
        {
            trail.enabled = !trail.enabled;
        }
    }

    // Open the info window for this robot
    public void OpenInfoWindow()
    {
        if (!isWindowOpen)
        {
            isWindowOpen = true;
            // If no window exists, make a new one
            if (myWindow == null)
            {
                myWindow = Instantiate(UIManager.instance.robotInspectorWindowPrefab,UIManager.instance.gameWindowContainer);
                myWindow.robot = this;
                myWindow.Initialize();

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
        startInfo.EnvironmentVariables["DISPLAY"] = ":0";
        if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            startInfo.WorkingDirectory = @"cygwin\bin";
        startInfo.UseShellExecute = false;
        startInfo.FileName = filepath;
        controlBinary.StartInfo = startInfo;
        ServerManager.instance.activeRobot = this;
        try
        {
            controlBinary.Start();
            controlBinaryPath = Path.GetFileName(filepath);
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
        if(this is IVWDrivable)
        {
            UnityEngine.Debug.Log("VWDriveable! clearing wait");
            (this as IVWDrivable).ClearVWWait();
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
            controlBinaryPath = "";
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
}
