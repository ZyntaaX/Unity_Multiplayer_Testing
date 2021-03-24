using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NewJoinLobbyManager : MonoBehaviour {

    // Public
    [Header("Required Components")]
    [SerializeField] private MyNewNetworkManager networkManager = null;
    [SerializeField] private MenuManager menuManager = null;
    [SerializeField] private Text errorMessage = null;
    [SerializeField] private InputField ipAdressInputField = null;
    [SerializeField] private InputField displayNameInputField = null;
    [SerializeField] private Button joinLobbyButton = null;

    // Private
    private const string PlayerPrefNameKey = "PlayerPrefName";
    private string displayName = null;

    void Start() {
        DisplayNameSetup();

        ipAdressInputField.text = "localhost"; // For development only
    }

    void Update() {
        if (string.IsNullOrEmpty(displayNameInputField.text) || string.IsNullOrEmpty(ipAdressInputField.text)) {
            joinLobbyButton.interactable = false;
        } else {
            joinLobbyButton.interactable = true;
        }
    }

    private void OnEnable() {
        errorMessage.enabled = false;

        MyNewNetworkManager.OnClientConnected += HandleClientConnected;
        MyNewNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {
        MyNewNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNewNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientConnected() {
        joinLobbyButton.interactable = true;

        menuManager.NavigateToMenu("Lobby");
    }

    private void HandleClientDisconnected() {
        SetErrorMessage("Disconnected!", Color.red);

        joinLobbyButton.interactable = true;

        menuManager.NavigateToMenu("Join Lobby");

        networkManager.StopClient(); // NEEDED?
    }

    public void JoinLobby() {
        displayName = displayNameInputField.text;

        if (DisplayNameAccepted(displayName)) {
            SaveName(displayName);

            joinLobbyButton.interactable = false;
            networkManager.networkAddress = ipAdressInputField.text;
            networkManager.StartClient();

            SetErrorMessage("Connecting...", Color.yellow);
        } else {
            SetErrorMessage(("'" + displayName + "'" + " is not a valid name!"), Color.red);
        }
    }

    private void SetErrorMessage(string message, Color textColor) {
        errorMessage.enabled = true;
        errorMessage.color = textColor;
        errorMessage.text = message;
    }

    public void ClearErrorMessage() {
        errorMessage.enabled = false;
    }

    public void LeaveLobby() {
        joinLobbyButton.interactable = true;
        menuManager.NavigateToMenu("Play Menu");
        networkManager.StopClient();
    }

    void DisplayNameSetup() {
        if (PlayerPrefs.HasKey(PlayerPrefNameKey)) {
            displayName = PlayerPrefs.GetString(PlayerPrefNameKey);
            displayNameInputField.text = displayName;
        }
    }

    bool DisplayNameAccepted(string name) {
        if (string.IsNullOrEmpty(name))
            return false;

        return true;
    }

    void SaveName(string name) {
        PlayerPrefs.SetString(PlayerPrefNameKey, name);
        PlayerPrefs.Save();
    }
}
