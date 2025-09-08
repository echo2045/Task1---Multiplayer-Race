using Photon.Pun;
using UnityEngine;

public class RaceManager : MonoBehaviourPunCallbacks
{
    public static RaceManager Instance { get; private set; }

    private double raceStartTime;
    private bool raceStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    // MasterClient starts the race
    public void StartRace()
    {
        if (PhotonNetwork.IsMasterClient && !raceStarted)
        {
            raceStarted = true;
            raceStartTime = PhotonNetwork.Time; // sync’d timestamp
            photonView.RPC("RPC_StartRace", RpcTarget.AllBuffered, raceStartTime);
        }
    }

    [PunRPC]
    private void RPC_StartRace(double startTime)
    {
        raceStartTime = startTime;
        raceStarted = true;
        Debug.Log("Race started at: " + startTime);
    }

    public double GetRaceStartTime()
    {
        return raceStartTime;
    }

    public bool IsRaceStarted()
    {
        return raceStarted;
    }
}
