using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyPlayerPrefab : NetworkBehaviour
{
    // Public
    [Header("Required Compontents")]
    [SerializeField] private Text[] playerDisplayNameTexts;
    [SerializeField] private Text[] playerReadytexts;
    [SerializeField] private Text[] waitingForPlayerTexts;
    [SerializeField] private Button readybutton = null;
    [SerializeField] private Button startGameButton = null;
}
