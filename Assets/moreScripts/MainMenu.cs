using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// class that repersents the menu of the game 
public class MainMenu : MonoBehaviour
{
    // start the game
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // quit game 
    public void QuitGame()
    {
        viewModel.EndGame();
        Application.Quit();
    }
}
