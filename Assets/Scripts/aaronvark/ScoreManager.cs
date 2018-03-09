using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void DataChanged();

/// <summary>
/// Server-only manager that handles scoring requests from clients
/// </summary>
public class ScoreManager : MonoBehaviour {
    public static DataChanged scoreChanged;

    static ushort[] clientScores = new ushort[Constants.MaxClients];

    //for host
    public static void HandleScore( ushort sender, ushort target ) {
        clientScores[sender]++;
        Notify();
    }

    public static ushort[] GetScores() {
        return clientScores;
    }

    public static void SetScores( ushort[] incomingClientScores ) {
        for (int i = 0; i < Constants.MaxClients; ++i)
        {
            clientScores[i] = incomingClientScores[i];
        }
        Notify();
    }

    public static void Reset() {
        clientScores = new ushort[Constants.MaxClients];
        Notify();
    }

    static void Notify() {
        if (scoreChanged != null)
        {
            scoreChanged();
        }
    }
}
