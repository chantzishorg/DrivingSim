using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;

// playfab is a backend platform for games and update in real-time
public class PlayFabManager : MonoBehaviour
{
   
    public static void SubmitScore(int playerScore)
    {
        //update the scores in the data of the user
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "PlatformScore",
                Value = playerScore
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }

    private static void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    // if there is a error 
    public static void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    
    // save the date and score in the data of the user 
    public static void SetUserData(int score)
    {
        DateTime nowTime = DateTime.Now;
        //create and update 
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
             {$"{nowTime}", $"{score}"}
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    // return the data of the user
    public static void GetUserData(HistoryScores historyscore)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            // keys we want to load
            Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null) Debug.Log("null");
            else historyscore.setTable(result.Data);
        }, error => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    
    /*
    static void GetPlayerProfile(string playFabId)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName),
        error => Debug.LogError(error.GenerateErrorReport()));
    }
    void OnSuccess(LoginResult result)
    {
        Debug.Log("succeful login");
    }
    public static void OnError(PlayFabError error)
    {
        Debug.Log("error while login");
    }


    public static void SendLeaderboard(int score)
    {
        GetPlayerProfile("2D6EC");

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="PlatformScore",
                    Value=score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    public static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("successful leaderboard sent");
    }
    */
}
