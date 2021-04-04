using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour {

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Animator animator = null;

    private Vector2 previousInput;
    private float movementSpeed;
    private bool isRunning = false;

    private Controls controls;
    private Controls Controls {
        get {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public override void OnStartAuthority() {
        enabled = true;

        Cursor.visible = false; //RIGHT PLACE?

        movementSpeed = walkSpeed;

        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
        Controls.Player.Sprint.performed += ctx => SetMovementSpeed(ctx.ReadValue<float>());
        Controls.Player.Sprint.canceled += ctx => SetMovementSpeed(ctx.ReadValue<float>());
    }

    private void SetMovementSpeed(float value) {
        isRunning = value == 1;

        animator.SetBool("isRunning", value == 1);
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
        movementSpeed = isRunning ? runSpeed : walkSpeed;
        SetAnimationState();
        Move();
    }

    [Client]
    private void SetMovement(Vector2 movement) {
        previousInput = movement;
    }

    [Client]
    private void ResetAnimationState() {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isStrafeWalkingRight", false);
        animator.SetBool("isStrafeWalkingLeft", false);
        animator.SetBool("isStrafeRunningRight", false);
        animator.SetBool("isStrafeRunningLeft", false);
    }

    [Client]
    private void SetAnimationState() {
        ResetAnimationState();

        if (previousInput.y > 0 || previousInput.y < 0) {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", isRunning);
        } 

        if (previousInput.x > 0) {
            if (isRunning) {
                animator.SetBool("isStrafeRunningRight", true);
            } else {
                animator.SetBool("isStrafeWalkingRight", true);
            }
        }

        if (previousInput.x < 0) {
            if (isRunning) {
                animator.SetBool("isStrafeRunningLeft", true);
            } else {
                animator.SetBool("isStrafeWalkingLeft", true);
            }
        }
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