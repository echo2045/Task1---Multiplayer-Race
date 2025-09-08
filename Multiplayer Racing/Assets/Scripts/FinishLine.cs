using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class FinishLine : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text winnerText;       // UI element to show winner
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
            if (view != null && view.IsMine) // only local player reports their own finish
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

        // Save final time in Photon Custom Properties (for other players to see)
        var props = new Hashtable { { "RaceTime", finalTime } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Build race result data object
        string roomId = PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "UnknownRoom";
        RaceResultData result = new RaceResultData(winnerName, finalTime, roomId);

        // Save race result to Firebase (both DBs)
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            FirebaseManager.Instance.SaveRaceResultToRealtime("raceResults", result.ToDict());
            FirebaseManager.Instance.SaveRaceResultToFirestore("raceResults", result.ToDict());
            Debug.Log($"Winner saved to Firebase: {winnerName}, Time: {finalTime:F2}");
        }
        else
        {
            Debug.LogWarning("Firebase not ready, race result not saved.");
        }
    }
}
