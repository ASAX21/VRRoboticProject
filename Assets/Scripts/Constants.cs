using System.Collections;
using System.Collections.Generic;

public static class PacketType  {

    public const byte CLIENT_HANDSHAKE = 0x01;
    public const byte SERVER_HANDSHAKE = 0x02;
    public const byte CLIENT_DISCONNECT = 0x03;
    public const byte SERVER_DISCONNECT = 0x04;
    public const byte SERVER_READY = 0x05;
    public const byte SERVER_STOP = 0x06;
    public const byte CLIENT_START = 0x07;
    public const byte CLIENT_STOP = 0x08;
    public const byte SERVER_MESSAGE = 0x09;
    public const byte CLIENT_MESSAGE = 0x0A;
    public const byte SERVER_CAMIMG = 0x0B;
    public const byte CLIENT_CAMGET = 0x0C;
    public const byte DRIVE_DONE = 0x0D;
    public const byte RADIO_MESSAGE = 0x0E;
}

public static class Layers
{
    public const int GroundLayer = 10;
}
