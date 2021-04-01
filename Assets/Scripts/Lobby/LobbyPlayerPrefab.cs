using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Steamworks;

public class LobbyPlayerPrefab : NetworkBehaviour {

    // Public
    [Header("Required Compontents")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Text[] playerNames = null;
    [SerializeField] private Text[] playerReadytexts = null;
    [SerializeField] private RawImage[] userImages = null; // For use later
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleSteamIDChanged))]
    private CSteamID userSteamID;
    public CSteamID UserSteamID {
        set {
            userSteamID = value;
        }
        get {
            return userSteamID;
        }
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleSteamIDChanged(CSteamID oldSteamId, CSteamID newSteamId) => UpdateDisplay();

    // Private
    private LobbyNetworkManager networkManager;
    private MenuManager menuManager;

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

    // Steam Callbacks
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    void Start() {
        networkManager = FindObjectOfType<LobbyNetworkManager>();
        menuManager = FindObjectOfType<MenuManager>();
        menuManager.HideLoading();
    }

    private void Update() {
        UpdateDisplay();
    }

    public override void OnStartAuthority() {
        CmdSetDisplayName(SteamFriends.GetFriendPersonaName(this.UserSteamID));
        CmdSetSteamID(SteamUser.GetSteamID());

        for (int i = 0; i < playerNames.Length; i++) {
            playerNames[i].color = Color.grey;
        }
        if (hasAuthority) {
            lobbyUI.SetActive(true);

        }
    }

    public override void OnStartClient() {
        Lobby.LobbyPlayers.Add(this);
        UpdateDisplay();

        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    public override void OnStopClient() {
        Lobby.LobbyPlayers.Remove(this);

        if (hasAuthority)
            menuManager.NavigateToMenu("Error", "<color=red>Disconnected!</color>");

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

        // Resets player names
        for (int i = 0; i < playerNames.Length; i++) {
            playerReadytexts[i].text = string.Empty;
            playerNames[i].color = Color.grey;
            playerNames[i].text = "Waiting For Player...";
        }

        // Resets player display images
        for (int i = 0; i < userImages.Length; i++) {
            Color grey = new Color(.2f, .2f, .2f, 1f); // Dark-grey
            userImages[i].color = Color.black;
            userImages[i].texture = null;
        }

        for (int i = 0; i < Lobby.LobbyPlayers.Count; i++) {
            // Player Names
            playerNames[i].text = SteamFriends.GetFriendPersonaName(Lobby.LobbyPlayers[i].UserSteamID);
            if (Lobby.LobbyPlayers[i].UserSteamID == this.UserSteamID) {
                playerNames[i].color = Color.yellow;
            } else {
                playerNames[i].color = Color.white;
            }
            /* 
            // Extra indication of who the player is in lobby
            if (Lobby.LobbyPlayers[i].UserSteamID == this.UserSteamID) {
                playerNames[i].text += "(you)";
            } */

            // Player Ready Text
            playerReadytexts[i].text = Lobby.LobbyPlayers[i].IsReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>";

            // Player Images
            userImages[i].color = Color.white;

            int imageId = SteamFriends.GetLargeFriendAvatar(Lobby.LobbyPlayers[i].UserSteamID);

            if (imageId == -1) { return; }

            userImages[i].texture = GetSteamImageAsTexture2D(imageId);
        }
    }

    private Texture2D GetSteamImageAsTexture2D(int iImage) {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid) {
            uint bufferSize = width * height * 4; // 4 to get 4 bits per pixel (RGBA)

            byte[] image = new byte[bufferSize];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)bufferSize);

            if (isValid) {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback) {
        for (int i = 0; i < Lobby.LobbyPlayers.Count; i++) {
            if (callback.m_steamID != Lobby.LobbyPlayers[i].UserSteamID) { return; }
            userImages[i].texture = GetSteamImageAsTexture2D(callback.m_iImage);
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
    private void CmdSetSteamID(CSteamID id) {
        UserSteamID = id;
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
        Lobby.StartGame();
        //networkManager.SendGameStartNotice();
    }
}
