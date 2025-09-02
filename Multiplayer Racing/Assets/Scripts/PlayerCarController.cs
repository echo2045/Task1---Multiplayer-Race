using Photon.Pun;
using UnityEngine;

public class PlayerCarController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    public Camera playerCamera; // Reference to the prefab camera

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on PlayerCarController!");
        }

     
        Transform cameraHolder = transform.Find("CameraHolder");
        if (cameraHolder != null)
        {
            playerCamera = cameraHolder.GetComponent<Camera>();
        }


        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera not found! Assign a Camera on prefab or a child named 'CameraHolder'.");
        }

        // Enable camera only for local player
        if (photonView.IsMine)
        {
            if (playerCamera != null) playerCamera.enabled = true;
        }
        else
        {
            if (playerCamera != null) playerCamera.enabled = false;
        }
    }

    void FixedUpdate()
    {
        // Only local player can move
        if (!photonView.IsMine) return;

        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move
        Vector3 movement = transform.forward * vertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Turn
        float turn = horizontal * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    // Optional: for Photon Rigidbody sync
    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { ... }
}
