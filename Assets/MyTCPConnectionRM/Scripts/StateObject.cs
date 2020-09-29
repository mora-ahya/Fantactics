using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class StateObject
{
    public Socket workSocket = null;
    public byte[] buffer;
}
