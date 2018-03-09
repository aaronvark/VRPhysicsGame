using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
    public int clientId = -1;

    TextMesh textMesh;

    private void Start()
    {
        ScoreManager.scoreChanged += UpdateUI;
    }

    void UpdateUI()
    {
        if (clientId != -1)
        {
            textMesh.text = ScoreManager.GetScores()[clientId].ToString();
        }
    }
}
