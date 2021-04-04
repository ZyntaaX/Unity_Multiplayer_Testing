using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour {

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

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

        movementSpeed = walkSpeed;

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
        Controls.Player.Sprint.performed += ctx => SetMovementSpeed(ctx.ReadValue<float>());
        Controls.Player.Sprint.canceled += ctx => SetMovementSpeed(ctx.ReadValue<float>());
    }

    private void SetMovementSpeed(float value) {
        movementSpeed = value == 1 ? runSpeed : walkSpeed;
    }

    [ClientCallback]
    private void OnEnable() {
        Controls.Enable();
    }

    [ClientCallback]
    private void OnDisable() {
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

    private void Move() {
        Vector3 right = controller.transform.right;
        Vector3 forward = controller.transform.forward;
        right.y = 0f;
        forward.y = 0f;

        Vector3 movement = right.normalized * previousInput.x + forward.normalized * previousInput.y;
        controller.Move(movement * movementSpeed * Time.deltaTime);
    }
}
