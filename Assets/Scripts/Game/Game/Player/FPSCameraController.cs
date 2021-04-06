using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSCameraController : NetworkBehaviour {
    // Public
    [Header("References")]
    [SerializeField] private Camera camera = null;
    [SerializeField] private Transform playerTransform = null;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 100f;

    // Private
    private float xRotation = 0f;
    private bool menuUp = true; // Starts as true as it will instantly change upon start
    public bool MenuUp {
        set {
            menuUp = value;
        }
        get {
            return menuUp;
        }
    }

    public override void OnStartAuthority() {
        camera.gameObject.SetActive(true);

        enabled = true;

        DisplayMenu();
    }

    private void DisplayMenu() {
        MenuUp = !MenuUp;

        if (MenuUp) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update() {
        if (!MenuUp) {
            UpdateCameraRotation();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            DisplayMenu();
        }
    }

    private void UpdateCameraRotation() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 50f);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
