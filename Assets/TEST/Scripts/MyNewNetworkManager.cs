using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MyNewNetworkManager : NetworkManager {
    // Public
    [Header("Lobby Settings")]
    [SerializeField] private int minimumPlayers = 2;

    [Header("Lobby Objects")]
    [SerializeField] private LobbyPlayerPrefab lobbyPlayerPrefab = null;
    [Scene] [SerializeField] private string menuScene = string.Empty;
  
    public static event System.Action OnClientConnected;
    public static event System.Action OnClientDisconnected;

    public List<LobbyPlayerPrefab> LobbyPlayers { get; } = new List<LobbyPlayerPrefab>();

    public override void OnStartServer() {
        //spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }
    public override void OnStartClient() {
        // Load all spawnable prefabs


    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn) {
        //base.OnServerConnect(conn);

        if (numPlayers >= maxConnections) {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != menuScene) {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);

        // For lobby 
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        //base.OnServerAddPlayer(conn);

        if (SceneManager.GetActiveScene().path == menuScene) {
            LobbyPlayerPrefab lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);


            Debug.Log("Adding player");
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }
}
