using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;


public class HistoryScores : MonoBehaviour
{
    //debug start
    string Encrypt(string pass)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }
    
    void Start()
    {
        var request = new LoginWithEmailAddressRequest { Email = "chaim@gmail.com", Password = Encrypt("****") };

        PlayFabClientAPI.LoginWithEmailAddress(request, null, null);
       
    }
    //debug end
    // the row
    public GameObject rowPrefab;
    // the table
    public Transform rowsParent;

    public void setTable(Dictionary<string, UserDataRecord> dict)
    {
        var table = GameObject.Find("Canvas").transform.Find("History").transform.Find("Table").transform;
        var n = table.childCount;
        int i = 0;
        foreach (KeyValuePair<string, UserDataRecord> entry in dict)
        {
            //Debug.Log($"{entry.Key}: {entry.Value.Value}");
            //GameObject newGo = Instantiate(rowPrefab, rowsParent);
            if(i>= n)
            {
                return;
            }
            var newGo = table.GetChild(i++);
            var texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = entry.Key;
            texts[1].text = entry.Value.Value;
        }

        //GameObject newGo = Instantiate(rowPrefab, rowsParent);
        //var texts = newGo.GetComponentsInChildren<Text>();
        //texts[0].text = "1";
        //texts[1].text = "e";

        //foreach (KeyValuePair<string, UserDataRecord> entry in dict)
        //{
        //  GameObject newGo = Instantiate(rowPrefab, rowsParent);
        //  var texts = newGo.GetComponentsInChildren<Text>();
        //  texts[0].text = entry.Key;
        //  texts[1].text = entry.Value.Value;
        //}

    }
    public void OnClick()
    {
        //Debug.Log("OnClick");
        var canvas = GameObject.Find("Canvas").transform;
        var menu = canvas.Find("MainMenu");
        menu.gameObject.SetActive(false);
        var panelHistory = canvas.Find("History");
        panelHistory.gameObject.SetActive(true);
        PlayFabManager.GetUserData(setTable);

    }

}
