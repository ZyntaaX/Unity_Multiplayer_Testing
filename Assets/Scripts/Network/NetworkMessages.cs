using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkMessages : MonoBehaviour {
    [SerializeField] Text notificationsText = null;

    void Start() {
        if (!NetworkClient.active) { return; }

        NetworkClient.RegisterHandler<Notification>(OnNotificationRecieved);
    }

    private void OnNotificationRecieved(Notification msg) {
        notificationsText.text += $"\n{msg.content}";
    }
}

public struct Notification : NetworkMessage {
    public string content;
}
