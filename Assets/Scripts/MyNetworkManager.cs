using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public static new MyNetworkManager singleton;

    public override void Awake() {
        base.Awake();
        singleton = this;
    }

    public override void OnStartServer() {
        base.OnStartServer();
        Debug.Log("Server Started");
        
    }

    public override void OnStopServer() {
        base.OnStopServer();
        Debug.Log("Server Stopped");
    }

    public override void OnClientConnect() {
        base.OnClientConnect();
        Debug.Log("Connected to Server");
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        Debug.Log("Disconnected to Server");
    }

    
}
