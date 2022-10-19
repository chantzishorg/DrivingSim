using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;
using TMPro;

public class HistoryScores : MonoBehaviour
{
    // the row
    public GameObject rowPrefab;
    // the table
    public Transform rowsParent;

    // update in table the data of user
    public void setTable(Dictionary<string, UserDataRecord> dict)
    {
        var table = GameObject.Find("Canvas").transform.Find("History").transform.Find("Table").transform;
        var n = table.childCount;
        int i = 0;
        foreach (KeyValuePair<string, UserDataRecord> entry in dict)
        {
            if(i>= n)
            {
                return;
            }
            var newGo = table.GetChild(i++);
            var texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = entry.Key;
            texts[1].text = entry.Value.Value;
        }

    }

    // represents the history screen
    public void OnClick()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var menu = canvas.Find("MainMenu");
        menu.gameObject.SetActive(false);
        var panelHistory = canvas.Find("History");
        panelHistory.gameObject.SetActive(true);
        PlayFabManager.GetUserData(setTable);
    }
    
    // if click the back button
    public void onClickButton()
    {
        var canvas = GameObject.Find("Canvas").transform;
        var panelHistory = canvas.Find("History");
        panelHistory.gameObject.SetActive(false);
        var menu = canvas.Find("MainMenu");
        menu.gameObject.SetActive(true);
    }
}
