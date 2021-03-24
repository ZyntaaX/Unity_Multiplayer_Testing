using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HostLobbyManager : MonoBehaviour {

    // Public
    [Header("Required Components")]
    [SerializeField] private MyNetworkManager networkManager = null;
    [SerializeField] private MainMenu menuManager = null;

    [SerializeField] private InputField displayNameInputField = null;
    [SerializeField] private Button createButton = null;

    // Private
    private string displayName = null;
    private string PlayerPrefNameKey = "PlayerPrefName";

    void Start() {
        DisplayNameSetup();
    }

    void Update()
    {
        if (string.IsNullOrEmpty(displayNameInputField.text))
            createButton.interactable = false;
        else 
            createButton.interactable = true;
    }

    public void HostLobby() {
        displayName = displayNameInputField.text;
        if (DisplayNameAccepted(displayName)) {
            SaveName(displayName);

            menuManager.GoToMenu("Host Game Lobby Menu");

            Debug.Log($"Created Lobby on IP {networkManager.networkAddress}");
            networkManager.StartHost();
        }
    }

    public void CloseLobby() {
        Debug.Log("Closing Lobby");

        menuManager.GoToMenu("Play Menu");

        networkManager.StopHost();
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
