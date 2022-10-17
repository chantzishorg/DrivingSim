using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class viewModel
{
    public static void Reportfailure(string failure)
    {
        Failure.Reportfailure(failure);
    }
   
    public static void loadImage(string fileName,bool isInstruction)
    {
        LastSign.loadImage(fileName, isInstruction);
    }
    public static void clearImage(bool isInstruction)
    {
        LastSign.clearImage(isInstruction);
    }
    public static void setScore(int score)
    {
        Score.setScore(score);
    }
    //public static void setTable(Dictionary<string, UserDataRecord> dict)
   // {
      //  HistoryScores.setTable(dict);
       // foreach (KeyValuePair<string, UserDataRecord> entry in dict) {
       //     Debug.Log($"{entry.Key} {entry.Value.Value}" );
       // }
  //  }
}
