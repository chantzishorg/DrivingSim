using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class End
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
    }
    public static void ReportEnd(string failure)
    {
        var canvas = GameObject.Find("Canvas");
        var failurescreen = canvas.transform.Find("SuccessBackground");
        failurescreen.gameObject.SetActive(true);
        var failuretext = failurescreen.transform.Find("successdescription");
        failuretext.GetComponent<TextMeshProUGUI>().text = failure;
        var scoretext = failurescreen.transform.Find("scoredescription");
        scoretext.GetComponent<TextMeshProUGUI>().text = $" Your score is: {App.Score}";
        Time.timeScale = 0f;
    }
}
