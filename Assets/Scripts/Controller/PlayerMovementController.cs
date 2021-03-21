using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour {
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float sprintSpeed = 10f;
    [SerializeField] private CharacterController controller = null;

    private Vector2 previousInput;

    private float movementSpeed;

    private Controls controls;
    private Controls Controls {
        get {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public override void OnStartAuthority() {
        enabled = true;
        Cursor.visible = false;

        movementSpeed = walkSpeed;

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();

        Controls.Player.Sprint.performed += ctx => SetSprinting(ctx.ReadValue<float>());
        Controls.Player.Sprint.canceled += ctx => SetSprinting(ctx.ReadValue<float>());
    }

    [ClientCallback]
    private void SetSprinting(float value) {

        Debug.Log(value);
        movementSpeed = (value == 1) ? sprintSpeed : walkSpeed;
    }

    [ClientCallback]
    private void OnEnable() {
        Controls.Enable();
    }

    [ClientCallback]
    void OnDisable() {
        Controls.Disable();
    }

    [ClientCallback]
    private void Update() {
        Move();
    }

    [Client]
    private void SetMovement(Vector2 movement) {
        previousInput = movement;
    }

    [Client]
    private void ResetMovement() {
        previousInput = Vector2.zero;
    }

    [Client]
    private void Move() {
        Vector3 right = controller.transform.right;
        Vector3 forward = controller.transform.forward;
        right.y = 0f;
        forward.y = 0f;

        Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y;

        controller.Move(movement * movementSpeed * Time.deltaTime);
    }
}
