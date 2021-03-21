using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MainMenu : NetworkBehaviour {

    [Header("Required Compontents")]
    public GameObject mainMenuHolder;
    public GameObject playMenuHolder;
    public GameObject optionsMenuHolder;

    public void OnPlayButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        playMenuHolder.SetActive(true);
    }

    public void OnOptionsButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
        playMenuHolder.SetActive(false);
    }

    public void OnBackButtonClicked() {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
        playMenuHolder.SetActive(false);
    }

    public void OnCreateLobbyButtonClicked() {

    }

    public void OnJoinLobbyButtonClicked() {
        
    }

    public void Quit() {
        Application.Quit();
    }
}
