using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static void setScore(int score)
    {       
        var scoreScreen = GameObject.Find("Canvas").transform.Find("valuescore").GetComponent<Text>();
       
        scoreScreen.text = score.ToString();
    } 
}
