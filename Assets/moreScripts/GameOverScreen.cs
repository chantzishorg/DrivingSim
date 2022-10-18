using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
  public void RestartButton()
    {
        // Time.timeScale = 1f;
        viewModel.EndGame();
        SceneManager.LoadScene("demo3");
    }

   public void ExitButton()
    {
        viewModel.EndGame();
        SceneManager.LoadScene("Menu");
    }
}
