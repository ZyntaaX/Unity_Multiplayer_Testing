using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {

    //[Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Lobby Settings")]
    [SerializeField] private int minimumPlayers = 2;

    [Header("Lobby Objects")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    public static event System.Action OnClientConnected;
    public static event System.Action OnClientDisconnected;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

    /*public override void OnStartServer() {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList(); //FIX
    }*/

    public override void OnStartClient() {
        /*var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs) {
            NetworkClient.RegisterPrefab(prefab); //WORKS? - ClientScene.RegisterPrefab(prefab);
        }*/
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
        if (numPlayers >= maxConnections) {
            conn.Disconnect();
            return;
        }
        /*
        if (SceneManager.GetActiveScene().name != menuScene) {
            conn.Disconnect();
            return;
        }
        */
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        //if (SceneManager.GetActiveScene().name == menuScene) {
        NetworkRoomPlayerLobby roomplayerInstance = Instantiate(roomPlayerPrefab);

        bool isLeader = RoomPlayers.Count == 0; // If first player in lobby

        roomplayerInstance.IsLeader = isLeader;

        NetworkServer.AddPlayerForConnection(conn, roomplayerInstance.gameObject);
        //}
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public void NotifyPlayersOfReadyState() {
        foreach (var player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public bool IsReadyToStart() {
        if (numPlayers < minimumPlayers) {
            return false;
        }

        foreach (var player in RoomPlayers) {
            if (!player.IsReady) {
                return false;
            }
        }

        return true;
    }

    public override void OnStopServer() {
        RoomPlayers.Clear();
    }
}
