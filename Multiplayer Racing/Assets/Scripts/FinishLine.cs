using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviourPun
{
    public Canvas canvas; // Optional: assign in Inspector
    private Text winnerDisplayText;
    private PlayerListUI playerListUI;
    private bool raceFinished = false;

    void Awake()
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in the scene!");
                return;
            }
        }

        // Create Winner Text
        GameObject winnerObj = new GameObject("WinnerText");
        winnerObj.transform.SetParent(canvas.transform);
        winnerDisplayText = winnerObj.AddComponent<Text>();

        winnerDisplayText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        winnerDisplayText.fontSize = 70;
        winnerDisplayText.alignment = TextAnchor.MiddleCenter;
        winnerDisplayText.color = Color.yellow;
        winnerDisplayText.horizontalOverflow = HorizontalWrapMode.Overflow;
        winnerDisplayText.verticalOverflow = VerticalWrapMode.Overflow;

        // Position: Middle of top third
        winnerDisplayText.rectTransform.anchorMin = new Vector2(0.5f, 0.66f);
        winnerDisplayText.rectTransform.anchorMax = new Vector2(0.5f, 0.66f);
        winnerDisplayText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        winnerDisplayText.rectTransform.anchoredPosition = Vector2.zero;
        winnerDisplayText.rectTransform.sizeDelta = new Vector2(1000, 200);
        winnerDisplayText.gameObject.SetActive(false);

        // Find PlayerListUI on Canvas
        playerListUI = canvas.GetComponent<PlayerListUI>();
        if (playerListUI == null)
        {
            Debug.LogError("PlayerListUI script not found on the Canvas!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (raceFinished) return;

        PlayerCarController car = other.GetComponent<PlayerCarController>();
        if (car != null && car.photonView.IsMine)
        {
            string winnerName = PhotonNetwork.LocalPlayer != null
                ? PhotonNetwork.LocalPlayer.NickName
                : "YOU";

            photonView.RPC("DeclareWinnerRPC", RpcTarget.All, winnerName);
            raceFinished = true;
        }
    }

    [PunRPC]
    void DeclareWinnerRPC(string winnerName)
    {
        if (winnerDisplayText != null)
        {
            winnerDisplayText.text = winnerName.ToUpper() + " WINS!";
            winnerDisplayText.gameObject.SetActive(true);
        }

        if (playerListUI != null)
        {
            playerListUI.SetRaceEnded();
        }
    }
}
