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

    private NetworkVariable<Vector3> networkPosition =
        new NetworkVariable<Vector3>(default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    Rigidbody rb;
    private float lerpSpeed = 12f;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        if (IsOwner)
        {
            networkPosition.Value = rb.position;
            moveAction.action.Enable(); // just enable once here
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            cameraTransform.gameObject.SetActive(false);
            enabled = false;
        }

    }

    void Update()
    {
        Camera();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
        {
            Vector3 target = networkPosition.Value;
            transform.position = Vector3.Lerp(transform.position, target, lerpSpeed);
            return;
        }

        else
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed;
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
            networkPosition.Value = rb.position;
        }
    }

    void Camera()
    {
        if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

