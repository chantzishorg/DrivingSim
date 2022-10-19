using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Failure
{
    public static void Reportfailure(string failure)
    {
        var canvas = GameObject.Find("Canvas");
        var failurescreen = canvas.transform.Find("FailureBackground");
        failurescreen.gameObject.SetActive(true);
        var failuretext = failurescreen.transform.Find("failuredescription");
        failuretext.GetComponent<TextMeshProUGUI>().text = failure;
        var scoretext= failurescreen.transform.Find("scoredescription");
        scoretext.GetComponent<TextMeshProUGUI>().text = $" Your score is: {App.Score}";
        Time.timeScale = 0f;
        Debug.Log("game over");
    }
}
