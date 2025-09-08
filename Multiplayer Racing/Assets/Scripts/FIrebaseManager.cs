using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    private FirebaseApp app;

    // --- Realtime Database ---
    public DatabaseReference DBreference { get; private set; }

    // --- Firestore ---
    public FirebaseFirestore FirestoreDB { get; private set; }

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        // Singleton pattern (only one FirebaseManager exists across scenes)
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
        // Check and fix Firebase dependencies (important for mobile & editor setup)
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // Get default Firebase app
                app = FirebaseApp.DefaultInstance;

                // --- Realtime Database reference ---
                DBreference = FirebaseDatabase.GetInstance(app,
                    "https://multiplayer-race-1115a-default-rtdb.asia-southeast1.firebasedatabase.app/")
                    .RootReference;

                // --- Firestore reference ---
                FirestoreDB = FirebaseFirestore.DefaultInstance;

                IsReady = true;
                Debug.Log("Firebase ready! (Realtime Database + Firestore)");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // ------------------------------
    // Write to Realtime Database (custom path)
    // ------------------------------
    public void WriteToRealtime(string childPath, string message)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        // Generate a unique key under the chosen path
        string key = DBreference.Child(childPath).Push().Key;

        // Save the message under that path
        DBreference.Child(childPath).Child(key).SetValueAsync(message);
        Debug.Log($"Wrote to Realtime DB ({childPath}): {message}");
    }

    // ------------------------------
    // Write to Firestore (custom collection)
    // ------------------------------
    public void WriteToFirestore(string collectionName, string message)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        // Create a new document with auto-generated ID inside the chosen collection
        DocumentReference docRef = FirestoreDB.Collection(collectionName).Document();

        // Create a data object (with timestamp)
        var messageData = new { text = message, timestamp = Timestamp.GetCurrentTimestamp() };

        // Save data to Firestore
        docRef.SetAsync(messageData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log($"Wrote to Firestore ({collectionName}): {message}");
            else
                Debug.LogError("Failed to write to Firestore: " + task.Exception);
        });
    }
}
