using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager {

    public override void OnStartServer() {
        Debug.Log("Server Start");
        base.OnStartServer();
    }

    public override void OnStopServer() {
        Debug.Log("Server Stop");
        base.OnStopServer();

    }

    public override void OnClientConnect(NetworkConnection conn) {
        Debug.Log("Client Connect");
        base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        Debug.Log("Client Disconnect");
        base.OnClientDisconnect(conn);
    }
}
