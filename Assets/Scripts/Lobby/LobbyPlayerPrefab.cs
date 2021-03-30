using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyPlayerPrefab : NetworkBehaviour {
    // Public
    [Header("Required Compontents")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Text[] playerNames = null;
    [SerializeField] private Text[] playerReadytexts = null;
    [SerializeField] private RawImage[] UserImages = null; // For use later
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    // Private
    private LobbyNetworkManager networkManager;

    private bool isLeader;
    public bool IsLeader {
        set {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    // Creates a singleton of the lobby
    private LobbyNetworkManager lobby;
    public LobbyNetworkManager Lobby {
        get {
            if (lobby != null) {
                return lobby;
            }

            return lobby = LobbyNetworkManager.singleton as LobbyNetworkManager;
        }
    }

    void Start() {
        networkManager = FindObjectOfType<LobbyNetworkManager>();
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(PlayerPrefs.GetString(PlayerPrefStrings.playerDisplayNamePref));

        for (int i = 0; i < playerNames.Length; i++) {
            playerNames[i].color = Color.grey;
        }

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient() {
        Lobby.LobbyPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient() {
        Lobby.LobbyPlayers.Remove(this);

        UpdateDisplay();
    }

    public void UpdateDisplay() {
        // Make sure the right player updates
        if (!hasAuthority) {
            foreach (var player in Lobby.LobbyPlayers) {
                if (player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        // Updates the lobby UI
        for (int i = 0; i < playerNames.Length; i++) {
            playerReadytexts[i].text = string.Empty;
            playerNames[i].color = Color.grey;
            playerNames[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < Lobby.LobbyPlayers.Count; i++) {
            playerNames[i].text = Lobby.LobbyPlayers[i].DisplayName;
            playerNames[i].color = Color.white;
            playerReadytexts[i].text = Lobby.LobbyPlayers[i].IsReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart) {
        if (!isLeader)
            return;

        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName) {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp() {
        IsReady = !IsReady;

        Lobby.NotifyPlayersOfReadyState();
        //Lobby.NotifyPlayersOfLobbyChange();
    }

    [Command]
    public void CmdStartGame() {
        if (Lobby.LobbyPlayers[0].connectionToClient != connectionToClient) {
            return;
        }

        //Start Game
        Debug.Log("Starting Game...");
        networkManager.SendGameStartNotice();
    }
}
