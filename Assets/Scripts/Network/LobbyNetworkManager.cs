using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Steamworks;

public class LobbyNetworkManager : NetworkManager {
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
        NotifyPlayersOfReadyState();

        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        NotifyPlayersOfReadyState(); // WORKS?

        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn) {
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
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<LobbyPlayerPrefab>();

            LobbyPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public void NotifyPlayersOfReadyState() {
        foreach (var player in LobbyPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart() {
        if (numPlayers < minimumPlayers) {
            return false;
        }

        foreach (var player in LobbyPlayers) {
            if (!player.IsReady) {
                return false;
            }
        }

        return true;
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        if (SceneManager.GetActiveScene().path == menuScene) {
            LobbyPlayerPrefab lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);

            bool isLeader = LobbyPlayers.Count == 0; // If first player in lobby
            lobbyPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);

            NotifyPlayersOfReadyState();
        }
    }

    public override void OnStopServer() {
        LobbyPlayers.Clear();
    }

    // - - - - - - - SERVER NOTIFICATIONS - - - - - - - -

    [SerializeField] private string notificationMessage = string.Empty;

    [ContextMenu("Send Notification")]
    private void SendNotification() {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }

    public void SendGameStartNotice() {
        StopCoroutine("StartGameCountdown");
        StartCoroutine("StartGameCountdown");
    }

    private IEnumerator StartGameCountdown() {
        int i = 4;
        string _content = string.Empty;
        while (i > 0) {
            if (i > 1) {
                _content = $"Game starts in {(i - 1)}...";
            } else {
                _content = "Game starting...";
            }
            NetworkServer.SendToAll(new Notification { content = _content });
            yield return new WaitForSeconds(1);
            i--;
        }
    }
}
