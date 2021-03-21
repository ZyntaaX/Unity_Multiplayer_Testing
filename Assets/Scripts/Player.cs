using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour {

    // Public Variables
    [Header("Required Components")]
    //dsdgfsdf

    [Header("Player Settings")]
    public float walkSpeed;
    public float runSpeed;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 1;

    /*
    [SyncVar(hook = "OnCountChange")]
    int count = 0;
    */

    float xRotation = 0f;

    void Start() {
        if (isLocalPlayer) {
            //Cursor.visible = false;
        }
    }

    void OnConnectedToServer() {
        if (isLocalPlayer)
            Cursor.visible = false;
    }

    void OnDisconnectedFromServer() {
        Cursor.visible = true;
    }

    void Update() {
        HandleMovement();

        /*
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X)) {
            IncreaseCount();
        }
        */
    }

    void HandleMovement() {
        if (isLocalPlayer) {
            float speed = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

            transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime);
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
        }
    }

    /*
    [Command]
    void IncreaseCount() {
        count++;
    }

    void OnCountChange(int oldCount, int newCount) {
        Debug.Log($"We had {oldCount}, but now we have {newCount}");
    }
    */

    /*
    [Command]
    [TargetRpc]
    [ClientRpc]
    */
}