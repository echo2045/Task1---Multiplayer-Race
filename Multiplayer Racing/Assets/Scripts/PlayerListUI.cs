using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerListUI : MonoBehaviourPunCallbacks
{
    private Text playerListText;
    private bool showAtRaceEnd = false;

    void Awake()
    {
        // Create Player List Text
        GameObject listObj = new GameObject("PlayerListText");
        listObj.transform.SetParent(transform);
        playerListText = listObj.AddComponent<Text>();

        playerListText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        playerListText.fontSize = 40;
        playerListText.alignment = TextAnchor.UpperCenter;
        playerListText.color = Color.cyan;
        playerListText.horizontalOverflow = HorizontalWrapMode.Overflow;
        playerListText.verticalOverflow = VerticalWrapMode.Overflow;

        // Position: lower third of top third
        playerListText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f + 0.33f * 0.33f); // 0.5 + 1/9 ? 0.611
        playerListText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f + 0.33f * 0.33f);
        playerListText.rectTransform.pivot = new Vector2(0.5f, 1f);
        playerListText.rectTransform.anchoredPosition = Vector2.zero;
        playerListText.rectTransform.sizeDelta = new Vector2(1000, 300);
        playerListText.gameObject.SetActive(false);
    }

    public void SetRaceEnded()
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
