using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkMessages : MonoBehaviour {
    [SerializeField] Text notificationsText = null;

    void Start() {
        if (!NetworkClient.active) { return; }

        NetworkClient.RegisterHandler<Notification>(OnNotificationRecieved);
    }

    public void ClearNotification() {
        notificationsText.text = string.Empty;
    }

    private void OnNotificationRecieved(Notification msg) {
        notificationsText.text = $"\n{msg.content}";
    }
}

public struct Notification : NetworkMessage {
    public string content;
}
