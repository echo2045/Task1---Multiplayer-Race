using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text winnerText;      // Assign in Inspector
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
            string winnerName = other.GetComponent<PhotonView>().Owner.NickName;
            if (string.IsNullOrEmpty(winnerName)) winnerName = "Player";

            // Call RPC to synchronize raceEnded and winner across all clients
            photonView.RPC("RPC_DeclareWinner", RpcTarget.AllBuffered, winnerName);
        }
    }

    [PunRPC]
    private void RPC_DeclareWinner(string winnerName)
    {
        if (raceEnded) return;

        raceEnded = true;

        if (winnerText != null)
        {
            winnerText.text = "Winner: " + winnerName;
            winnerText.gameObject.SetActive(true);
        }

        if (playerListUI != null)
        {
            playerListUI.ShowPlayerList();
        }
    }
}
