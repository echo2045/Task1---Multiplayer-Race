using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Threading.Tasks;
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
                Debug.Log("Firebase ready! (Realtime DB + Firestore)");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + dependencyStatus);
            }
        });
    }





    /*
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

    // ------------------------------
    // Save race result to Firestore
    // ------------------------------
    public void SaveRaceResultToFirestore(string collection, Dictionary<string, object> data)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        FirestoreDB.Collection(collection).AddAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Race result saved in Firestore.");
            else
                Debug.LogError("Failed to save result in Firestore: " + task.Exception);
        });
    }

    // ------------------------------
    // Save race result to Realtime Database
    // ------------------------------
    public void SaveRaceResultToRealtime(string parentPath, Dictionary<string, object> data)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        DatabaseReference refPath = DBreference.Child(parentPath).Push();
        refPath.SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
                Debug.Log("Race result saved in Realtime DB.");
            else
                Debug.LogError("Failed to save result in Realtime DB: " + task.Exception);
        });
    }

    */


    // ------------------------------
    // Write to Realtime Database (custom path)
    // ------------------------------
    public async Task WriteToRealtimeAsync(string childPath, string message)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        string key = DBreference.Child(childPath).Push().Key;

        try
        {
            await DBreference.Child(childPath).Child(key).SetValueAsync(message);
            Debug.Log($"Wrote to Realtime DB ({childPath}): {message}");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to Realtime DB: " + e);
        }
    }

    // ------------------------------
    // Write to Firestore (custom collection)
    // ------------------------------
    public async Task WriteToFirestoreAsync(string collectionName, string message)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        DocumentReference docRef = FirestoreDB.Collection(collectionName).Document();
        var messageData = new { text = message, timestamp = Timestamp.GetCurrentTimestamp() };

        try
        {
            await docRef.SetAsync(messageData);
            Debug.Log($"Wrote to Firestore ({collectionName}): {message}");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to Firestore: " + e);
        }
    }

    // ------------------------------
    // Save race result to Firestore
    // ------------------------------
    public async Task SaveRaceResultToFirestoreAsync(string collection, Dictionary<string, object> data)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        try
        {
            await FirestoreDB.Collection(collection).AddAsync(data);
            Debug.Log("Race result saved in Firestore.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save result in Firestore: " + e);
        }
    }

    // ------------------------------
    // Save race result to Realtime Database
    // ------------------------------
    public async Task SaveRaceResultToRealtimeAsync(string parentPath, Dictionary<string, object> data)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Firebase not ready yet!");
            return;
        }

        DatabaseReference refPath = DBreference.Child(parentPath).Push();

        try
        {
            await refPath.SetValueAsync(data);
            Debug.Log("Race result saved in Realtime DB.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save result in Realtime DB: " + e);
        }
    }


}
