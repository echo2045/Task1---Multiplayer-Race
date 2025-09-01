using Photon.Pun;
using UnityEngine;

public class PlayerCarController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    private Rigidbody rb;
    private Camera playerCamera; // To hold the camera for this player

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on PlayerCarController!");
        }

        if (photonView.IsMine)
        {
            //  Local player: attach a camera
            playerCamera = new GameObject("PlayerCamera").AddComponent<Camera>();
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0, 3, -5); // Behind & above
            playerCamera.transform.localRotation = Quaternion.Euler(15, 0, 0);
            playerCamera.depth = 0;
        }
        else
        {
            //  Remote player: disable any cameras that might be on the prefab
            Camera remoteCam = GetComponentInChildren<Camera>();
            if (remoteCam != null)
            {
                remoteCam.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        //  Remote players skip input, but Rigidbody still syncs via Photon
        if (!photonView.IsMine) return;

        // Movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * vertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float turn = horizontal * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0, turn, 0);
        rb.MoveRotation(rb.rotation * turnRotation);
    }



    // Optional: If you need to send custom data, implement IPunObservable
    // For basic movement with PhotonRigidbodyView, this might not be strictly necessary
    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         // We own this player: send the others our data
    //         stream.SendNext(rb.position);
    //         stream.SendNext(rb.rotation);
    //         stream.SendNext(rb.velocity);
    //         stream.SendNext(rb.angularVelocity);
    //     }
    //     else
    //     {
    //         // Network player, receive data
    //         // These will be automatically applied by PhotonRigidbodyView if you use it
    //         // Otherwise, you'd interpolate here
    //     }
    // }
}