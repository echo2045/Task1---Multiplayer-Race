using System;
using System.Collections.Generic;

[Serializable]
public class RaceResultData
{
    public string winnerName;
    public double finishTime;   // in seconds
    public string roomId;       // optional: Photon room ID
    public DateTime timestamp;  // when race finished

    public RaceResultData(string winnerName, double finishTime, string roomId)
    {
        this.winnerName = winnerName;
        this.finishTime = finishTime;
        this.roomId = roomId;
        this.timestamp = DateTime.UtcNow;
    }

    // Convert to Firestore / Realtime format
    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            { nameof(winnerName), winnerName },
            { nameof(finishTime), finishTime },
            { nameof(roomId), roomId },
            { nameof(timestamp), timestamp.ToString("o") } // ISO8601
        };
    }
}
