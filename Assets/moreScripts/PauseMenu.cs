using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// class that represents the menu we enter esc
public class PauseMenu : MonoBehaviour
{
    // variable that repersents if the game is paused
    public static bool GameIsPauesed = false;
    //
    public GameObject pauseMenuUi;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPauesed)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        // to return the time to normal
        Time.timeScale = 1f;
        GameIsPauesed = false;
    }

    void Pause()
    {
        pauseMenuUi.SetActive(true);
        // to freeze the game
        Time.timeScale = 0f;
        GameIsPauesed = true;
    }
    
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        viewModel.EndGame();
        Debug.Log("Quitting menu...");
        Application.Quit();
    }
    
}
    