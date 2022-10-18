using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Statistics : MonoBehaviour
{
    public static void setScore(int score)
    {
        var scoreScreen = GameObject.Find("Canvas").transform.Find("score").GetComponent<TextMeshProUGUI>();
        scoreScreen.text = $"Score: {score}";
    }

    public static void setSpeed(int speed)
    {
        var speedScreen = GameObject.Find("Canvas").transform.Find("Speed").GetComponent<TextMeshProUGUI>();
        speedScreen.text = $"{speed} km/h";
    }
}
