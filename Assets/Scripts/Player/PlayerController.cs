﻿using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 0.04f;
    private bool isRight = true;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    private bool jumpPressed = false;

    [Header("Raycast")]
    [SerializeField] private LayerMask levelMask;
    [SerializeField] private float raycastThreshold = 0.03f;

    [Header("Defaults")]
    private Rigidbody rb;
    private enum RotateType { FLIP, CORNER }
    private const float ROTATE_FROM_RIGHT = -90f;
    private const float ROTATE_FLIP = 180F;
    private const float RAYCAST_LENGTH = 5f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleCorners();
        HandleMovement();

        if (Input.GetButtonDown("Jump") && !jumpPressed)
            jumpPressed = true;
    }

    private void FixedUpdate() {
        if (jumpPressed) {
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
            jumpPressed = false;
        }

        Debug.Log(rb.velocity);
        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1);
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1);
    }

    private void HandleMovement() {
        float hInput = Input.GetAxis("Horizontal");
        if (hInput < 0 && isRight || hInput > 0 && !isRight)
            RotatePlayer(ROTATE_FLIP);
        // Since we're rotating the character when he flips (instead of applying a negative scale) we're only interested in the absolute value of the input.
        Vector3 horizontalMovement = transform.right * Mathf.Abs(hInput);
        transform.position += horizontalMovement * movementSpeed * Time.deltaTime;
    }

    // Raycasts from a threshold to detect the end of the level.
    // It it doesn't detect the level, it rotates the gameobject accordingly.
    private void HandleCorners() {
        Debug.DrawLine(transform.position + transform.right * raycastThreshold, transform.position + transform.right * raycastThreshold - Vector3.up * RAYCAST_LENGTH, Color.red);
        if (!Physics.Raycast(transform.position + transform.right * raycastThreshold, -Vector3.up, RAYCAST_LENGTH, levelMask))
            RotatePlayer(isRight ? ROTATE_FROM_RIGHT : -ROTATE_FROM_RIGHT);
    }

    // Rotates this gameobject by a 'rotateAmount'.
    // If the amount if equal to the flip amount, it sets the corresponding variable.
    private void RotatePlayer(float rotateAmount) {
        if (rotateAmount == ROTATE_FLIP)
            isRight = !isRight;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotateAmount);
    }

    private void HandleJump() {
        if (jumpPressed) {
            rb.AddForce(0, 0, 5, ForceMode.Impulse);
            jumpPressed = false;
        }

        if (rb.velocity.z < 0)
            rb.velocity += Vector3.up * Physics.gravity.z * (fallMultiplier - 1);
    }
}
