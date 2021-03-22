using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenu : NetworkBehaviour {

    // Public
    [Header("Required Compontents")]
    public MenuHolder[] menuHolders;

    // Private
    int currentActiveMenuIndex;

    public void Awake() {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].menuHolder.activeSelf) {
                currentActiveMenuIndex = i;
            }
        }
    }

    public void OnPlayButtonClicked() {
        GoToMenu("Play Menu");
    }

    public void OnCreateLobbyButtonClicked() {
        GoToMenu("Create Lobby Menu");
    }

    public void OnJoinLobbyButtonClicked() {
        GoToMenu("Join Lobby Menu");
    }

    public void OnOptionsButtonClicked() {
        GoToMenu("Options Menu");
    }

    public void OnBackButtonClicked() {
        string currentMenu = menuHolders[currentActiveMenuIndex].name;
        if (currentMenu == "Join Lobby Menu" || currentMenu == "Create Lobby Menu" || currentMenu == "Game Lobby Menu") {
            GoToMenu("Play Menu");
        } else {
            GoToMenu("Main Menu");
        }
    }

    public void OnCreateGame() {
        GoToMenu("Game Lobby Menu");
    }

    public void OnJoinGame() {
        GoToMenu("Game Lobby Menu");
    }

    public void Quit() {
        Application.Quit();
    }

    void GoToMenu(string name) {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].name == name) {
                menuHolders[i].menuHolder.SetActive(true);
                currentActiveMenuIndex = i;
            } else {
                menuHolders[i].menuHolder.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public struct MenuHolder {
        public string name;
        public GameObject menuHolder;

        public MenuHolder(string _name, GameObject _menuHolder) {
            name = _name;
            menuHolder = _menuHolder;
        }
    }
}
