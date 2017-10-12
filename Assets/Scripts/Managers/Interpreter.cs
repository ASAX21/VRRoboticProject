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
/// 

public class Interpreter {

    public ServerManager serverManager;

    public void ReturnDriveDone(RobotConnection conn)
    {
        Packet p = new Packet();
        p.packetType = PacketType.DRIVE_DONE;
        p.dataSize = 0;
        serverManager.WritePacket(conn, p);
    }

    public void ForwardRadioMessage(RobotConnection conn, byte[] msg)
    {
        Debug.Log("Forwarding a Packet");
        Packet p = new Packet();
        p.packetType = PacketType.RADIO_MESSAGE;
        p.dataSize =  msg.Length;
        p.data = msg;
        serverManager.WritePacket(conn, p);
    }
    // Get Encoder
    private void Command_e(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IMotors)
        {
            int quad = recv[1] - 1;
            int tick = (conn.robot as IMotors).GetEncoder(quad);
            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 4;
            p.data = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(tick));
            serverManager.WritePacket(conn, p);
        }
    }

    // Drive Motor Uncontrolled
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
            int angle = Convert.ToInt32(recv[2]);
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
        {           Debug.Log("Get PSD Command from a robot without PSDs");
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
            packet.dataSize = img.Length;
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

    // Set Speed
    private void Command_x(byte[] recv, RobotConnection conn)
    {
        if(conn.robot is IVWDrivable)
        {
            int linSpeed = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 1));
            int angSpeed = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(recv, 3));
            (conn.robot as IVWDrivable).VWSetVehicleSpeed(linSpeed, angSpeed);
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
        }
        else
        {
            Debug.Log("Requested drive done from a non VW drivable robot");
        }
    }

    //Drive Wait
    private void Command_L(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            (conn.robot as IVWDrivable).VWDriveWait(ReturnDriveDone);
        }
        else
        {
            Debug.Log("Requested drive wait from a non VW drivable robot");
        }
    }

    // Drive Remaining
    private void Command_z(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IVWDrivable)
        {
            bool done = (conn.robot as IVWDrivable).VWDriveDone();
            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 1;
            p.data = BitConverter.GetBytes(done);
            serverManager.WritePacket(conn, p);
        }
    }

    // Play Beep
    private void Command_b(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IAudio)
        {
            (conn.robot as IAudio).PlayBeep();
        }
    }

    // Send Radio Message (Add to receiver's buffer)
    private void Command_R(byte[] recv, RobotConnection conn){     
        if(conn.robot is IRadio)
        {
            // Get ID of sender, and length of null-terminated string
            int id = BitConverter.ToInt32(recv, 1);
            int strlen = Array.IndexOf(recv, (byte)0, 5) - 4;
            id = IPAddress.NetworkToHostOrder(id);
            if (strlen <= 0)     
                return;
            // Forward message to robot as id | string
            Robot receiver = SimManager.instance.GetRobotByID(id);
            if ( (receiver != null) && (receiver is IRadio) )
            {
                byte[] msg = new byte[4 + strlen];
                int fromId = IPAddress.HostToNetworkOrder(conn.robot.objectID);
                BitConverter.GetBytes(fromId).CopyTo(msg, 0);
                Array.Copy(recv, 5, msg, 4, strlen);
                (receiver as IRadio).AddMessageToBuffer(msg);
            }
        }
    }

    // Receive Radio Message (Send to receiver)
    private void Command_r(byte[] recv, RobotConnection conn) {
        if(conn.robot is IRadio)
        {
            Debug.Log("Receive Radio");
            byte[] msg = (conn.robot as IRadio).RetrieveMessageFromBuffer();
            if (msg == null)
            {
                (conn.robot as IRadio).WaitForRadioMessage(ForwardRadioMessage);
                return;
            }
            ForwardRadioMessage(conn, msg);
        }
    }

    // Check the Radio Message Buffer
    private void Command_c(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is IRadio)
        {
            int numMsgs = (conn.robot as IRadio).GetNumberOfMessages();
            Debug.Log("num msgs = " + numMsgs);
            numMsgs = IPAddress.HostToNetworkOrder(numMsgs);

            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 4;
            p.data = new byte[p.dataSize];
            BitConverter.GetBytes(numMsgs).CopyTo(p.data, 0);
            serverManager.WritePacket(conn, p);
        }
        else
        {
            Debug.Log("Radio Check on a non radio robot");
        }
    }
    // Get own ID
    private void Command_i(byte[] recv, RobotConnection conn)
    {
        int id = conn.robot.objectID;
        id = IPAddress.HostToNetworkOrder(id);

        Packet p = new Packet();
        p.packetType = PacketType.SERVER_MESSAGE;
        p.dataSize = 4;
        p.data = new byte[p.dataSize];
        BitConverter.GetBytes(id).CopyTo(p.data, 0);
        serverManager.WritePacket(conn, p);
    }
    // Get all other IDs
    private void Command_I(byte[] recv, RobotConnection conn)
    {
        int numBots = SimManager.instance.allRobots.Count;
        Debug.Log("numBots: " + numBots);
        Packet p = new Packet();
        p.packetType = PacketType.SERVER_MESSAGE;
        p.dataSize = 4 + (numBots * 4);
        p.data = new byte[p.dataSize];
        // Get ID of each robot
        for (int i = 0; i < numBots; i++)
        {
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(SimManager.instance.allRobots[i].objectID))
                .CopyTo(p.data, 4 * (i + 1));
        }
        // Write number of robots
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(numBots)).CopyTo(p.data, 0);
        serverManager.WritePacket(conn, p);
    }
    // Laser scan
    private void Command_l(byte[] recv, RobotConnection conn)
    {
        if (conn.robot is ILaser)
        {
            Packet p = new Packet();
            p.packetType = PacketType.SERVER_MESSAGE;
            p.dataSize = 1440;
            p.data = new byte[p.dataSize];
            int[] vals = (conn.robot as ILaser).LaserScan();
            for (int i = 0; i < vals.Length; i++)
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(vals[i])).CopyTo(p.data, i * 4);
            serverManager.WritePacket(conn, p);
        }
        else
        {
            Debug.Log("Robot cannot perform a laser scan");
        }
    }

    // Get Robot Pose
    private void Command_1(byte[] recv, RobotConnection conn)
    {
        int xPos = (int) Mathf.Round( Eyesim.Scale * conn.robot.transform.position.x);
        int yPos = (int) Mathf.Round( Eyesim.Scale * conn.robot.transform.position.z);
        int phi = (int)Mathf.Round((360 - conn.robot.transform.rotation.eulerAngles.y) % 360);

        Packet p = new Packet();
        p.packetType = PacketType.SERVER_MESSAGE;
        p.dataSize = sizeof(int) * 3;
        p.data = new byte[p.dataSize];
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(xPos)).CopyTo(p.data, 0);
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(yPos)).CopyTo(p.data, 4);
        BitConverter.GetBytes(IPAddress.HostToNetworkOrder(phi)).CopyTo(p.data, 8);
        serverManager.WritePacket(conn, p);
    }

    // Set Robot Pose
    private void Command_2(byte[] recv, RobotConnection conn)
    {
        int xPos =IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 1));
        int yPos = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 5));
        int phi = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 9));
        conn.robot.SetRobotPosition(xPos, yPos, phi);
    }

    // Get Object Pose
    private void Command_3(byte[] recv, RobotConnection conn)
    {
        int obj = BitConverter.ToInt32(recv, 1);
        obj = IPAddress.NetworkToHostOrder(obj);

        int[] pose = SimManager.instance.GetObjectPoseByID(obj);
        Packet p = new Packet();
        p.packetType = PacketType.SERVER_MESSAGE;
        p.dataSize = sizeof(int) * 3 + 1;
        p.data = new byte[p.dataSize];
        if (pose == null)
            Array.Clear(p.data, 0, 13);
        else
        {
            p.data[0] = 1;
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(pose[0])).CopyTo(p.data, 1);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(pose[1])).CopyTo(p.data, 5);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder((360 - pose[2]) % 360)).CopyTo(p.data, 9);       
        }
        serverManager.WritePacket(conn, p);
    }

    // Set Object Pose
    private void Command_4(byte[] recv, RobotConnection conn)
    {
        int id = BitConverter.ToInt32(recv, 1);
        id = IPAddress.NetworkToHostOrder(id);

        int x = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 5));
        int y = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 9));
        int phi = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(recv, 13));

        PlaceableObject obj = SimManager.instance.GetObjectByID(id);
        if (obj == null)
            Debug.Log("Set Position: Invalid ID argument");
        else
        {
            obj.transform.position = new Vector3(x / Eyesim.Scale, obj.defaultVerticalOffset, y / Eyesim.Scale);
            obj.transform.rotation = Quaternion.Euler(new Vector3(0, phi, 0));
        }
        
    }

    public void ReceiveCommand(byte[] recv, RobotConnection conn)
    {
        switch ((char)recv[0])
        {
            // Read Encoder
            case 'e':
                Command_e(recv, conn);
                break;
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
            // VW Set Speed
            case 'x':
                Command_x(recv, conn);
                break;
            // Drive Done or Stalled
            case 'Z':
                Command_Z(recv, conn);
                break;
            // Drive Wait
            case 'L':
                Command_L(recv, conn);
                break;
            // Drive Remaining
            case 'z':
                Command_z(recv, conn);
                break;
            // Send Radio Message
            case 'R':
                Command_R(recv, conn);
                break;
            // Receive Radio message
            case 'r':
                Command_r(recv, conn);
                break;
            // Radio Check
            case 'c':
                Command_c(recv, conn);
                break;
            // Get ID
            case 'i':
                Command_i(recv, conn);
                break;
            // Get all other IDs
            case 'I':
                Command_I(recv, conn);
                break;
            // Laser scan
            case 'l':
                Command_l(recv, conn);
                break;
            // Sim Get Pose
            case '1':
                Command_1(recv, conn);
                break;
            // Sim Set Pose
            case '2':
                Command_2(recv, conn);
                break;
            // Sim Get Object (position)
            case '3':
                Command_3(recv, conn);
                break;
            // Sim Set Object (position)
            case '4':
                Command_4(recv, conn);
                break;
            // Play beep
            case 'b':
                Command_b(recv, conn);
                break;
            default:
                Debug.Log("unknown : " + Convert.ToChar(recv[0]));
                Debug.Log("Received an unknown command.");
                break;
        }
    }
}
