using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Failure
{
    public static void Reportfailure(string failure)
    {
        var canvas = GameObject.Find("Canvas");
        var failurescreen = canvas.transform.Find("FailureBackground");
        failurescreen.gameObject.SetActive(true);
        var failuretext = failurescreen.transform.Find("Failuredescription");
        failuretext.GetComponent<Text>().text = failure +$" Your score is: {App.Score}";
        Time.timeScale = 0f;
        Debug.Log("game over");
    }
}
