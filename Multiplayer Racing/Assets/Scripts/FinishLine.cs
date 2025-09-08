using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class FinishLine : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text winnerText;
    [SerializeField] private PlayerListUI playerListUI;

    private bool raceEnded = false;

    void Start()
    {
        if (winnerText != null)
        {
            winnerText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!raceEnded && other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();
            if (view != null && view.IsMine) // only local player reports their time
            {
                double finalTime = PhotonNetwork.Time - RaceManager.Instance.GetRaceStartTime();
                photonView.RPC("RPC_DeclareWinner", RpcTarget.AllBuffered, view.Owner.NickName, finalTime);
            }
        }
    }

    [PunRPC]
    private void RPC_DeclareWinner(string winnerName, double finalTime)
    {
        if (raceEnded) return;
        raceEnded = true;

        // Update UI
        if (winnerText != null)
        {
            winnerText.text = $"Winner: {winnerName} ({finalTime:F2} sec)";
            winnerText.gameObject.SetActive(true);
        }

        if (playerListUI != null)
        {
            playerListUI.ShowPlayerList();
        }

        // Save final time in Photon Custom Properties
        var props = new Hashtable { { "RaceTime", finalTime } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Optionally save to Firebase
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            FirebaseManager.Instance.WriteToRealtime($"Winner: {winnerName}, Time: {finalTime:F2}", "raceResults");
            FirebaseManager.Instance.WriteToFirestore($"Winner: {winnerName}, Time: {finalTime:F2}", "raceResults");
        }

        Debug.Log($"Winner saved: {winnerName}, Time: {finalTime:F2}");
    }
}
