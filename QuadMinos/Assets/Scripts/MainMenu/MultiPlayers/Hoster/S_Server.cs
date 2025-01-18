using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Conn
{
    private const int BUFFER_SIZE = 8192;

    public byte[] buffer;
    public int offset;
    public Socket socket;
    public bool active;

    public Conn ()
    {
        buffer = new byte[BUFFER_SIZE];
        offset = 0;
        active = false;
    }

    ~Conn()
    {
        buffer = null;
    }

    public void Init(Socket _socket)
    {
        socket = _socket;
        offset = 0;
        active = true;
        return;
    }

    public EndPoint GetAddress()
    {
        if (active) return null;
        return socket.RemoteEndPoint;
    }

    public int GetBufferRemain()
    {
        return BUFFER_SIZE - offset;
    }

    public void Close()
    {
        if (active) active = false;
        if (socket == null) return;
        socket.Close();
        return;
    }
}

public class Server
{
    const int MAX_CLIENTS_SIZE = 31;
    public Socket listenfd;
    public Conn[] conns = null;

    public int GetNewConn()
    {
        if (conns == null) conns = new Conn[MAX_CLIENTS_SIZE];
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null) conns[i] = new Conn();
            if (conns[i].active) continue;
            return i;
        }
        return -1;
    }

    public void Init(IPEndPoint _ipEp)
    {
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listenfd.Bind(_ipEp);
        listenfd.Listen(MAX_CLIENTS_SIZE);
        // Accept
        listenfd.BeginAccept(Acception, null);
    }

    private void Reciption(IAsyncResult _rst)
    {
        Conn conn = _rst.AsyncState as Conn;
        try
        {
            int messageSize = conn.socket.EndReceive(_rst);
            if (messageSize <= 0)
            {
                conn.Close();
                return;
            }
            string str = System.Text.Encoding.ASCII.GetString(conn.buffer, conn.offset, messageSize);

            // Send to All connections
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(str);
            foreach (Conn c in conns)
            {
                if (c.active)
                {
                    c.socket.Send(bytes);
                }
            }

            conn.socket.BeginReceive(conn.buffer, conn.offset, conn.GetBufferRemain(), SocketFlags.None, Reciption, conn);
        }
        catch (Exception ex)
        {
            Debug.Log("[Server] Receive Error: " + ex);
        }
    }

    private void Acception(IAsyncResult _rst) {
        try
        {
            Socket clientSocket = listenfd.EndAccept(_rst);
            int ind = GetNewConn();

            if (ind < 0)
            {
                clientSocket.Close();
                Debug.Log("[Server] Accept Error: conns' full.");
            }
            else
            {
                conns[ind].Init(clientSocket);

                // Receive
                conns[ind].socket.BeginReceive(conns[ind].buffer, conns[ind].offset, conns[ind].GetBufferRemain(), SocketFlags.None, Reciption, conns[ind]);
            }

            // Reacception
            listenfd.BeginAccept(Acception, null);
        }
        catch (Exception ex)
        {
            Debug.Log("[Server] Accept Error: " + ex);
        }
    }
}

public class S_Server : MonoBehaviour
{
    
}
