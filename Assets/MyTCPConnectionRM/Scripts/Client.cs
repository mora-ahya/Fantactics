using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client
{
    //通信用のソケット
    Socket mySocket = null;

    readonly object syncLock = new object();

    public delegate void SendEventHandler(object sender);
    public event SendEventHandler OnSendData;

    public delegate void RecieveEventHandler(object sender, byte[] data);
    public event RecieveEventHandler OnRecieveData;

    public delegate void DisconnectedEventHandler(object sender, EventArgs e);
    public event DisconnectedEventHandler OnDisconnected;

    public delegate void ConnectedEventHandler(EventArgs e);
    public event ConnectedEventHandler OnConnected;

    /// <summary>
    /// 初期処理
    /// </summary>
    /// <param name="l"></param>
    public void Initialize()
    {
        mySocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// 通信を切断する
    /// </summary>
    public void Close()
    {
        mySocket.Shutdown(SocketShutdown.Both);
        mySocket.Close();
        mySocket = null;

        OnDisconnected(this, new EventArgs());
    }

    /// <summary>
    /// 通信を開始する
    /// </summary>
    /// <param name="host">ホスト名</param>
    /// <param name="port">ポート番号</param>
    public void Connect(string host, int port)
    {
        IPEndPoint ipEnd = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
        mySocket.BeginConnect(ipEnd, new AsyncCallback(ConnectCallback), mySocket);
    }

    /// <summary>
    /// 通信処理のコールバック
    /// </summary>
    /// <param name="ar"></param>
    void ConnectCallback(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;
        client.EndConnect(ar);
        OnConnected?.Invoke(new EventArgs());
    }

    /// <summary>
    /// サーバからのデータ受信を開始する
    /// </summary>
    /// <param name="bufferSize">受け取るデータサイズ</param>
    public void StartReceive(int bufferSize)
    {
        StateObject state = new StateObject();
        state.workSocket = mySocket;
        state.buffer = new byte[bufferSize];

        mySocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveDataCallback), state);
    }

    /// <summary>
    /// データ受信のコールバック
    /// </summary>
    /// <param name="ar"></param>
    void ReceiveDataCallback(IAsyncResult ar)
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
    }

    /// <summary>
    /// サーバにデータを送信する
    /// </summary>
    /// <param name="bytes">データ</param>
    public void StartSend(byte[] bytes)
    {
        if (mySocket == null)
            return;

        StateObject state = new StateObject();
        state.buffer = bytes;
        state.workSocket = mySocket;

        lock (syncLock)
        {
            mySocket.BeginSend(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), state);
            //mySocket.Send(state.buffer);
        }
        OnSendData?.Invoke(this);
    }

    /// <summary>
    /// データ送信のコールバック
    /// </summary>
    /// <param name="ar"></param>
    void SendCallback(IAsyncResult ar)
    {
        mySocket.EndSend(ar);
    }
}
