using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;

public class GamePlayerPrefab : NetworkBehaviour {

    // Public

    [SyncVar]
    public string DisplayName = "Loading...";
    [SyncVar]
    private CSteamID userSteamID;
    public CSteamID UserSteamID {
        set {
            userSteamID = value;
        }
        get {
            return userSteamID;
        }
    }

    // Private
    private LobbyNetworkManager networkManager;
    private MenuManager menuManager;

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

    // Steam Callbacks
    //protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    void Start() {
        networkManager = FindObjectOfType<LobbyNetworkManager>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    public override void OnStartClient() {
        DontDestroyOnLoad(gameObject);
        Lobby.GamePlayers.Add(this);

        //avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    public override void OnStopClient() {
        Lobby.GamePlayers.Remove(this);
    }

    [Server] // ???? or not ????
    public void SetSteamID(CSteamID id) {
        this.UserSteamID = id;
    }
}
