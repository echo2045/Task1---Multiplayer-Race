using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerListUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text playerListText; // Assign in Inspector
    private bool showAtRaceEnd = false;

    void Start()
    {
        if (playerListText != null)
        {
            playerListText.gameObject.SetActive(false);
        }
    }

    public void ShowPlayerList()
    {
        showAtRaceEnd = true;
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        if (!showAtRaceEnd || playerListText == null) return;

        playerListText.gameObject.SetActive(true);

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            playerListText.text = "LEADERBOARD\n1. YOU";
            return;
        }

        string list = "LEADERBOARD\n";

        List<Player> sortedPlayers = PhotonNetwork.CurrentRoom.Players.Values
            .OrderBy(p => p.ActorNumber).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            Player player = sortedPlayers[i];
            string name = string.IsNullOrEmpty(player.NickName) ? "Player" : player.NickName;

            if (player.IsLocal) name += " (YOU)";
            list += (i + 1) + ". " + name + "\n";
        }

        playerListText.text = list;
    }
}
