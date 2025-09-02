using UnityEngine;
using System.Collections;

public class FirebaseExample : MonoBehaviour
{
    private IEnumerator Start()
    {
        //  Wait until FirebaseManager is initialized & ready
        while (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady)
        {
            Debug.Log(" Waiting for Firebase...");
            yield return null;
        }

        Debug.Log(" Firebase is ready in FirebaseExample!");

        // Example write
        FirebaseManager.Instance.WriteTestData("Hello from FirebaseExample!");
    }
}
