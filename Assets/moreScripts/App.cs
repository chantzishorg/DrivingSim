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
public enum LightColor : ushort
{
    Green = 0,
    Yellow = 1,
    Red = 2,
}

public enum InstructionDirection : ushort
{
    turnRight = 0,
    turnLeft = 1,
}

public delegate void OnLightChange(int index, LightColor color);
public class TrafficLightIntersection
{
    public (DirectedLine pre, DirectedLine passing, LightColor color)[] trafficLights =
        new (DirectedLine pre, DirectedLine passing, LightColor color)[4];
    public OnLightChange onLightChange;
    public TrafficLightIntersection((DirectedLine pre, DirectedLine passing) trafficLight1,
        (DirectedLine pre, DirectedLine passing) trafficLight2,
        (DirectedLine pre, DirectedLine passing) trafficLight3,
        (DirectedLine pre, DirectedLine passing) trafficLight4,
        OnLightChange onLightChange
        )
    {
        trafficLights[0].pre = trafficLight1.pre;
        trafficLights[1].pre = trafficLight2.pre;
        trafficLights[2].pre = trafficLight3.pre;
        trafficLights[3].pre = trafficLight4.pre;
        trafficLights[0].passing = trafficLight1.passing;
        trafficLights[1].passing = trafficLight2.passing;
        trafficLights[2].passing = trafficLight3.passing;
        trafficLights[3].passing = trafficLight4.passing;
        this.onLightChange = onLightChange;
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

public class Sign
{
    // sign line
    public DirectedLine signLine;
    // name of the image of the sign
    public string nameImage;
    public Sign(DirectedLine signLine, string nameImage)
    {
        this.signLine = signLine;
        this.nameImage = nameImage;
    }
}

// class of the turn instruction
public class TurnInstruction
{
    // turn line
    public DirectedLine turnLine;
    public InstructionDirection turnDirection;
    public TurnInstruction(DirectedLine turnLine, InstructionDirection turnDirection)
    {
        this.turnLine = turnLine;
        this.turnDirection = turnDirection;
    }
}

// class of score spline
public class scoreSpline
{
    public DirectedLine scoreLine;
    public int score;
    public scoreSpline(DirectedLine scoreLine, int score)
    {
        this.scoreLine = scoreLine;
        this.score = score;
    }
}

public class App : MonoBehaviour
{
    private static int score = 0;
    public static int Score { get { return score; } }
    public float initialSpeedLimit;
    private static bool isStop = false;
    private static List<DirectedLine> NoEntranceVector = new List<DirectedLine>();
    private static List<SpeedLimit> SpeedLimitList = new List<SpeedLimit>();
    private static List<DirectedLine> stopFirstVector = new List<DirectedLine>();
    private static List<DirectedLine> stopSecondVector = new List<DirectedLine>();
    // list of the turn lines
    private static List<TurnInstruction> turnDirectionVector = new List<TurnInstruction>();
    private static List<Sign> signVector = new List<Sign>();
    private static List<DirectedLine> noValidDirectionVector = new List<DirectedLine>();
    private static List<DirectedLine> ValidDirectionVector = new List<DirectedLine>();
    private static List<scoreSpline> scoreVector = new List<scoreSpline>();
    private static Vector2 carLocation = new Vector2(-1f, -1f);
    private static float currentSpeedLimit;
    private static RoadsModel roadsModel = null;
    private static List<(Vector2, Vector2)> npcCars = new List<(Vector2, Vector2)>();
    private static List<TrafficLightIntersection> trafficLightIntersections = new List<TrafficLightIntersection>();
    private int currentGreenRoad = 0;

    public static void AddTrafficLights(
        (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl1,
        (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl2,
        (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl3,
        (Vector2 middle1, float width1, Vector2 vec1, Vector2 middle2, float width2, Vector2 vec2) tl4,
        OnLightChange onLightChange
        )
    {
        var trafficLight1 = (new DirectedLine(tl1.middle1.x, tl1.middle1.y, tl1.width1, tl1.vec1.x, tl1.vec1.y),
            new DirectedLine(tl1.middle2.x, tl1.middle2.y, tl1.width2, tl1.vec2.x, tl1.vec2.y));
        var trafficLight2 = (new DirectedLine(tl2.middle1.x, tl2.middle1.y, tl2.width1, tl2.vec1.x, tl2.vec1.y),
            new DirectedLine(tl2.middle2.x, tl2.middle2.y, tl2.width2, tl2.vec2.x, tl2.vec2.y));
        var trafficLight3 = (new DirectedLine(tl3.middle1.x, tl3.middle1.y, tl3.width1, tl3.vec1.x, tl3.vec1.y),
            new DirectedLine(tl3.middle2.x, tl3.middle2.y, tl3.width2, tl3.vec2.x, tl3.vec2.y));
        var trafficLight4 = (new DirectedLine(tl4.middle1.x, tl4.middle1.y, tl4.width1, tl4.vec1.x, tl4.vec1.y),
            new DirectedLine(tl4.middle2.x, tl4.middle2.y, tl4.width2, tl4.vec2.x, tl4.vec2.y));
        trafficLightIntersections.Add(
            new TrafficLightIntersection(trafficLight1, trafficLight2, trafficLight3, trafficLight4, onLightChange)
            );
    }
    public static void AddNpcCars(float pX, float pY, Vector2 vector)
    {
        Vector2 point = new Vector2(pX, pY);
        npcCars.Add((point, vector));
    }

    public static void SetRoadsLocation(List<List<Vector3>> allRoadsNodes)
    {
        roadsModel = new RoadsModel(allRoadsNodes);
        if (carLocation != null)
        {
            roadsModel.setCarStartingPoint(carLocation.x, carLocation.y);
        }
    }

    public static void SetInitialCarLocation(float x, float z)
    {
        carLocation = new Vector2(x, z);
        if (roadsModel != null)
        {
            roadsModel.setCarStartingPoint(x, z);
        }
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

    public static void AddSign(float x, float z, float width, float vector_x, float vector_z, string nameImage)
    {
        signVector.Add(new Sign(new DirectedLine(x, z, width, vector_x, vector_z), nameImage));
    }

    // add the direction instruction to list
    public static void AddDirectionInstruction(float x, float z, float width, float vector_x, float vector_z, InstructionDirection turnDirection)
    {
        turnDirectionVector.Add(new TurnInstruction(new DirectedLine(x, z, width, vector_x, vector_z), turnDirection));
    }
    public static void AddnoValidDirectionVector(float x, float z, float width, float vector_x, float vector_z)
    {
        noValidDirectionVector.Add(new DirectedLine(x, z, width, vector_x, vector_z));
    }
    public static void AddValidDirectionVector(float x, float z, float width, float vector_x, float vector_z)
    {
        ValidDirectionVector.Add(new DirectedLine(x, z, width, vector_x, vector_z));
    }
    public static void AddScoreVector(float x, float z, float width, float vector_x, float vector_z, int score)
    {
        scoreVector.Add(new scoreSpline(new DirectedLine(x, z, width, vector_x, vector_z), score));
    }
    public static void MoveCar(float x, float z, Vector2 direction)
    {
        roadsModel.MoveCar(x, z, direction);
        Vector2 oldLocation = carLocation;
        carLocation = new Vector2(x, z);
        for (int i = 0; i < NoEntranceVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, NoEntranceVector[i]);
            if (result == PassingCode.SameDirection)
            {
                viewModel.Reportfailure("You entered to no entry place!");
            }
        }
        for (int i = 0; i < SpeedLimitList.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, SpeedLimitList[i].speedLimitLine);
            if (result == PassingCode.SameDirection)
            {
                currentSpeedLimit = SpeedLimitList[i].newSpeedLimit;
                viewModel.clearImage(false);
            }
            if (result == PassingCode.OppositeDirection)
            {
                currentSpeedLimit = SpeedLimitList[i].oldSpeedLimit;
                viewModel.clearImage(false);
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
                    viewModel.clearImage(false);
                }
            }
        }
        //loop over the signVector
        for (int i = 0; i < signVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, signVector[i].signLine);
            if (result == PassingCode.SameDirection)
            {
                viewModel.loadImage(signVector[i].nameImage, false);
            }
        }

        //loop over the turnDirectionVector
        for (int i = 0; i < turnDirectionVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, turnDirectionVector[i].turnLine);
            if (result == PassingCode.SameDirection)
            {
                if (turnDirectionVector[i].turnDirection == InstructionDirection.turnRight)
                {
                    viewModel.loadImage("Assets/moreSigns/turnRight.png", true);
                }
                else
                {
                    viewModel.loadImage("Assets/moreSigns/turnLeft.png", true);
                }
            }
        }
        for (int i = 0; i < noValidDirectionVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, noValidDirectionVector[i]);
            if (result == PassingCode.SameDirection)
            {
                viewModel.Reportfailure("You didn't turn in the right direction!");
                //viewModel.clearImage();
            }
        }
        for (int i = 0; i < ValidDirectionVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, ValidDirectionVector[i]);
            if (result == PassingCode.SameDirection)
            {
                viewModel.clearImage(true);
            }
        }
        for (int i = 0; i < scoreVector.Count; i++)
        {
            PassingCode result = checkCross(oldLocation, carLocation, scoreVector[i].scoreLine);
            if (result == PassingCode.SameDirection)
            {
                score += scoreVector[i].score;
                viewModel.setScore(score);
            }
        }
        foreach ((Vector2 point, Vector2 vector) npcCar in npcCars)
        {
            if (Collision.isIntersection(carLocation, npcCar.point, direction, npcCar.vector))
            {
                viewModel.Reportfailure("You collide with car!");
            }
        }
    }
    public static void ReportSpeed(float speed)
    {
        viewModel.setSpeed(speed);
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

    // function that calucate the rectangle around the center of the car
    private static PassingCode checkCross(Vector2 start, Vector2 end, DirectedLine directedLine)
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
            // if they start and end in the same side
            if (directedLine.x <= start.x && directedLine.x <= end.x || directedLine.x >= start.x && directedLine.x >= end.x) return PassingCode.None;
            // calculate line between start and end
            m_start_end = (end.y - start.y) / (end.x - start.x);
            b_start_end = -m_start_end * start.x + start.y;
            z_i = m_start_end * directedLine.x + b_start_end;
            distance = Mathf.Abs(z_i - directedLine.z);
            if (distance > directedLine.width / 2f) return PassingCode.None;
            if (directedLine.vector_x > 0 && start.x > end.x) return PassingCode.OppositeDirection;
            if (directedLine.vector_x < 0 && start.x < end.x) return PassingCode.OppositeDirection;
            return PassingCode.SameDirection;
        }
        float m = -1 / vector_m;
        // start and end are two lines parallel to directedLine
        float b_start = -m * start.x + start.y;
        float b_end = -m * end.x + end.y;
        float b_directedLine = -m * directedLine.x + directedLine.z;
        // if start and end are both on the same side
        if (b_directedLine <= b_start && b_directedLine <= b_end || b_directedLine >= b_start && b_directedLine >= b_end) return PassingCode.None;
        m_start_end = (end.y - start.y) / (end.x - start.x);
        b_start_end = -m_start_end * start.x + start.y;
        x_i = (b_directedLine - b_start_end) / (m_start_end - m);
        z_i = m * x_i + b_directedLine;
        distance = Mathf.Sqrt((x_i - directedLine.x) * (x_i - directedLine.x) + (z_i - directedLine.z) * (z_i - directedLine.z));
        // if beyond the directedLine
        if (distance > directedLine.width / 2f) return PassingCode.None;

        if (directedLine.vector_z > 0 && b_start >= b_end) return PassingCode.OppositeDirection;
        if (directedLine.vector_z < 0 && b_end >= b_start) return PassingCode.OppositeDirection;
        return PassingCode.SameDirection;
    }

    void Awake()
    {
        score = 0;
        viewModel.isRunning = true;
        isStop = false;
        NoEntranceVector = new List<DirectedLine>();
        SpeedLimitList = new List<SpeedLimit>();
        stopFirstVector = new List<DirectedLine>();
        stopSecondVector = new List<DirectedLine>();
        // list of the turn lines
        turnDirectionVector = new List<TurnInstruction>();
        signVector = new List<Sign>();
        noValidDirectionVector = new List<DirectedLine>();
        ValidDirectionVector = new List<DirectedLine>();
        scoreVector = new List<scoreSpline>();
        carLocation = new Vector2(-1f, -1f);
        roadsModel = null;
        npcCars = new List<(Vector2, Vector2)>();
        trafficLightIntersections = new List<TrafficLightIntersection>();
        currentGreenRoad = 0;
    }

    void Start()
    {
        viewModel.setScore(score);
        Time.timeScale = 1f;
        SetInitialSpeed(initialSpeedLimit);
        Invoke("flipRoadGreen", 1f);
    }

    static public void EndGame()
    {
        PlayFabManager.SetUserData(score);
    }

    void flipRoadGreen()
    {
        foreach (var trafficLightIntersection in trafficLightIntersections)
        {
            trafficLightIntersection.onLightChange(currentGreenRoad, LightColor.Green);
            trafficLightIntersection.onLightChange(currentGreenRoad + 1, LightColor.Green);
        }
        currentGreenRoad = (currentGreenRoad + 2) % 4;
        foreach (var trafficLightIntersection in trafficLightIntersections)
        {
            trafficLightIntersection.onLightChange(currentGreenRoad, LightColor.Red);
            trafficLightIntersection.onLightChange(currentGreenRoad + 1, LightColor.Red);
        }
        Invoke("flipRoadYellow", 10f);
    }
    void flipRoadYellow()
    {
        currentGreenRoad = (currentGreenRoad + 2) % 4;
        foreach (var trafficLightIntersection in trafficLightIntersections)
        {
            trafficLightIntersection.onLightChange(currentGreenRoad, LightColor.Yellow);
            trafficLightIntersection.onLightChange(currentGreenRoad + 1, LightColor.Yellow);
        }
        Invoke("flipRoadRed", 1f);
    }
    void flipRoadRed()
    {
        foreach (var trafficLightIntersection in trafficLightIntersections)
        {
            trafficLightIntersection.onLightChange(currentGreenRoad, LightColor.Red);
            trafficLightIntersection.onLightChange(currentGreenRoad + 1, LightColor.Red);
        }
        currentGreenRoad = (currentGreenRoad + 2) % 4;
        Invoke("flipRoadGreen", 1f);
    }
}


