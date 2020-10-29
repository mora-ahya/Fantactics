using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public delegate void SendEventHandler(object sender);
    public event SendEventHandler OnSendData;

    public delegate void RecieveEventHandler(object sender, byte[] data);
    public event RecieveEventHandler OnRecieveData;

    public delegate void DisconnectedEventHandler(object sender, EventArgs e);
    public event DisconnectedEventHandler OnDisconnected;

    public delegate void ConnectedEventHandler(Socket client, EventArgs e);
    public event ConnectedEventHandler OnConnected;

    IPHostEntry iPHostInfo;
    IPAddress ipAddress;
    IPEndPoint localEndPoint;

    Socket listener = null;

    /// <summary>
    /// 初期処理
    /// </summary>
    /// <param name="l"></param>
    public void Initialize()
    {
        iPHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ipAddress = iPHostInfo.AddressList[0];
        localEndPoint = new IPEndPoint(ipAddress, 11000);

        listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(100);
        StartAccept();
    }

    /// <summary>
    /// クライアントの受け入れを開始する
    /// </summary>
    void StartAccept()
    {
        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
    }

    /// <summary>
    /// クライアント受け入れのコールバック
    /// </summary>
    /// <param name="ar"></param>
    void AcceptCallback(IAsyncResult ar)
    {
        Socket server = (Socket)ar.AsyncState;
        Socket connectedClient;
        try
        {
            connectedClient = server.EndAccept(ar);
            OnConnected?.Invoke(connectedClient, new EventArgs());
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// クライアントからのデータ受信を開始する
    /// </summary>
    /// <param name="client"></param>
    /// <param name="bufferSize">データのサイズ</param>
    public void StartReceive(Socket client, int bufferSize)
    {
        StateObject state = new StateObject();
        state.workSocket = client;
        state.buffer = new byte[bufferSize];
        client.BeginReceive(state.buffer, 0, bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
    }

    /// <summary>
    /// データ受信のコールバック
    /// </summary>
    /// <param name="ar"></param>
    void ReceiveCallback(IAsyncResult ar)
    {
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;
        //Debug.Log(handler == connectedClient);
        int readSize = handler.EndReceive(ar);
        if (readSize < 1)
        {
            return;
        }
        OnRecieveData?.Invoke(this, state.buffer);

        //ChangeReceiveDataIntoCommand(bb);
        //ServerMain.Instance.SetData(bb);
        //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        //Array.Copy(state.buffer, bb, readSize);
        //bb = ChangeReceiveDataIntoCommand(bb);
        //handler.BeginSend(bb, 0, bb.Length, 0, new AsyncCallback(WriteCallback), state);
    }

    /// <summary>
    /// クライアントにデータを送信する
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public void StartSend(Socket client, byte[] data)
    {
        StateObject state = new StateObject
        {
            workSocket = client,
            buffer = data
        };
        client.BeginSend(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(SendDataCallback), state);
    }

    /// <summary>
    /// データ送信のコールバック
    /// </summary>
    /// <param name="ar"></param>
    void SendDataCallback(IAsyncResult ar)
    {
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;
        handler.EndSend(ar);
        OnSendData?.Invoke(this);
    }

    /// <summary>
    /// サーバを閉じる
    /// </summary>
    /// <param name="s"></param>
    public void DisConnect(Socket s)
    {
        Debug.Log("接続終了");
        if (s != null)
        {
            s.Close();
        }
        listener.Close();
    }
}
