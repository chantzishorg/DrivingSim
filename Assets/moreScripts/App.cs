using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum PassingCode : ushort
{
    None = 0,
    OppositeDirection = 1,
    SameDirection = 2,
}
public class MyPoint
{
    public float x, z;
    public MyPoint(float x, float z)
    {
        this.x = x;
        this.z = z;
    }
}
public class DirectedLine
{
    //public MyPoint middle;
    //public MyPoint vector;
    public float x;
    public float z;
    public float width;
    public float vector_x;
    public float vector_z;
    public DirectedLine(float x, float z, float width, float vector_x, float vector_z)
    {
        //this.middle = new MyPoint(x, z);
        //this.vector = new MyPoint(vector_x, vector_z);
        this.x = x;
        this.z = z;
        this.width = width;
        this.vector_x = vector_x;
        this.vector_z = vector_z;
    }
}

public class SpeedLimit
{
    public DirectedLine speedLimitLine;
    public float newSpeedLimit;
    public float oldSpeedLimit;
    public SpeedLimit(DirectedLine speedLimitLine, float newSpeedLimit, float oldSpeedLimit)
    {
        this.speedLimitLine = speedLimitLine;
        this.newSpeedLimit = newSpeedLimit;
        this.oldSpeedLimit = oldSpeedLimit;
    }
}

public class App : MonoBehaviour
{
    //public GameObject failureObject
    public static bool isStop = false;
    public float initialSpeedLimit;
    private static List<DirectedLine> NoEntranceVector = new List<DirectedLine>();
    private static List<SpeedLimit> SpeedLimitList = new List<SpeedLimit>();
    private static List<DirectedLine> stopFirstVector = new List<DirectedLine>();
    private static List<DirectedLine> stopSecondVector = new List<DirectedLine>();
    private static MyPoint carLocation;
    private static float currentSpeedLimit;
    public static void SetInitialCarLocation(float x, float z)
    {
        carLocation = new MyPoint(x, z);
    }
    public static void SetInitialSpeed(float speed)
    {
        currentSpeedLimit = speed;
    }
    public static void AddNoEntrance(float x, float z, float width, float vector_x, float vector_z)
    {
        NoEntranceVector.Add(new DirectedLine(x, z, width, vector_x, vector_z));
    }
    public static void AddSpeedLimit(float x, float z, float width, float vector_x, float vector_z, float newSpeedLimit, float oldSpeedLimit)
    {
        SpeedLimitList.Add(new SpeedLimit(new DirectedLine(x, z, width, vector_x, vector_z), newSpeedLimit, oldSpeedLimit));
    }

    public static void AddFirstStop(float x, float z, float width, float vector_x, float vector_z) 
    {
        stopFirstVector.Add(new DirectedLine(x, z, width, vector_x, vector_z));
    }

    public static void AddSecondStop(float x, float z, float width, float vector_x, float vector_z)
    {
        stopSecondVector.Add(new DirectedLine(x, z, width, vector_x, vector_z));
    }

    public static void MoveCar(float x, float z)
    {
        MyPoint oldLocation = carLocation;
        carLocation = new MyPoint(x, z);
        for (int i = 0; i < NoEntranceVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, NoEntranceVector[i]);
            if (result == PassingCode.SameDirection)
            {
                viewModel.Reportfailure("You entered to no entry place!");
                Debug.Log("game over");
            }
        }
        for (int i = 0; i < SpeedLimitList.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, SpeedLimitList[i].speedLimitLine);
            if (result == PassingCode.SameDirection)
            {
                currentSpeedLimit = SpeedLimitList[i].newSpeedLimit;
            }
            if (result == PassingCode.OppositeDirection)
            {
                currentSpeedLimit = SpeedLimitList[i].oldSpeedLimit;
            }
        }
        // check stop stripes
        //loop over the first stop stripes 
        for (int i = 0; i < stopFirstVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, stopFirstVector[i]);
            if (result == PassingCode.SameDirection)
            {
                isStop = false;
            }
        }
        //loop over the second stop stripes 
        for (int i = 0; i < stopSecondVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, stopSecondVector[i]);
            if (result == PassingCode.SameDirection)
            {
                if (isStop == false)
                {
                    viewModel.Reportfailure("You dont stop!");
                }
            }
        }
    }
    public static void ReportSpeed(float speed)
    {
        // if the speed 0 the car stops
        if (speed < 1)
        {
           // Debug.Log("the speed is 0");
            isStop = true;
        }
        if (speed > currentSpeedLimit)
        {
            Debug.Log("game over");
            viewModel.Reportfailure("You exceeded the speed limit!");
        }
    }
    private static PassingCode checkCross(MyPoint start, MyPoint end, DirectedLine directedLine)
    {
        // start_end is a line between start and end
        float m_start_end;
        float b_start_end;
        // (x_i,z_i) = intersection between start_end and directedLine
        float x_i;
        float z_i;
        // distance between the intersection (x_i,z_i) to middle of directedLine
        float distance;
        // vector_m = slope of the vector of directedLine (in order to calculate -1/vector_m which is the slope of directedLine)
        float vector_m = directedLine.vector_z / directedLine.vector_x;
        // can't calculate -1/0
        if (vector_m == 0f)
        {
            if (directedLine.x <= start.x && directedLine.x <= end.x || directedLine.x >= start.x && directedLine.x >= end.x) return PassingCode.None;
            m_start_end = (end.z - start.z) / (end.x - start.x);
            b_start_end = -m_start_end * start.x + start.z;
            z_i = m_start_end * directedLine.x + b_start_end;
            distance = Mathf.Abs(z_i - directedLine.z);
            if (distance > directedLine.width / 2f) return PassingCode.None;
            if (directedLine.vector_x > 0 && start.x > end.x) return PassingCode.OppositeDirection;
            if (directedLine.vector_x < 0 && start.x < end.x) return PassingCode.OppositeDirection;
            return PassingCode.SameDirection;
        }
        float m = -1 / vector_m;
        // start and end are two lines parallel to directedLine
        float b_start = -m * start.x + start.z;
        float b_end = -m * end.x + end.z;
        float b_directedLine = -m * directedLine.x + directedLine.z;
        // if start and end are both on the same side
        if (b_directedLine <= b_start && b_directedLine <= b_end || b_directedLine >= b_start && b_directedLine >= b_end) return PassingCode.None;
        m_start_end = (end.z - start.z) / (end.x - start.x);
        b_start_end = -m_start_end * start.x + start.z;
        x_i = (b_directedLine - b_start_end) / (m_start_end - m);
        z_i = m * x_i + b_directedLine;
        distance = Mathf.Sqrt((x_i - directedLine.x) * (x_i - directedLine.x) + (z_i - directedLine.z) * (z_i - directedLine.z));
        // if beyond the directedLine
        if (distance > directedLine.width / 2f) return PassingCode.None;

        if (directedLine.vector_z > 0 && b_start >= b_end) return PassingCode.OppositeDirection;
        if (directedLine.vector_z < 0 && b_end >= b_start) return PassingCode.OppositeDirection;
        return PassingCode.SameDirection;
    }

    void Start()
    {
       // viewModel.loadImage("speedLimit30.png");
        Time.timeScale = 1f;
        SetInitialSpeed(initialSpeedLimit);
    }
}
