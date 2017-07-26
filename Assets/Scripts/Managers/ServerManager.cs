using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System;

///<summary>
/// Server manager handles network traffic and socket communication
/// between the simulator and the client program.
/// </summary>

[assembly: InternalsVisibleTo("Interpreter")]
internal class Packet
{
    // 5 bytes header
    internal byte packetType;
    internal UInt32 dataSize;
    // payload
    internal byte[] data;
}

// Robot connection class contains reference to the socket connection,
// and the corresponding simulated robot
public class RobotConnection
{
    public TcpClient tcpClient;
    public Robot robot;

    public int ID;
    public bool inScene = false;

    public RobotConnection(TcpClient newClient, int newID)
    {
        tcpClient = newClient;
        ID = newID;
    }

    public void AddRobotToScene(Robot newRobot)
    {
        robot = newRobot;
    }
}

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance { get; private set; }

    List<RobotConnection> conns = new List<RobotConnection>();

    public Robot activeRobot;
    private int robotIDs = 1;

    TcpListener listener = null;
    int port = 8888;
    IPAddress localAddr = IPAddress.Parse("127.0.0.1");
    
    public Interpreter interpreter;

    byte[] recvBuf = new byte[1024];
    string data = null;

    void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Create an interpreter for RoBIOS Commands
        interpreter = new Interpreter();
        interpreter.serverManager = this;

        // Listener for TCP connections
        listener = new TcpListener(localAddr, port);
        listener.Start();

        // Periodically check if connections are active
        StartCoroutine(CheckConnections());
        Debug.Log("Server Started");
    }

    // Terminate a connection, and remove form the connection list
    private void CloseConnection(RobotConnection conn)
    {
        conn.tcpClient.Close();
        if (!conns.Remove(conn))
            Debug.Log("Failed to remove connection");
    }

    // Handshake connection with control program
    private void ReplyHandshake(RobotConnection conn)
    {
        Packet p = new Packet();
        p.packetType = PacketType.SERVER_HANDSHAKE;
        p.dataSize = 0;
        WritePacket(conn, p);

        p = new Packet();
        p.packetType = PacketType.SERVER_READY;
        p.dataSize = 0;
        WritePacket(conn, p);
    }

    // Accept a pending connection
    private void AcceptConnection()
    {
        // Check if there is a robot ready to receive control
        if (activeRobot == null)
        {
            Debug.Log("Control program connected but no robot active");
            return;
        }
        else
        {
            // Create a new TcpClient to receive messages
            TcpClient client = listener.AcceptTcpClient();
            RobotConnection newClient = new RobotConnection(client, robotIDs);

            // Associate the RobotConnection 
            newClient.robot = activeRobot;
            activeRobot.myConnection = newClient;
            conns.Add(newClient);
            robotIDs++;
            
            // Reply to the robot to begin control
            Debug.Log("Accepted a connection");
            ReplyHandshake(newClient);
        }
    }

    // Write a packet to a connection
    internal void WritePacket(RobotConnection conn, Packet packet)
    {
        byte[] sendBuf = new byte[packet.dataSize + 5];
        UInt32 size = packet.dataSize;

        if (BitConverter.IsLittleEndian)
            size = RobotFunction.ReverseBytes(size);

        sendBuf[0] = Convert.ToByte(packet.packetType);
        BitConverter.GetBytes(size).CopyTo(sendBuf, 1);
        if (packet.dataSize > 0)
        {
            packet.data.CopyTo(sendBuf, 5);
        }

        NetworkStream stream = conn.tcpClient.GetStream();
        stream.Write(sendBuf, 0, ((int)packet.dataSize) + 5);
    }

    // Read a packet from a connection
    // DataAvailable flag must be true before calling
    private void ReadPacket(RobotConnection conn)
    {
        NetworkStream stream = conn.tcpClient.GetStream();

        // Read Header
        int bytesRead = stream.Read(recvBuf, 0, 5);

        // Check the read is successful
        if(bytesRead != 5)
        {
            // If failed, flush the read buffer
            Debug.Log("Failed to read packet header");
            while (stream.DataAvailable)
            {
                stream.Read(recvBuf, 0, recvBuf.Length);
            }
            return;
        }
        uint dataSize = BitConverter.ToUInt32(recvBuf,1);
        if (BitConverter.IsLittleEndian)
        {
            dataSize = RobotFunction.ReverseBytes(dataSize);
        }
        int packetType = recvBuf[0];

        // Read Body
        if (dataSize > 0)
        {
            bytesRead = stream.Read(recvBuf, 0, (int)dataSize);
        }
        switch(packetType){
            case PacketType.CLIENT_HANDSHAKE:
                if(conn.robot == null)
                {
                    
                }
                break;
            case PacketType.CLIENT_MESSAGE:
                interpreter.ReceiveCommand(recvBuf, conn);
                break;
            case PacketType.CLIENT_DISCONNECT:
                DisconnectRobot(conn);
                break;
            default:
                break;
        }
    }

    public void DisconnectRobot(RobotConnection conn)
    {
        conn.robot.myConnection = null;
        conns.Remove(conn);
    }

    void Update ()
    {
        if (listener.Pending())
        {
            AcceptConnection();
        }
        // Check each connection for a message
        foreach (RobotConnection conn in conns)
        {
            NetworkStream stream = conn.tcpClient.GetStream();
            if (stream.DataAvailable)
                ReadPacket(conn);
        }
    }

    // Check each open connection every 5 seconds
    private IEnumerator CheckConnections()
    {
        while (true)
        {
            foreach (RobotConnection conn in conns)
            {
                if (conn.tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buf = new byte[1];
                    if (conn.tcpClient.Client.Receive(buf, SocketFlags.Peek) == 0)
                    {
                        Debug.Log("Connection lost");
                        CloseConnection(conn);
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(5);
        }
    }
}
