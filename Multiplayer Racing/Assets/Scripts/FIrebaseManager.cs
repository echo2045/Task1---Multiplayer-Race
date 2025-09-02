using UnityEngine;
using Firebase;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private FirebaseApp app;
    public DatabaseReference DBreference { get; private set; }

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                // Your database URL
                DBreference = FirebaseDatabase.GetInstance(app,
                    "https://multiplayer-race-1115a-default-rtdb.asia-southeast1.firebasedatabase.app/")
                    .RootReference;

                IsReady = true;
                Debug.Log("Firebase is ready!");
            }
            else
            {
                Debug.LogError(" Could not resolve Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void WriteTestData(string message)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        string key = DBreference.Child("messages").Push().Key;
        DBreference.Child("messages").Child(key).SetValueAsync(message);
        Debug.Log("Wrote to Firebase: " + message);
    }
}
