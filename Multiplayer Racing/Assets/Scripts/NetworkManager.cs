using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerCarPrefab;
    public Transform[] spawnPoints; // Assign these in the Inspector

    void Start()
    {
        // Connect to Photon Cloud
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        PhotonNetwork.JoinOrCreateRoom("RaceRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        // Assign nickname based on ActorNumber
        PhotonNetwork.NickName = "Player " + PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("Joined Room! Nickname: " + PhotonNetwork.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am the Master Client!");
            // Master Client can handle game start logic here
        }

        SpawnPlayerCar();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene on disconnect
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entered the room!");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Two players connected! Starting game...");
            // Optionally, start a countdown here
        }
    }

    private void SpawnPlayerCar()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber starts from 1

        if (playerIndex >= 0 && playerIndex < spawnPoints.Length)
        {
            PhotonNetwork.Instantiate(
                playerCarPrefab.name,
                spawnPoints[playerIndex].position,
                spawnPoints[playerIndex].rotation
            );
        }
        else
        {
            Debug.LogError("Not enough spawn points for player: " + playerIndex);
            // Fallback to a default spawn point
            PhotonNetwork.Instantiate(playerCarPrefab.name, Vector3.zero, Quaternion.identity);
        }
    }
}
