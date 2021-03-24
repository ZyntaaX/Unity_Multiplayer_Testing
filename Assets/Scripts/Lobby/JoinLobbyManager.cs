using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyManager : MonoBehaviour {
    // Public
    [Header("Required Components")]
    [SerializeField] private MyNetworkManager networkManager = null;
    [SerializeField] private MainMenu menuManager = null;

    [SerializeField] private InputField displayNameInputField = null;
    [SerializeField] private InputField ipAdressInputField = null;

    [SerializeField] private Button joinButton = null;

    [SerializeField] private Text messageText = null;

    // Private
    private string displayName = null;
    private string PlayerPrefNameKey = "PlayerPrefName";

    void Start() {  
        DisplayNameSetup();

        ipAdressInputField.text = "localhost"; //Development ONLY
    }

    void Update() {
        if (string.IsNullOrEmpty(displayNameInputField.text) || string.IsNullOrEmpty(ipAdressInputField.text))
            joinButton.interactable = false;
        else
            joinButton.interactable = true;
    }

    private void OnEnable() {
        messageText.enabled = false;

        MyNetworkManager.OnClientConnected += HandleClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable() {

        MyNetworkManager.OnClientConnected -= HandleClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientConnected() {
        Debug.Log("Connected!");

        joinButton.interactable = true;

        menuManager.GoToMenu("Join Game Lobby Menu");
    }

    private void HandleClientDisconnected() {
        Debug.Log("Disconnected!");

        joinButton.interactable = true;

        messageText.color = Color.red;
        messageText.text = "Disconnected!";

        menuManager.GoToMenu("Join Lobby Menu");

        networkManager.StopClient();
    }

    public void LeaveLobby() {
        joinButton.interactable = true;
        menuManager.GoToMenu("Join Lobby Menu");
        networkManager.StopClient();
    }

    public void JoinLobby() {
        displayName = displayNameInputField.text;
        if (DisplayNameAccepted(displayName)) {
            SaveName(displayName);
            joinButton.interactable = false;
            networkManager.networkAddress = ipAdressInputField.text;
            networkManager.StartClient();

            messageText.enabled = true;
            messageText.color = Color.yellow;
            messageText.text = "Connecting...";
        } else {
            messageText.enabled = true;
            messageText.color = Color.red;
            messageText.text = "'" + displayName + "'" +  " is not a valid displayname!";
        }
    }

    private void DisplayNameSetup() {
        if (PlayerPrefs.HasKey(PlayerPrefNameKey)) {
            displayName = PlayerPrefs.GetString(PlayerPrefNameKey);
            displayNameInputField.text = displayName;
        }
    }

    private bool DisplayNameAccepted(string name) {
        if (string.IsNullOrEmpty(name))
            return false;

        return true;
    }

    private void SaveName(string name) {
        PlayerPrefs.SetString(PlayerPrefNameKey, name);
        PlayerPrefs.Save();
    }
}
