using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour {

    [SerializeField] private Animator animator = null;

    private LobbyNetworkManager lobby;
    private LobbyNetworkManager Lobby {
        get {
            if (lobby != null) { return lobby; }
            return lobby = NetworkManager.singleton as LobbyNetworkManager;
        }
    }

    public void CountdownEnded() {
        animator.enabled = false;
    }

    /* - - - - - - - - - - - - - - SERVER - - - - - - - - - - - - - - - */
    public override void OnStartServer() {
        LobbyNetworkManager.OnServerStopped += CleanUpServer;
        LobbyNetworkManager.OnServerReadied += CheckToStartRound;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        CleanUpServer();
    }

    [Server]
    private void CleanUpServer() {
        LobbyNetworkManager.OnServerStopped -= CleanUpServer;
        LobbyNetworkManager.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]
    public void StartRound() {
        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn) {
        if (Lobby.GamePlayers.Count(x => x.connectionToClient.isReady) != Lobby.GamePlayers.Count) { return; }

        animator.enabled = true;

        RpcStartCountdown();
    }
    
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    /* - - - - - - - - - - - - - - CLIENT - - - - - - - - - - - - - - - */

    [ClientRpc]
    private void RpcStartCountdown() {
        animator.enabled = true;
    }
    
    [ClientRpc]
    private void RpcStartRound() {
        Debug.Log("Start");
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
}
