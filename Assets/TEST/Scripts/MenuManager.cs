using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [System.Serializable]
    public struct MenuHolder {
        public string name;
        public GameObject holder;

        public MenuHolder(string _name, GameObject _holder) {
            name = _name;
            holder = _holder;
        }
    }

    // Public
    [Header("Menu Holders")]
    public MenuHolder[] menuHolders = null;

    // Private
    int currentActiveHolderIndex;

    // Start is called before the first frame update
    public void Awake() {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].holder.activeSelf) {
                currentActiveHolderIndex = i;
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlayButtonClicked() {
        NavigateToMenu("Play Menu");
    }

    public void OptionsButtonClicked() {
        NavigateToMenu("Options Menu");
    }

    public void CreateLobbyButtonClicked() {
        NavigateToMenu("Create Lobby");
    }

    public void JoinLobbyButtonClicked() {
        NavigateToMenu("Join Lobby");
    }

    public void Quit() {
        Application.Quit();
    }

    public void BackButtonClicked() {
        string currentMenu = menuHolders[currentActiveHolderIndex].name;
        if (currentMenu == "Create Lobby" || currentMenu == "Join Lobby") {
            NavigateToMenu("Play Menu");
        } else {
            NavigateToMenu("Landing Page");
        }
    }

    public void NavigateToMenu(string menuName) {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].name == menuName) {
                menuHolders[i].holder.SetActive(true);
                currentActiveHolderIndex = i;
            } else {
                menuHolders[i].holder.SetActive(false);
            }
        }
    }
}
