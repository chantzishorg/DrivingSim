using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
public class viewModel
{
    // variable that check if the game is runnning
    public static bool isRunning = true;
    public static void Reportfailure(string failure)
    {
        if (isRunning)
        {
            isRunning = false;
            Failure.Reportfailure(failure);
        }
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
        Statistics.setScore(score);
    }
    public static void setSpeed(float speed)
    {
        Statistics.setSpeed(Mathf.RoundToInt(speed));
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

    public static void AddValidSignVector(float x, float z, float width, float vector_x, float vector_z)
    {
        App.AddValidSignVector(x, z, width, vector_x, vector_z);
    }

    public static void AddTrafficLights(
       (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl1,
       (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl2,
       (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl3,
       (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl4,
       OnLightChange onLightChange,
       OnAreaEnter onAreaEnter
       )
    {
        App.AddTrafficLights(tl1, tl2, tl3, tl4, onLightChange, onAreaEnter);
    }

        public static void MoveCar(float x, float z, Vector2 direction)
    {
        if (isRunning)
        {
            App.MoveCar(x, z, direction);
        }
    }

    public static void ReportSpeed(float speed)
    {
        if (isRunning)
        {
            App.ReportSpeed(speed);
        }
    }

    public static void EndGame()
    {
            isRunning = false;
            App.EndGame(); 
    }
}
