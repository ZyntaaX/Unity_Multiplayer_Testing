using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour {

    // Public Variables
    [Header("Player Settings")]
    public float walkSpeed;
    public float runSpeed;

    void Update() {
        HandleMovement();
    }

    void HandleMovement() {
        if (isLocalPlayer) {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float speed = (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

            Vector3 movement = new Vector3(moveHorizontal * Time.deltaTime * speed, moveVertical * Time.deltaTime * speed, 0f);
            transform.position = transform.position + movement;
        }
    }
}