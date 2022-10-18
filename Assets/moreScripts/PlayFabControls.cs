using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabControls : MonoBehaviour
{
    [SerializeField] GameObject signUpTab, loginTab, startPanel, HUD;
    public Text username, userEmail, userPassword, userEmailLogin, userPasswordLogin, errorSignUp, errorLogin;
    string encryptedPassword;

    public void SwitchToSignUpTab()
    {
        signUpTab.SetActive(true);
        loginTab.SetActive(false);
    }

    public void SwitchToLoginTab()
    {
        signUpTab.SetActive(false);
        loginTab.SetActive(true);
    }
    public void SignUpTab()
    {
        signUpTab.SetActive(true);
        loginTab.SetActive(false);

    }
    public void LoginTab()
    {
        signUpTab.SetActive(false);
        loginTab.SetActive(true);
    }

   string Encrypt(string pass)
   {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        // encodes a set of characters of pass into a sequence of bytes
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
        // hash of the password
        bs = x.ComputeHash(bs);
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach(byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }
        return s.ToString();
    }

    public void SignUp()
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail.text, Password = Encrypt(userPassword.text), Username = username.text };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
    }

    public void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        errorSignUp.text = "";
        errorLogin.text = "";
        StartGame();
    }

    public void RegisterError(PlayFabError error)
    {
        errorSignUp.text = error.ErrorMessage;
    }

    public void LogIn()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmailLogin.text, Password = Encrypt(userPasswordLogin.text)};
        //Debug.Log(userEmailLogin.text);
        //Debug.Log(userPasswordLogin.text);
      //  var request = new LoginWithEmailAddressRequest { Email = "chaim@gmail.com", Password = Encrypt("****")};
        PlayFabClientAPI.LoginWithEmailAddress(request, LogInSuccess, LogInError);
    }

    public void LogInSuccess(LoginResult result)
    {
        errorSignUp.text = "";
        errorLogin.text = "";
      //  DontDestroyOnLoad(); // keeps this gameobject through scene transitions
        StartGame();
    }

    public void LogInError(PlayFabError error)
    {
        errorLogin.text = error.ErrorMessage;

    }
    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    /*
    public void SendLeaderboard(int score)
    {
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
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, LogInError);
    }
        void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("successful leaderboard sent");
     
    }
    */
    /*
    void Start()
    {
        DontDestroyOnLoad(this.gameObject); // keeps this gameobject through scene transitions
        // here you might kick off some kind of login thing, like showing a login box or whatever
    }
    */
}
