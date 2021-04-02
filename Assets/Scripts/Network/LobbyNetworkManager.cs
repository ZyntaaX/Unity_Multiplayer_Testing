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

    [Header("Lobby")]
    [SerializeField] private LobbyPlayerPrefab lobbyPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private GamePlayerPrefab gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;

    [Header("Scenes")]
    [Scene] [SerializeField] private string menuScene = string.Empty;

    public static event System.Action OnClientConnected;
    public static event System.Action OnClientDisconnected;
    public static event System.Action<NetworkConnection> OnServerReadied;

    public List<LobbyPlayerPrefab> LobbyPlayers { get; } = new List<LobbyPlayerPrefab>();
    public List<GamePlayerPrefab> GamePlayers { get; } = new List<GamePlayerPrefab>();

    // - - - - - - - LOBBY - - - - - - - -

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
        NotifyPlayersOfReadyState();

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

    // - - - - - - - GAME - - - - - - - -

    public void StartGame() {
        if (SceneManager.GetActiveScene().path == menuScene) {
            if (!IsReadyToStart()) { return; }

            StopCoroutine("StartGameCoroutine");
            StartCoroutine("StartGameCoroutine");
        }
    }

    private IEnumerator StartGameCoroutine() {
        int i = 4;
        string _content = string.Empty;
        while (i > 0) {
            if (IsReadyToStart()) {
                if (i > 1) {
                    _content = $"Game starts in {(i - 1)}...";
                } else {
                    _content = "<color=yellow>Game is about to start...</color>";
                }

                NetworkServer.SendToAll(new Notification { content = _content });
                yield return new WaitForSeconds(1);
                i--;
            } else {
                NetworkServer.SendToAll(new Notification { content = "<color=red>Cancelled</color>" });
                break;
            }
        }

        if (IsReadyToStart()) { ServerChangeScene("Scene_Map_01"); }
    }

    public override void ServerChangeScene(string newSceneName) {
        // From menu to game
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Scene_Map")) {
            for (int i = LobbyPlayers.Count - 1; i >= 0; i--) {
                var conn = LobbyPlayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(gamePlayerPrefab);
                gamePlayerInstance.UserSteamID = LobbyPlayers[i].UserSteamID;
                Debug.Log($"Check steamID, (not 0?): {LobbyPlayers[i].UserSteamID}");

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName) {
        if (sceneName.StartsWith("Scene_Map")) {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
