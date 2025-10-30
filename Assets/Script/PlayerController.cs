using System;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public InputActionReference moveAction; // Vector2 (WASD)
    public InputActionReference sprintAction;
    Vector2 moveInput = Vector2.zero;

    [Header("Camera")]
    public Transform cameraTransform;
    public Transform cameraPivot;
    public float sensitivity = 2f;
    public InputActionReference lookAction;
    float xRotation = 0f;
    float yRotation = 0f;

    [Header("Animation")]
    public Animator animator;

    private NetworkVariable<Vector3> networkPosition =
        new NetworkVariable<Vector3>(default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    Rigidbody rb;
    private float lerpSpeed = 12f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;

        if (IsOwner)
        {
            networkPosition.Value = rb.position;
            moveAction.action.Enable(); // just enable once here
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            if (cameraTransform != null)
                cameraTransform.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (!IsOwner) return;

        CameraMovement();
        moveInput = moveAction.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, lerpSpeed);
            return;
        }

        float pace = 1f;
        if (sprintAction.action.IsPressed())
            pace = 2f;

        //Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * pace;

        Vector3 move = ZeroOutY(cameraTransform.forward) * moveInput.y + ZeroOutY(cameraTransform.right) * moveInput.x;
        move.Normalize();

        rb.linearVelocity = move * moveSpeed * pace + Vector3.up * rb.linearVelocity.y;

        networkPosition.Value = rb.position;
        animator.SetFloat("Move", rb.linearVelocity.magnitude);
    }

    Vector3 ZeroOutY(Vector3 value)
    {
        value.y = 0f;
        return value.normalized;
    }

    void CameraMovement()
    {
        Vector2 cameraDirection = lookAction.action.ReadValue<Vector2>();
        float mouseX = cameraDirection.x * sensitivity;
        float mouseY = cameraDirection.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        yRotation += mouseX;

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraPivot.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}

