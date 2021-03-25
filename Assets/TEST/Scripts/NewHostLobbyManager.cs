using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NewHostLobbyManager : MonoBehaviour {

    // Public
    [Header("Required Components")]
    [SerializeField] private InputField displayNameInputField = null;
    [SerializeField] private Button createLobbyButton = null;
    [SerializeField] private MenuManager menuManager = null;
    [SerializeField] private MyNewNetworkManager networkManager = null;
    [SerializeField] private Text ipAdressNotification = null;

    // Private
    private string displayName = null;

    void Start() {
        DisplayNameSetup();
        ipAdressNotification.enabled = false;
    }

    void Update() {
        if (string.IsNullOrEmpty(displayNameInputField.text)) {
            createLobbyButton.interactable = false;
        } else {
            createLobbyButton.interactable = true;
        }
    }

    public void HostLobby() {
        displayName = displayNameInputField.text;
        if (DisplayNameAccepted(displayName)) {
            SaveName(displayName);

            Debug.Log($"Hosting a lobby on IP: {networkManager.networkAddress}");

            menuManager.NavigateToMenu("Lobby");
            networkManager.StartHost();

            ipAdressNotification.enabled = true;
            ipAdressNotification.text = "IP - Adress: " + networkManager.networkAddress;
        }
    }

    public void CloseLobby() {
        networkManager.StopHost();

        menuManager.NavigateToMenu("Play Menu");
    }

    void DisplayNameSetup() {
        if (PlayerPrefs.HasKey(PlayerPrefStrings.playerDisplayNamePref)) {
            displayName = PlayerPrefs.GetString(PlayerPrefStrings.playerDisplayNamePref);
            displayNameInputField.text = displayName;
        }
    }

    bool DisplayNameAccepted(string name) {
        if (string.IsNullOrEmpty(name))
            return false;

        return true;
    }

    void SaveName(string name) {
        PlayerPrefs.SetString(PlayerPrefStrings.playerDisplayNamePref, name);
        PlayerPrefs.Save();
    }
}
