using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Failure : MonoBehaviour
{
    public GameObject FailureTab;
    public Text FailureText;
    
    public void Reportfailure(string failure)
    {
        FailureTab.SetActive(true);
        FailureText.text = failure;
        //GameObject failurescreen=GameObject.Find("FailureBackground");
        //failurescreen.SetActive(true);
        //var failuretext = failurescreen.transform.Find("Failuredescription");
        // failuretext.GetComponent<Text>().text = failure;
        Time.timeScale = 0f;
        Debug.Log("game over");
    }

}
