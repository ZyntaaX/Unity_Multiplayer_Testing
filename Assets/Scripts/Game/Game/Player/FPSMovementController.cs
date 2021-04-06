using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSMovementController : NetworkBehaviour {
    // Public
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private Camera camera = null;
    [SerializeField] private Transform cameraOriginalPosition = null;
    [SerializeField] private Transform cameraSprintPosition = null;


    [Header("Player Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float backingSpeed = 4f;
    [SerializeField] private float jumpHeight = 3f;

    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float distanceToGroundCheck = .2f;
    [SerializeField] private LayerMask groundMask;

    // Private
    private float movementSpeed;
    private Vector3 velocity;
    private bool isGrounded;
    private float x = 0f;
    private float z = 0f;
    private bool isRunning = false;
    private bool isWalking = false;
    private bool isWalkingBackwards = false;
    private bool isStrafingRight = false;
    private bool isStrafingLeft = false;

    public override void OnStartAuthority() {
        movementSpeed = walkSpeed;

        enabled = true;
    }

    // Update is called once per frame
    private void LateUpdate() {
        CheckInputs();
        Move();
        CheckGravity();
        SetMovementSpeed();

        SetAnimationBooleans();
        SetAnimatorStates();

        if (isRunning || isStrafingLeft || isStrafingRight) {
            camera.transform.position = cameraSprintPosition.position;
        } else {
            camera.transform.position = cameraOriginalPosition.position;
        }
    }

    private void CheckInputs() {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isWalkingBackwards) {
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            isRunning = false;
        }
    }

    private void SetMovementSpeed() {
        if (isRunning) {
            movementSpeed = runSpeed;
        } else if (isWalkingBackwards) {
            movementSpeed = backingSpeed;
        } else {
            movementSpeed = walkSpeed;
        }
    }


    private void SetAnimationBooleans() {
        ResetAnimationBooleans();

        if (x > 0) {
            isStrafingRight = true;
        } else if (x < 0) {
            isStrafingLeft = true;
        }

        if (z > 0) {
            isWalking = true;
        } else if (z < 0) {
            isWalkingBackwards = true;
        }
    }

    private void ResetAnimationBooleans() {
        //isRunning = false;
        isWalking = false;
        isWalkingBackwards = false;
        isStrafingRight = false;
        isStrafingLeft = false;
    }

    private void SetAnimatorStates() {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isWalkingBackwards", isWalkingBackwards);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isStrafingLeft", isStrafingLeft);
        animator.SetBool("isStrafingRight", isStrafingRight);

        animator.SetBool("isJumping", !isGrounded);
    }

    private void Move() {
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * movementSpeed * Time.deltaTime);
    }

    private void CheckGravity() {
        isGrounded = Physics.CheckSphere(groundCheck.position, distanceToGroundCheck, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -5f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
