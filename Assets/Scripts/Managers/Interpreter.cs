using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Intepreter takes RoBIOS commands and parses them, calling the appropriate function
/// on the robot. Input commands are of the form xABCD, where x is an ASCII character,
///  A,B,C,D are 1 or 2 byte integers, depending on the command.
/// </summary>

public class Interpreter {

    public ServerManager serverManager;


    public void ReturnDriveDone(RobotConnection conn)
    {
        Packet p = new Packet();
        p.packetType = PacketType.DRIVE_DONE;
        p.dataSize = 0;
        Debug.Log("writing back drive done!");
        serverManager.WritePacket(conn, p);
    }

    // Motor Drive Uncontrolled
    private void Command_m(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IMotors)
        {
            int motor = recv[1] - 1;
            int speed = recv[2];
            (conn.robot as IMotors).DriveMotor(motor, speed);
        }
        else
        {
            Debug.Log("Drive Command received for a non drivable robot");
        }
    }
    // Motor Drive Controlled
    private void Command_M(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IPIDUsable)
        {
            int motor = recv[1] - 1;
            int ticks = recv[2];
            (conn.robot as IPIDUsable).DriveMotorControlled(motor, ticks);
        }
        else
        {
            Debug.Log("Drive Command received for a non drivable robot");
        }
    }
    // Set Motor PID
    private void Command_d(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IPIDUsable)
        {
            int motor = recv[1] - 1;
            int p = recv[2];
            int i = recv[3];
            int d = recv[4];
            (conn.robot as IPIDUsable).SetPID(motor, p, i, d);
        }
        else
        {
            Debug.Log("Set PID Command received for a non drivable robot");
        }
    }
    // Set Servo position
    private void Command_s(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IServoSettable)
        {
            int servo = recv[1] - 1;
            int angle = recv[2];
            (conn.robot as IServoSettable).SetServo(servo, angle);
        }
        else
        {
            Debug.Log("Set Servo Command received for a non servo settable robot");
        }

    }
    // Get PSD Value
    private void Command_p(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IPSDSensors)
        {
            int psd = recv[1] - 1;
            byte[] value = BitConverter.GetBytes((conn.robot as IPSDSensors).GetPSD(psd));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(value);
            }
            Packet packet = new Packet();
            packet.packetType = PacketType.SERVER_MESSAGE;
            packet.dataSize = 2;
            packet.data = new byte[2];
            value.CopyTo(packet.data, 0);
            serverManager.WritePacket(conn, packet);
        }
        else
        {
            Debug.Log("Get PSD Command from a robot without PSDs");
        }
    }
    // Get Vehicle Pose
    private void Command_q(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IVWDrivable)
        {
            Int16[] pose = (conn.robot as IVWDrivable).GetPose();         
            Packet packet = new Packet();
            packet.packetType = PacketType.SERVER_MESSAGE;
            packet.dataSize = 6;
            packet.data = new byte[6];
            for (int i = 0; i < 3; i++)
            {
                Debug.Log(i + "   " + pose[i]);
                pose[i] = IPAddress.HostToNetworkOrder(pose[i]);
                BitConverter.GetBytes(pose[i]).CopyTo(packet.data, 2 * i);
            }
            
            serverManager.WritePacket(conn, packet);
        }
        else
        {
            Debug.Log("Requested pose from a non posable robot");
        }
    }
    // Set Pose
    private void Command_Q(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            int x = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 1));
            int y = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 3));
            int phi = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 5));
            Debug.Log("Setting pose as x y phi: " + x + " " + y + " " + phi);
            (conn.robot as IVWDrivable).SetPose(x, y, phi);
        }
        else
        {
            Debug.Log("Requested pose from a non posable robot");
        }
    }

    // Get Camera
    private void Command_f(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is ICameras)
        {
            byte[] img = (conn.robot as ICameras).GetCameraOutput(0);
            Packet packet = new Packet();
            packet.packetType = PacketType.SERVER_CAMIMG;
            packet.dataSize = (UInt32)img.Length;
            packet.data = img;
            serverManager.WritePacket(conn, packet);
        }
    }
    // Set Camera
    private void Command_F(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is ICameras)
        {
            int camera = recv[1] - 1;
            int width = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 2));
            int height = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 4));
            (conn.robot as ICameras).SetCameraResolution(camera, width, height);
        }
    }
    // VW Drive Straight
    private void Command_y(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IVWDrivable)
        {
            // Velocity is first byte, distance is second byte
            // Order revered in input array to match function call semantics
            int distance = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 3));
            int speed = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 1));
            (conn.robot as IVWDrivable).VWDriveStraight(distance, speed);
        }
    }
    // VW Drive Turn
    private void Command_Y(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            int velocity = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(recv, 1));
            int angle = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(recv, 3));
            (conn.robot as IVWDrivable).VWDriveTurn(angle, velocity);
        }
    }
    // VW Drive Curve
    private void Command_C(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            int distance = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 1));
            int angle = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 3));
            int speed = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 5));     
            (conn.robot as IVWDrivable).VWDriveCurve(distance, angle, speed);
            Debug.Log("Drive Curve: " + distance + " " + angle + " " + speed);
        }
    }
    // Get Speed
    private void Command_X(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IVWDrivable)
        {
            Packet p = new Packet();
            Speed speed = (conn.robot as IVWDrivable).VWGetVehicleSpeed();
            byte[] lin = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(speed.linear));
            byte[] ang = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(speed.angular));
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 4;
            p.data = new byte[4];
            lin.CopyTo(p.data, 0);
            ang.CopyTo(p.data, 2);
            serverManager.WritePacket(conn, p);
        }
    }
    // Drive Done
    private void Command_Z(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            bool done = (conn.robot as IVWDrivable).VWDriveDone();
            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 1;
            p.data = BitConverter.GetBytes(done);
            serverManager.WritePacket(conn, p);
            (conn.robot as IVWDrivable).VWDriveWait(ReturnDriveDone);
        }
        else
        {
            Debug.Log("Requested drive done from a non VW drivable robot");
        }
    }
    // Drive Remaining
    private void Command_z(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IVWDrivable)
        {
            bool done = (conn.robot as IVWDrivable).VWDriveDone();
            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 1;
            p.data = BitConverter.GetBytes(done);
            serverManager.WritePacket(conn, p);
        }
    }

    public void ReceiveCommand(byte[] recv, RobotConnection conn)
    {
        
        switch ((char)recv[0])
        {
            // Motor Drive Uncontrolled
            case 'm':
                Command_m(recv, conn);
                break;
            // Motor Drive Controlled
            case 'M':
                Command_M(recv, conn);
                break;
            // Set PID Paramters
            case 'd':
                Command_d(recv, conn);
                break;
            // Set Servo Position
            case 's':
                Command_s(recv, conn);
                break;
            // Set Servo Range
            case 'S':
                break;
            // Read PSD Value
            case 'p':
                Command_p(recv, conn);
                break;
            // Get Vehicle Pose (robot coordinates)
            case 'q':
                Command_q(recv, conn);
                break;
            // Set Vehicle Pose (robot coordinates)
            case 'Q':
                Command_Q(recv, conn);
                break;
            // Get Camera Image
            case 'f':
                Command_f(recv, conn);
                break;
            // Set Camera Resolution
            case 'F':
                Command_F(recv, conn);
                break;
            // VW Drive Straight
            case 'y':
                Command_y(recv, conn);
                break;
            // VW Drive Turn
            case 'Y':
                Command_Y(recv, conn);
                break;
            // VW Drive Curve
            case 'C':
                Command_C(recv, conn);
                break;
            // VW Get Speed
            case 'X':
                Command_X(recv, conn);
                break;
            // Drive Done or Stalled
            case 'Z':
                Command_Z(recv, conn);
                break;
            // Drive Remaining
            case 'z':
                Command_z(recv, conn);
                break;
            default:
                Debug.Log("unknown : " + Convert.ToChar(recv[0]));
                Debug.Log("Received an unknown command.");
                break;
        }
    }
}
