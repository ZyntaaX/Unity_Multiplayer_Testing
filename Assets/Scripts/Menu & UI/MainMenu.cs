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
    public GameObject createLobbyMenuHolder;
    public GameObject joinLobbyMenuHolder;

    public void OnPlayButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        createLobbyMenuHolder.SetActive(false);
        joinLobbyMenuHolder.SetActive(false);
        playMenuHolder.SetActive(true);
    }

    public void OnOptionsButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
        createLobbyMenuHolder.SetActive(false);
        joinLobbyMenuHolder.SetActive(false);
        playMenuHolder.SetActive(false);
    }

    public void OnBackButtonClicked() {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
        playMenuHolder.SetActive(false);
        createLobbyMenuHolder.SetActive(false);
        joinLobbyMenuHolder.SetActive(false);
    }

    public void OnCreateLobbyButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        playMenuHolder.SetActive(false);
        createLobbyMenuHolder.SetActive(true);
        joinLobbyMenuHolder.SetActive(false);
    }

    public void OnJoinLobbyButtonClicked() {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(false);
        playMenuHolder.SetActive(false);
        createLobbyMenuHolder.SetActive(false);
        joinLobbyMenuHolder.SetActive(true);
    }

    public void Quit() {
        Application.Quit();
    }
}
