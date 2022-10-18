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

    public static void SetRoadsLocation(List<List<Vector3>> allRoadsNodes)
    {
        App.SetRoadsLocation(allRoadsNodes);
    }

    public static void SetInitialCarLocation(float x, float z)
    {
        App.SetInitialCarLocation(x, z);
    }

    public static void SetInitialSpeed(float speed)
    {
        App.SetInitialSpeed(speed);
    }

    public static void AddNpcCars(float pX, float pY, Vector2 vector)
    {
        App.AddNpcCars(pX, pY, vector);
    }

    public static void AddNoEntrance(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddNoEntrance(x, z, width, vector_x, vector_z);
    }

    public static void AddSpeedLimit(float x, float z, float width, float vector_x, float vector_z, float newSpeedLimit, float oldSpeedLimit)
    {
        App.AddSpeedLimit(x, z, width, vector_x, vector_z, newSpeedLimit, oldSpeedLimit);
    }

    public static void AddFirstStop(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddFirstStop(x, z, width, vector_x, vector_z);
    }

    public static void AddSecondStop(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddSecondStop(x, z, width, vector_x, vector_z);
    }

    public static void AddSign(float x, float z, float width, float vector_x, float vector_z, string nameImage)
    {
        App.AddSign(x, z, width, vector_x, vector_z, nameImage);
    }

    public static void AddDirectionInstruction(float x, float z, float width, float vector_x, float vector_z, InstructionDirection turnDirection)
    {
        App.AddDirectionInstruction(x, z, width, vector_x, vector_z, turnDirection);
    }

    public static void AddnoValidDirectionVector(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddnoValidDirectionVector(x, z, width, vector_x, vector_z);
    }

    public static void AddValidDirectionVector(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddValidDirectionVector(x, z, width, vector_x, vector_z);
    }

    public static void AddScoreVector(float x, float z, float width, float vector_x, float vector_z, int score)
    {
        App.AddScoreVector(x, z, width, vector_x, vector_z, score);
    }

    public static void MoveCar(float x, float z, Vector2 direction)
    {
        App.MoveCar(x, z, direction);
    }

    public static void ReportSpeed(float speed)
    {
        App.ReportSpeed(speed);
    }

    public static void EndGame()
    {
        App.EndGame();
    }

    //public static void setTable(Dictionary<string, UserDataRecord> dict)
    // {
    //  HistoryScores.setTable(dict);
    // foreach (KeyValuePair<string, UserDataRecord> entry in dict) {
    //     Debug.Log($"{entry.Key} {entry.Value.Value}" );
    // }
    //  }
}
