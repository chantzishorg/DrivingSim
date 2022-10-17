using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;


public class HistoryScores : MonoBehaviour
{
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
    // the row
    public  GameObject rowPrefab;
    // the table
    public  Transform rowsParent;

    public  void setTable(Dictionary<string, UserDataRecord> dict)
    {
         GameObject newGo = Instantiate(rowPrefab, rowsParent);
         var texts = newGo.GetComponentsInChildren<Text>();
          texts[0].text = "1";
         texts[1].text = "e";

        // foreach (KeyValuePair<string, UserDataRecord> entry in dict)
        //{
        //   GameObject newGo = Instantiate(rowPrefab, rowsParent);
        //  var texts = newGo.GetComponentsInChildren<Text>();
        //  texts[0].text = entry.Key;
        //  texts[1].text = entry.Value.Value;

        // }

    }
    public void OnClick()
    {
        var menu = GameObject.Find("Canvas").transform.Find("MainMenu");
        menu.gameObject.SetActive(false);
        var panelHistory = GameObject.Find("Canvas").transform.Find("History");
        panelHistory.gameObject.SetActive(true);
        PlayFabManager.GetUserData(this);
       
    }

}
