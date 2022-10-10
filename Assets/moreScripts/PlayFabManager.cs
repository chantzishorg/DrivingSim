using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterButton(string email, string password)
    {
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest()
            {
                Email = email,
                Password = password,
                RequireBothUsernameAndEmail = true
            },
       response =>
       {
           Debug.Log($"Successful Account Creation: {email}");
       },
       error =>
       {
           Debug.Log($"UnSuccessful Account Creation: {email} \n {error.ErrorMessage}");

       }
       );
    }

 
    
}
