﻿using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Components")]
    private PlayerMovementComponent movComp;
    private Rigidbody rb;

    [Header("Defaults")]
    private Vector3 startPosition;
    private Quaternion startRotation;

    public delegate void PlayerDie();
    public static PlayerDie OnPlayerDie;

    private void Awake() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        movComp = GetComponent<PlayerMovementComponent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        // Movement.
        float hInput = Input.GetAxis("Horizontal");
        movComp.Move(hInput);

        // Jump.
        if (Input.GetButtonDown("Jump") && movComp.IsGrounded())
            movComp.Jump();
        if (Input.GetButton("Jump")) {
            if (!movComp.HoldingJump())
                movComp.SetHoldingJump(true);
        }
        else if (movComp.HoldingJump())
            movComp.SetHoldingJump(false);

        if (Input.GetKeyDown(KeyCode.Q))
            Respawn();
    }

    public void Respawn() {
        if (OnPlayerDie != null)
            OnPlayerDie();

        rb.velocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = startRotation;
        movComp.SetIsRight(true);
    }

    private void OnDrawGizmosSelected() {
        DebugExtension.DebugCapsule(transform.position, transform.position - new Vector3(0, 0.03f, 0), 0.05f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "YKillZone")
            GameMode.Instance().Restart();
        else if (other.gameObject.tag == "KillCollider") {
            BaseEnemy enemy = other.gameObject.transform.parent.GetComponent<BaseEnemy>();
            if (enemy != null) {
                Debug.Log("Enemy killed.");
                enemy.Die();
            }
            else Destroy(other.gameObject.transform.parent.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Enemy") {
            if (collision.gameObject.tag == "Bullet")
                Destroy(collision.gameObject);
            GameMode.Instance().Restart();
        }
    }
}
