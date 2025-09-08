using UnityEngine;
using UnityEngine.UI;
using TMPro; // if using TextMeshPro
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField; // Assign in Inspector
    [SerializeField] private Button confirmButton;          // Assign in Inspector

    private const string PlayerNamePrefKey = "PlayerName"; // Save between sessions

    void Start()
    {
        // Load last saved name if available
        string defaultName = PlayerPrefs.GetString(PlayerNamePrefKey, "");
        nameInputField.text = defaultName;

        // Add listener to confirm button
        confirmButton.onClick.AddListener(OnConfirmName);
    }

    private void OnConfirmName()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player"; // fallback
        }

        // Save locally
        PlayerPrefs.SetString(PlayerNamePrefKey, playerName);

        // Set Photon nickname
        PhotonNetwork.NickName = playerName;
        Debug.Log("Player name set to: " + PhotonNetwork.NickName);

        // Load the next scene (your game scene)
        SceneManager.LoadScene("SampleScene"); 
    }
}
