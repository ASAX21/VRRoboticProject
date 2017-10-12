using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : MonoBehaviour {

    public Queue<byte[]> msgQueue = new Queue<byte[]>();

    public int numMessages = 0;
    private int totalMessageLength = 0;
    private int maxByteInQueue = 1024;

    public Action<byte[]> receivedCallback = null;

    // Add a message to the message queue
    public void QueueMessage(byte[] msg)
    {
        if ((totalMessageLength + msg.Length) > maxByteInQueue) {
            Debug.Log("Maximum receive buffer reached");
            return;
        }
        msgQueue.Enqueue(msg);
        totalMessageLength += msg.Length;
        numMessages++;
    }

    // Return a message from the queue
    public byte[] DequeueMessage()
    {
        if (msgQueue.Count == 0) return null;
        byte[] msg = msgQueue.Dequeue();
        totalMessageLength -= msg.Length;
        numMessages--;
        return msg;
    }

    private void Update()
    {
        if(receivedCallback != null && msgQueue.Count > 0)
        {
            receivedCallback(msgQueue.Dequeue());
            receivedCallback = null;
        }
    }
}
