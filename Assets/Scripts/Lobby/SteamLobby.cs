using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour {

    // Public 
    [SerializeField] MenuManager menuManager = null;

    // Private
    private LobbyNetworkManager networkManager = null;
    private const string HostAdressKey = "HostAdress";

    // Steam Callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    void Start() {
        networkManager = GetComponent<LobbyNetworkManager>();

        if (!SteamManager.Initialized) { 
            menuManager.NavigateToMenu("Error", "The game needs to be launched through Steam!");
            return; 
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby() {
        menuManager.NavigateToMenu("Loading");
        try {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        } catch(System.Exception e) {
            menuManager.NavigateToMenu("Error", e.Message);
        }

    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        if (callback.m_eResult != EResult.k_EResultOK) {
            menuManager.NavigateToMenu("Landing");
            return;
        }

        menuManager.NavigateToMenu("Lobby");
        networkManager.StartHost();
        
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAdressKey,
            SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        menuManager.NavigateToMenu("Loading");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        if (NetworkServer.active) { return; }

        string hostAdress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAdressKey);

        networkManager.networkAddress = hostAdress;

        networkManager.StartClient();
        menuManager.NavigateToMenu("Lobby");
    }

    public void LeaveLobby() {
        networkManager.StopHost();

        menuManager.NavigateToMenu("Landing");
    }

}
