using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlller : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public InputActionReference moveAction; // Vector2 (WASD)

    [Header("Camera")]
    public Transform cameraTransform;
    public float sensitivity = 2f;
    float xRotation = 0f;
    float yRotation = 0f;
    public InputActionReference lookAction;

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

        Camera();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, lerpSpeed);
            return;
        }

            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
            networkPosition.Value = rb.position;
    }

    void Camera()
    {
        Vector2 cameraDirection = lookAction.action.ReadValue<Vector2>();
        float mouseX = cameraDirection.x * sensitivity;
        float mouseY = cameraDirection.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        yRotation += mouseX;

        cameraTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }
}

