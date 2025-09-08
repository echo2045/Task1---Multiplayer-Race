using UnityEngine;

public class StartLine : MonoBehaviour
{
    [SerializeField] private RaceManager raceManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Race started!");
            raceManager.StartRace();
        }
    }
}
