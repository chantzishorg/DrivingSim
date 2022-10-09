//#undef UNITY_EDITOR
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadArchitect;
using RoadArchitect.Roads;
using System.Xml;
//using MyNamespace;

public class Way
{
    public List<Vector2> wayValues;
    public int maxSpeedWay;
    public bool isOneWay;

    public Way(List<Vector2> wayValues, int maxSpeedWay, bool isOneWay)
    {
        this.wayValues = wayValues;
        this.maxSpeedWay = maxSpeedWay;
        this.isOneWay = isOneWay;
    }

    // public List<Vector2> points { get; set; }

    // public bool isOneway { get; set; }
    //public int maxspeed { get; set; }

}
public class Square
{
    public Vector2 point_1, point_2, point_3, point_4; // 4 edge points (normally in clock wise order starting in the lower left point)
    public float a_1_2, b_1_2, a_2_3, b_2_3, a_3_4, b_3_4, a_1_4, b_1_4; // 4 line equations
    public Square s_1_2, s_2_3, s_3_4, s_1_4; // adjacent squares also numbered sides: 1, 2, 3, 4
    public Square() { }
    public Square(Vector2 point_1, Vector2 point_2, Vector2 point_3, Vector2 point_4, Square s_1_2, Square s_2_3, Square s_3_4, Square s_1_4)
    {
        this.point_1 = point_1;
        this.point_2 = point_2;
        this.point_3 = point_3;
        this.point_4 = point_4;
        
        this.s_1_2 = s_1_2;
        this.s_2_3 = s_2_3;
        this.s_3_4 = s_3_4;
        this.s_1_4 = s_1_4;

        // calculate equations
        a_1_2 = (point_1.y - point_2.y) / (point_1.x - point_2.x);
        b_1_2 = point_1.y - a_1_2 * point_1.x;
        a_2_3 = (point_2.y - point_3.y) / (point_2.x - point_3.x);
        b_2_3 = point_2.y - a_2_3 * point_2.x;
        a_3_4 = (point_3.y - point_4.y) / (point_3.x - point_4.x);
        b_3_4 = point_3.y - a_3_4 * point_3.x;
        a_1_4 = (point_1.y - point_4.y) / (point_1.x - point_4.x);
        b_1_4 = point_1.y - a_1_4 * point_1.x;
    }
    public Square calculate(Vector2 oldLocation, Vector2 newLocation)
    {
        if (checkCross(oldLocation, newLocation, point_1, point_2, a_1_2, b_1_2))
        {
            return s_1_2;
        }
        if (checkCross(oldLocation, newLocation, point_2, point_3, a_2_3, b_2_3))
        {
            return s_2_3;
        }
        if (checkCross(oldLocation, newLocation, point_3, point_4, a_3_4, b_3_4))
        {
            return s_3_4;
        }
        if (checkCross(oldLocation, newLocation, point_1, point_4, a_1_4, b_1_4))
        {
            return s_1_4;
        }
        return this;
    }

    private static bool checkCross(Vector2 start, Vector2 end, Vector2 point_1, Vector2 point_2, float a_1_2, float b_1_2)
    {
        Vector2 middle = new Vector2((point_1.x+ point_2.x) /2, (point_1.y + point_2.y) / 2);
        float width = Vector2.Distance(point_2, point_1);
        // start_end is a line between start and end
        float a_start_end;
        float b_start_end;
        // (x_i,y_i) = intersection between start_end and directedLine
        float x_i;
        float y_i;
        // distance between the intersection (x_i,z_i) to middle of directedLine
        float distance;
        // vector_m = slope of the vector of directedLine (in order to calculate -1/vector_m which is the slope of directedLine)
        //float vector_m = directedLine.vector_z / directedLine.vector_x;
        // can't calculate -1/0
        if (float.IsInfinity(a_1_2))
        {
            if (middle.x <= start.x && middle.x <= end.x || middle.x >= start.x && middle.x >= end.x) return false;
            a_start_end = (end.y - start.y) / (end.x - start.x);
            b_start_end = -a_start_end * start.x + start.y;
            y_i = a_start_end * middle.x + b_start_end;
            distance = Mathf.Abs(y_i - middle.y);
            if (distance > width / 2f) return false;
            //if (directedLine.vector_x > 0 && start.x > end.x) return PassingCode.OppositeDirection;
            //if (directedLine.vector_x < 0 && start.x < end.x) return PassingCode.OppositeDirection;
            return true;
        }
        //float m = -1 / vector_m;
        // start and end are two lines parallel to directedLine
        float b_start = -a_1_2 * start.x + start.y;
        float b_end = -a_1_2 * end.x + end.y;
        //float b_directedLine = -m * directedLine.x + directedLine.z;
        // if start and end are both on the same side
        if (b_1_2 <= b_start && b_1_2 <= b_end || b_1_2 >= b_start && b_1_2 >= b_end) return false;
        a_start_end = (end.y - start.y) / (end.x - start.x);
        b_start_end = -a_start_end * start.x + start.y;
        x_i = (b_1_2 - b_start_end) / (a_start_end - a_1_2);
        y_i = a_1_2 * x_i + b_1_2;
        distance = Mathf.Sqrt((x_i - middle.x) * (x_i - middle.x) + (y_i - middle.y) * (y_i - middle.y));
        // if beyond the directedLine
        if (distance > width / 2f) return false;
        //if (directedLine.vector_z > 0 && b_start >= b_end) return PassingCode.OppositeDirection;
        //if (directedLine.vector_z < 0 && b_end >= b_start) return PassingCode.OppositeDirection;
        return true;
    }
}
public class Intersection
{
    public Square square;
    //public Vector3[] splinePoints;
    public int middleIdx;
    public bool isFirstRoad; // first road corresponds to: 1_2 back 3_4 front. seconnd road corresponds to: 1_4 back 2_3 front.
    public bool isCounterClockWise;
    public Intersection(Square square, /*Vector3 a, Vector3 b, Vector3 c,*/ int middleIdx, bool isFirstRoad, bool isCounterClockWise) {
        //this.splinePoints = new Vector3[3];
        this.square = square;
        //this.splinePoints[0] = a;
        //this.splinePoints[1] = b;
        //this.splinePoints[2] = c;
        this.middleIdx = middleIdx;
        this.isFirstRoad = isFirstRoad;
        this.isCounterClockWise = isCounterClockWise;
    }
}
public class test2 : MonoBehaviour
{
    #region model
    public static Vector2 carLocation;
    public static Square carSquare;
    //public Vector2[] carBorders;
    public static void MoveCar(float x, float z)
    {
        Vector2 oldLocation = carLocation;
        carLocation = new Vector2(x, z);
        if (carSquare == null)
        {
            Debug.Log("Car outside of the road");
            return;
        }
        //Debug.Log($"{carSquare.point_1}, {carSquare.point_2}, {carSquare.point_3}, {carSquare.point_4}");
        carSquare = carSquare.calculate(oldLocation, carLocation);
    }

    static List<Way> Parsexml()
    {
        List<string> highways = new List<string>();
        // the names of highways we insert to the list highways
        string[] namesHighways = { "motorway", "trunk", "primary", "secondary", "tertiary", "unclassified","residential",
                                       "motorway_link", "trunk_link", "primary_link", "secondary_link", "tertiary_link " };
        highways.AddRange(namesHighways);
        // List of all the ways lists
        List<Way> ways = new List<Way>();
        // read from the path the xml file
        XmlDocument xmldoc = new XmlDocument();
        string XMLpath = Directory.GetCurrentDirectory() + @"\example.xml";
        xmldoc.Load(XMLpath);
        // get the lines of tag node
        XmlNodeList nodelist = xmldoc.GetElementsByTagName("node");
        // list of the ways
        XmlNodeList waylist = xmldoc.GetElementsByTagName("way");
        // bounds tag
        XmlNodeList bounds = xmldoc.GetElementsByTagName("bounds");
        double maxlat, maxlon, minlat, minlon;
        minlat = double.Parse(bounds[0].Attributes["minlat"].Value);
        minlon = double.Parse(bounds[0].Attributes["minlon"].Value);
        maxlat = double.Parse(bounds[0].Attributes["maxlat"].Value);
        maxlon = double.Parse(bounds[0].Attributes["maxlon"].Value);
        // list of lat values
        List<double> latList = new List<double>();
        // list of lon values
        List<double> lonList = new List<double>();
        // dictionart the key is id of node and value is the index of the lat and lon values
        Dictionary<long, int> idDict = new Dictionary<long, int>();
        // the way is valid type
        bool isValidType = false;
        // the way is highway
        bool isHighWay = false;
        // the way is oneway
        bool isoneWay = false;


        // the values in float
        double valueLatF;
        double valueLonF;
        int finalIndex = 0;
        for (int i = 0; i < nodelist.Count; i++)
        {
            string valueLat = nodelist[i].Attributes["lat"].Value;
            valueLatF = double.Parse(valueLat);
            
            string valueLon = nodelist[i].Attributes["lon"].Value;
            valueLonF = double.Parse(valueLon);

            if (valueLatF <= maxlat && valueLatF >= minlat && valueLonF <= maxlon && valueLonF >= minlon) { 
                latList.Add(valueLatF);
                lonList.Add(valueLonF);
                string valueId = nodelist[i].Attributes["id"].Value;
                long valueIdLong = long.Parse(valueId);
                idDict[valueIdLong] = finalIndex++;
                //Console.WriteLine($"value lat {valueLatF} of the {i} node"); 
                //Console.WriteLine($"value lon {valueLonF} of the {i} node");
            }
        }
        // min value lon and latitude
        double minvalueLatF = latList.Min();
        double maxvalueLatF = latList.Max();
        double minvalueLonF = lonList.Min();
        double maxvalueLonF = lonList.Max();
        //Debug.Log($"value min lat {minvalueLatF}");
        //Debug.Log($"value min lon {minvalueLonF}");
        //Debug.Log($"value max lat {maxvalueLatF}");
        //Debug.Log($"value max lon {maxvalueLonF}");
        // values according to: (x-minlon)*1000/(maxlon-minlon), (y-minlat)*1000/(maxlat-minlat) 

        for (int i = 0; i < latList.Count; i++)
        {
            latList[i] = (latList[i] - minvalueLatF) * 1000 / (maxvalueLatF - minvalueLatF);
            //Console.WriteLine($"final value lat {latList[i]} of the {i} node");

        }

        for (int j = 0; j < latList.Count; j++)
        {
            lonList[j] = (lonList[j] - minvalueLonF) * 1000 / (maxvalueLonF - minvalueLonF);
            //Console.WriteLine($"final value lon {lonList[j]} of the {j} node");

        }
        // loop over the way tags

        for (int index = 0; index < waylist.Count; index++)
        {
            // list of values of lon annd lat values of the way 
            List<Vector2> wayPoints = new List<Vector2>();
            int maxSpeed = 0;

            if (waylist[index].HasChildNodes)
            {

                //loop over the nodes in ways tag
                for (int i = 0; i < waylist[index].ChildNodes.Count; i++)
                {
                    // check if the node is tag 
                    if (waylist[index].ChildNodes[i].LocalName == "tag")
                    {
                        // check if the type is highway
                        string valueTag = waylist[index].ChildNodes[i].Attributes["k"].Value;
                        if (valueTag == "highway")
                        {
                            isHighWay = true;
                            // check if the type from the namehighways 
                            string valueTypeHighWay = waylist[index].ChildNodes[i].Attributes["v"].Value;
                            if (highways.Contains(valueTypeHighWay))
                            {
                                isValidType = true;
                            }
                        }

                        // check if the type is oneway
                        if (valueTag == "oneway")
                        {
                            isoneWay = true;
                        }

                        // check the maxspeed
                        if (valueTag == "maxspeed")
                        {
                            string valueSpeed = waylist[index].ChildNodes[i].Attributes["v"].Value;
                            //Debug.Log(valueSpeed);
                            if(valueSpeed.ToLower().EndsWith(" mph"))
                            {
                                valueSpeed = valueSpeed.Substring(0,valueSpeed.Length-4);
                                maxSpeed = int.Parse(valueSpeed);
                                maxSpeed = Mathf.RoundToInt((float)(1.609344 * maxSpeed));
                            }
                            else
                            {
                                maxSpeed = int.Parse(valueSpeed);
                            }
                        }
                    }

                    // check if the node is nd 
                    if (waylist[index].ChildNodes[i].LocalName == "nd")
                    {
                        // get the id of the node
                        string valueRef = waylist[index].ChildNodes[i].Attributes["ref"].Value;
                        // get from the dictionary the index of lat and lon

                        if (idDict.ContainsKey(long.Parse(valueRef))){
                            int indexList = idDict[long.Parse(valueRef)];
                            // create point of lat and lon values
                            Vector2 point = new Vector2((float)latList[indexList], (float)lonList[indexList]);
                            wayPoints.Add(point);
                        }

                    }
                }
            }
            if (isHighWay == true && isValidType == true)
            {
                Way way = new Way(wayPoints, maxSpeed, isoneWay);
                //  Console.WriteLine(way.maxSpeedWay);
                ways.Add(way);
            }
            isValidType = false;
            isHighWay = false;
        }

        // print for check

        /*foreach (var way in ways)
        {
            if (way.isOneWay)
            {
                Console.WriteLine("the way is one way");
            }

            Console.WriteLine("the speed is:" + way.maxSpeedWay);
            foreach (var point in way.wayValues)
            {
                Console.WriteLine("the x value:" + point.x + " the y value:" + point.y);
            }
        }*/

        return ways;

    }
    #endregion

    #region view
    public GameObject carPrefab;
    public static float tangent2Yrotation(Vector3 POS)
    {
        float angle = Mathf.Atan(POS.x / POS.z) * 180 / Mathf.PI;
        if (POS.z < 0)
        {
            if (POS.x > 0) angle += 180;
            else angle -= 180; // if x<=0
        }
        return angle;
    }
    #endregion

    // Start is called before the first frame update
    void Start() //main
    {
        Debug.Log(Directory.GetCurrentDirectory());
        #region model
        List<Way> allWays = Parsexml();
        List<RoadModel> allroads = new List<RoadModel>();
        foreach (var way in allWays)
        {
            List<Vector3> nodes = new List<Vector3>();
            foreach (var point in way.wayValues)
            {
                nodes.Add(new Vector3(point.x, 0f, point.y));
            }
            RoadModel road1 = new RoadModel(nodes);
            allroads.Add(road1);
        }

        /*List<RoadModel> allroads = new List<RoadModel>(); // all roads model
        List<List<Intersection> > tempallintersections = new List<List<Intersection> >(); // temporary list of intersections
        List<Square> squaremap = new List<Square>(); // square map

        // add first road
        Vector3[] arr = { new Vector3(608.9f, 0.0f, 541.9f), new Vector3(525.9f, 0.0f, 540.0f), new Vector3(461.8f, 0.0f, 539.7f), 
            new Vector3(388.4f, 0.0f, 538.0f), new Vector3(317.2f, 0.0f, 537.9f), new Vector3(227.0f, 0.0f, 537.1f), 
            new Vector3(154.4f, 0.0f, 537.4f), new Vector3(88.4f, 0.0f, 535.0f)};
        List<Vector3> nodes = new List<Vector3>(arr);
        RoadModel road1 = new RoadModel(nodes);
        allroads.Add(road1);
        tempallintersections.Add(new List<Intersection>());

        // add second road
        Vector3[] arr2 = { new Vector3(529.9f, 0.0f, 448.3f), new Vector3(525.9f, 0.0f, 540.0f), new Vector3(523.9f, 0.0f, 584.8f),
            new Vector3(522.1f, 0.0f, 644.7f)};
        nodes = new List<Vector3>(arr2);
        RoadModel road2 = new RoadModel(nodes);
        allroads.Add(road2);
        tempallintersections.Add(new List<Intersection>());
        
        // intersections
        // find intersections
        for (var i = 0; i < allroads.Count-1; i++)
        {
            for(var k = i+1; k < allroads.Count; k++)
            {
                for ( var n = 0; n < allroads[i].nodes.Count; n++ )
                {
                    for (var m = 0; m < allroads[k].nodes.Count; m++)
                    {
                        if(allroads[i].nodes[n] == allroads[k].nodes[m])
                        { // found an intersection!
                            float a_1, b_1, a_2, b_2, a_3, b_3, a_4, b_4;
                            // road 1
                            Vector3 tempPOS = allroads[i].tangents[n];
                            float tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
                            float temproadWidth = 10f;
                            float tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
                            float tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
                            if (tempPOS.x > 0) tempb = -1 * tempb;
                            if (tempPOS.z < 0) tempa = -1 * tempa;
                            Vector3 middle = allroads[i].nodes[n];
                            //point 1
                            Vector3 road2middle1 = new Vector3(middle.x + tempa, middle.y, middle.z + tempb);
                            //point 2
                            Vector3 road2middle2 = new Vector3(middle.x - tempa, middle.y, middle.z - tempb);
                            bool isCounterClockWise = false;
                            Vector3 temp = road2middle2 - middle;
                            if (Vector3.Angle(tempPOS, temp) > 90) // sort the vertices order
                            {
                                temp = road2middle1;
                                road2middle1 = road2middle2;
                                road2middle2 = temp;
                                isCounterClockWise = !isCounterClockWise;
                            }
                            a_1 = tempPOS.z / tempPOS.x; // line equation point 1
                            b_1 = road2middle1.z - a_1 * road2middle1.x;
                            a_2 = tempPOS.z / tempPOS.x; // line equation point 2
                            b_2 = road2middle2.z - a_2 * road2middle2.x;
                            // road 2
                            tempPOS = allroads[k].tangents[m];
                            tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
                            temproadWidth = 10f;
                            tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
                            tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
                            if (tempPOS.x > 0) tempb = -1 * tempb;
                            if (tempPOS.z < 0) tempa = -1 * tempa;
                            //middle = allroads[k].nodes[m];
                            //point 3
                            Vector3 road1middle1 = new Vector3(middle.x + tempa, middle.y, middle.z + tempb);
                            //point 4
                            Vector3 road1middle2 = new Vector3(middle.x - tempa, middle.y, middle.z - tempb);
                            temp = road1middle2 - middle;
                            if (Vector3.Angle(tempPOS, temp) > 90) // sort the vertices order
                            {
                                temp = road1middle1;
                                road1middle1 = road1middle2;
                                road1middle2 = temp;
                                isCounterClockWise = !isCounterClockWise;
                            }
                            a_3 = tempPOS.z / tempPOS.x; // line equation point 3
                            b_3 = road1middle1.z - a_3 * road1middle1.x;
                            a_4 = tempPOS.z / tempPOS.x; // line equation point 4
                            b_4 = road1middle2.z - a_4 * road1middle2.x;
                            // intersections - 4 edges of the square
                            // 1-3
                            Vector2 inter1_3 = new Vector2();
                            inter1_3.x = (b_1 - b_3) / (a_3 - a_1);
                            inter1_3.y = a_1 * inter1_3.x + b_1;
                            // 1-4
                            Vector2 inter1_4 = new Vector2();
                            inter1_4.x = (b_1 - b_4) / (a_4 - a_1);
                            inter1_4.y = a_1 * inter1_4.x + b_1;
                            // 2-3
                            Vector2 inter2_3 = new Vector2();
                            inter2_3.x = (b_3 - b_2) / (a_2 - a_3);
                            inter2_3.y = a_3 * inter2_3.x + b_3;
                            // 2-4
                            Vector2 inter2_4 = new Vector2();
                            inter2_4.x = (b_4 - b_2) / (a_2 - a_4);
                            inter2_4.y = a_4 * inter2_4.x + b_4;

                            // create Square object representing the intersection
                            Square intersection = new Square();
                            intersection.point_1 = inter1_3;
                            intersection.point_2 = inter2_3;
                            intersection.point_3 = inter2_4;
                            intersection.point_4 = inter1_4;
                            intersection.a_1_2 = a_3;
                            intersection.b_1_2 = b_3;
                            intersection.a_2_3 = a_2;
                            intersection.b_2_3 = b_2;
                            intersection.a_3_4 = a_4;
                            intersection.b_3_4 = b_4;
                            intersection.a_1_4 = a_1;
                            intersection.b_1_4 = b_1;

                            // add the square to tempsquares buffer
                            tempallintersections[i].Add(new Intersection(intersection, n, true, isCounterClockWise));
                            tempallintersections[k].Add(new Intersection(intersection, m, false, isCounterClockWise));

                        }
                    }
                }
            }
        }
        // create square map
        for (var i = 0; i < allroads.Count; i++)
        {
            // sort the tempsquares buffer
            tempallintersections[i].Sort(delegate (Intersection x, Intersection y)
            {
                return x.middleIdx - y.middleIdx;
            });
            Square finalRoadSquares = null;
            Square currentRoadSquare = null;
            int intersectionIdx = 0;
            Intersection nextInter = tempallintersections[i].Count > intersectionIdx ? tempallintersections[i][intersectionIdx++] : null;
            float temproadWidth = 10f;
            float startInterDist = nextInter!=null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth/2 : float.MaxValue;
            float endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth/2 : float.MinValue;
            for (var n = 1; n < allroads[i].RoadMiddlePoints.Length; n++)
            {
                if (allroads[i].RoadDefValuesArray[n] < startInterDist)
                {
                    // add simple square
                    if (finalRoadSquares == null)
                    {
                        finalRoadSquares = new Square(allroads[i].RoadRightPoints[n-1], allroads[i].RoadLeftPoints[n-1],
                            allroads[i].RoadLeftPoints[n], allroads[i].RoadRightPoints[n], null, null, null, null);
                        currentRoadSquare = finalRoadSquares;
                    }
                    else
                    {
                        currentRoadSquare.s_3_4 = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                            allroads[i].RoadLeftPoints[n], allroads[i].RoadRightPoints[n], currentRoadSquare, null, null, null);
                        currentRoadSquare = currentRoadSquare.s_3_4;
                    }
                    //(new GameObject($"{n}-1")).transform.position = new Vector3(currentRoadSquare.point_1.x,0f,currentRoadSquare.point_1.y);
                    //(new GameObject($"{n}-2")).transform.position = new Vector3(currentRoadSquare.point_2.x, 0f, currentRoadSquare.point_2.y);
                }
                else
                {
                    // sort square points
                    Vector2 point_1, point_2, point_3, point_4;
                    if (nextInter.isFirstRoad)
                    {
                        if (nextInter.isCounterClockWise)
                        {
                            point_1 = nextInter.square.point_2;
                            point_2 = nextInter.square.point_1;
                            point_3 = nextInter.square.point_4;
                            point_4 = nextInter.square.point_3;
                        }
                        else
                        {
                            point_1 = nextInter.square.point_1;
                            point_2 = nextInter.square.point_2;
                            point_3 = nextInter.square.point_3;
                            point_4 = nextInter.square.point_4;
                        }
                    }
                    else
                    {
                        if (nextInter.isCounterClockWise)
                        {
                            Debug.Log("isCounterClockWise");
                            point_1 = nextInter.square.point_1;
                            point_2 = nextInter.square.point_4;
                            point_3 = nextInter.square.point_3;
                            point_4 = nextInter.square.point_2;
                        }
                        else
                        {
                            Debug.Log("ClockWise");
                            point_1 = nextInter.square.point_2;
                            point_2 = nextInter.square.point_3;
                            point_3 = nextInter.square.point_4;
                            point_4 = nextInter.square.point_1;
                        }
                    }
                    // add the intersection and the next square if exists
                    if (finalRoadSquares == null)
                    {
                        if (allroads[i].RoadDefValuesArray[n-1] < startInterDist)
                        {
                            finalRoadSquares = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                                point_2, point_1, null, null, null, null);
                            currentRoadSquare = finalRoadSquares;
                            currentRoadSquare.s_3_4 = nextInter.square;
                            if (nextInter.isFirstRoad)
                            {
                                nextInter.square.s_1_2 = currentRoadSquare;
                            }
                            else
                            {
                                nextInter.square.s_2_3 = currentRoadSquare;
                            }
                        }
                        else
                        {
                            finalRoadSquares = nextInter.square;

                        }
                        while (allroads[i].RoadDefValuesArray[n] <= endInterDist)
                        {
                            n++;
                        }
                        if (n < allroads[i].RoadMiddlePoints.Length)
                        {
                            if (nextInter.isFirstRoad)
                            {
                                nextInter.square.s_3_4 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_3_4;
                            }
                            else
                            {
                                nextInter.square.s_1_4 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_2_3;
                            }
                        }
                        nextInter = tempallintersections[i].Count > intersectionIdx ? tempallintersections[i][intersectionIdx++] : null;
                        startInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth / 2 : float.MaxValue;
                        endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth / 2 : float.MinValue;
                    }
                    else
                    {
                        currentRoadSquare.s_3_4 = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                            point_2, point_1, currentRoadSquare, null, null, null);
                        currentRoadSquare = currentRoadSquare.s_3_4;
                        currentRoadSquare.s_3_4 = nextInter.square;
                        if (nextInter.isFirstRoad)
                        {
                            nextInter.square.s_1_2 = currentRoadSquare;
                        }
                        else
                        {
                            nextInter.square.s_2_3 = currentRoadSquare;
                        }
                        while (allroads[i].RoadDefValuesArray[n] <= endInterDist)
                        {
                            n++;
                        }
                        if(n < allroads[i].RoadMiddlePoints.Length)
                        {
                            if (nextInter.isFirstRoad)
                            {
                                nextInter.square.s_3_4 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_3_4;
                            }
                            else
                            {
                                nextInter.square.s_1_4 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_1_4;
                            }
                        }
                        nextInter = tempallintersections[i].Count > intersectionIdx ? tempallintersections[i][intersectionIdx++] : null;
                        startInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth / 2 : float.MaxValue;
                        endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth / 2 : float.MinValue;
                    }
                }
            }
            squaremap.Add(finalRoadSquares);
        }
        /*for (var i = 1; i < 2; i++)
        {
            Square current = squaremap[i];
            int n = 0, maxloop = 100000000;
            while (current != null && maxloop-->0)
            {
                if (current.s_1_4 == null)
                {
                    (new GameObject($"{n}-1")).transform.position = new Vector3(current.point_1.x, 0f, current.point_1.y);
                    (new GameObject($"{n}-2")).transform.position = new Vector3(current.point_2.x, 0f, current.point_2.y);
                    (new GameObject($"{n}-3")).transform.position = new Vector3(current.point_3.x, 0f, current.point_3.y);
                    (new GameObject($"{n}-4")).transform.position = new Vector3(current.point_4.x, 0f, current.point_4.y);
                    n++;
                    current = current.s_3_4;
                }
                else
                {
                    Debug.Log("intersection");
                    (new GameObject($"{n}-1")).transform.position = new Vector3(current.point_1.x, 0f, current.point_1.y);
                    (new GameObject($"{n}-2")).transform.position = new Vector3(current.point_2.x, 0f, current.point_2.y);
                    (new GameObject($"{n}-3")).transform.position = new Vector3(current.point_3.x, 0f, current.point_3.y);
                    (new GameObject($"{n}-4")).transform.position = new Vector3(current.point_4.x, 0f, current.point_4.y);
                    Debug.Log($"{current.s_1_2.point_1}, {current.s_1_2.point_2}, {current.s_1_2.point_3}, {current.s_1_2.point_4}");
                    Debug.Log($"{current.s_2_3.point_1}, {current.s_2_3.point_2}, {current.s_2_3.point_3}, {current.s_2_3.point_4}");
                    Debug.Log($"{current.s_3_4.point_1}, {current.s_3_4.point_2}, {current.s_3_4.point_3}, {current.s_3_4.point_4}");
                    Debug.Log($"{current.s_1_4.point_1}, {current.s_1_4.point_2}, {current.s_1_4.point_3}, {current.s_1_4.point_4}");
                    current = current.s_1_4;
                }
            }
            if (maxloop <= 0)
            {
                Debug.Log("max loop reached");
            }
        }*/
        /*
        // car start point 
        var rand = new System.Random();
        var startroadnum = rand.Next(2);
        var startroad = allroads[startroadnum];
        float j = startroad.TranslateInverseParamToFloat(startroad.RoadDefKeysArray[1]);
        float l= startroad.TranslateInverseParamToFloat(startroad.RoadDefKeysArray[2]);
        j = (l + j) / 2;
        Vector3 carStart, startPOS;
        startroad.GetSplineValueBoth(j, out carStart, out startPOS);
        float angle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(startPOS.x) / Mathf.Abs(startPOS.z));
        float roadWidth = 10f;
        float a = (roadWidth / 4) * Mathf.Sin(angle);
        float b = (roadWidth / 4) * Mathf.Cos(angle);
        if (startPOS.x > 0) b = -1 * b;
        if (startPOS.z < 0) a = -1 * a;
        carStart.x += a;
        carStart.z += b;
        carStart.y++;

        carLocation = new Vector2(carStart.x, carStart.z);
        carSquare = squaremap[startroadnum].s_3_4;
        #endregion

        #region view
        */
        // Terrain Creation
        TerrainData newTerrainData = new TerrainData();
        newTerrainData.heightmapResolution = 513;
        newTerrainData.baseMapResolution = 1024;
        newTerrainData.SetDetailResolution(1024,32);
        newTerrainData.size = new Vector3(1000, 600, 1000);
        GameObject newTerrainObject = Terrain.CreateTerrainGameObject(newTerrainData);

        // Road Architect Creation

        Object[] allRoadSystemObjects = GameObject.FindObjectsOfType<RoadSystem>();
        int nextCount = (allRoadSystemObjects.Length + 1);
        allRoadSystemObjects = null;

        GameObject newRoadSystemObject = new GameObject("RoadArchitectSystem" + nextCount.ToString());
        RoadSystem newRoadSystem = newRoadSystemObject.AddComponent<RoadSystem>();
        newRoadSystem.isMultithreaded = false;

        //Add road for new road system.
        //GameObject newRoad = newRoadSystem.AddRoad(false);

        GameObject masterIntersectionsObject = new GameObject("Intersections");
        masterIntersectionsObject.transform.parent = newRoadSystemObject.transform;
        
        // Create Nodes
        newRoadSystem.isAllowingRoadUpdates = false;


        List<Road> allRoadArchitectRoads = new List<Road>();
        foreach (var roadModel in allroads) {
            Road road = RoadAutomation.CreateRoadProgrammatically(newRoadSystem, ref roadModel.nodes);
            road.isSavingTerrainHistoryOnDisk = false;
            allRoadArchitectRoads.Add(road);
        }
        
        foreach (var road in allRoadArchitectRoads)
        {
            RoadAutomation.CreateIntersectionsProgrammaticallyForRoad(road);
        }
        
        newRoadSystem.isAllowingRoadUpdates = true;

        newRoadSystem.UpdateAllRoads();
        /*
        //create car
        float yRotation = tangent2Yrotation(startPOS);
        Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);
        GameObject carClone = Instantiate(carPrefab, carStart, targetRotation);

        // set the car in other scripts as needed
        //Speed[] allSpeedObjects = GameObject.FindObjectsOfType<Speed>();
        //allSpeedObjects[0].vehicleBody = carClone.GetComponent<Rigidbody>();
        carClone.AddComponent<Speed>();
        Speedometer[] allSpeedometerObjects = GameObject.FindObjectsOfType<Speedometer>();
        allSpeedometerObjects[0].vehicleBody = carClone.GetComponent<Rigidbody>();
        DriftCamera[] allDriftCameraObjects = GameObject.FindObjectsOfType<DriftCamera>();
        allDriftCameraObjects[0].lookAtTarget = carClone.transform.Find("CamRig").transform.Find("CamLookAtTarget").transform;
        allDriftCameraObjects[0].positionTarget = carClone.transform.Find("CamRig").transform.Find("CamPosition").transform;
        allDriftCameraObjects[0].sideView = carClone.transform.Find("CamRig").transform.Find("CamSidePosition").transform;

        /*int kFinalCount = road1.RoadDefKeysArray.Length;
        for (var kCount = 0; kCount < kFinalCount; kCount++)
        {
            var i = road1.TranslateInverseParamToFloat(road1.RoadDefKeysArray[kCount]);
            Vector3 myVect, tempPOS;
            road1.GetSplineValueBoth(i, out myVect, out tempPOS);
            (new GameObject($"road1 {kCount}: {myVect.ToString()}")).transform.position = myVect;
            float tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
            float temproadWidth = 10f;
            float tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
            float tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
            if (tempPOS.x > 0) tempb = -1 * tempb;
            if (tempPOS.z < 0) tempa = -1 * tempa;
            myVect.x += tempa;
            myVect.z += tempb;
            (new GameObject($"road1 {kCount}-right: {myVect.ToString()}")).transform.position = myVect;
            myVect.x -= 2 * tempa;
            myVect.z -= 2 * tempb;
            (new GameObject($"road1 {kCount}-left: {myVect.ToString()}")).transform.position = myVect;
        }
        kFinalCount = road2.RoadDefKeysArray.Length;
        for (var kCount = 0; kCount < kFinalCount; kCount++)
        {
            var i = road2.TranslateInverseParamToFloat(road2.RoadDefKeysArray[kCount]);
            Vector3 myVect, tempPOS;
            road2.GetSplineValueBoth(i, out myVect, out tempPOS);
            (new GameObject($"road2 {kCount}: {myVect.ToString()}")).transform.position = myVect;
            float tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
            float temproadWidth = 10f;
            float tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
            float tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
            if (tempPOS.x > 0) tempb = -1 * tempb;
            if (tempPOS.z < 0) tempa = -1 * tempa;
            myVect.x += tempa;
            myVect.z += tempb;
            (new GameObject($"road2 {kCount}-right: {myVect.ToString()}")).transform.position = myVect;
            myVect.x -= 2 * tempa;
            myVect.z -= 2 * tempb;
            (new GameObject($"road2 {kCount}-left: {myVect.ToString()}")).transform.position = myVect;
        }
        
        float a_1, b_1, a_2, b_2, a_3, b_3, a_4, b_4;
        var i = 1;
        // road 1
        Vector3 tempPOS = road1.tangents[i];
        float tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
        float temproadWidth = 10f;
        float tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
        float tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
        if (tempPOS.x > 0) tempb = -1 * tempb;
        if (tempPOS.z < 0) tempa = -1 * tempa;
        Vector3 myVect = new Vector3(road1.nodes[i].x, road1.nodes[i].y, road1.nodes[i].z);
        //point 1
        myVect.x += tempa;
        myVect.z += tempb;
        a_1 = tempPOS.z / tempPOS.x;
        b_1 = myVect.z - a_1 * myVect.x;
        (new GameObject($"right1: {myVect.ToString()}")).transform.position = myVect;
        //point 2
        myVect.x -= 2 * tempa;
        myVect.z -= 2 * tempb;
        a_2 = tempPOS.z / tempPOS.x;
        b_2 = myVect.z - a_2 * myVect.x;
        (new GameObject($"left1: {myVect.ToString()}")).transform.position = myVect;
        // road 2
        tempPOS = road2.tangents[i];
        tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
        temproadWidth = 10f;
        tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
        tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
        if (tempPOS.x > 0) tempb = -1 * tempb;
        if (tempPOS.z < 0) tempa = -1 * tempa;
        myVect = new Vector3(road2.nodes[i].x, road2.nodes[i].y, road2.nodes[i].z);
        //point 3
        myVect.x += tempa;
        myVect.z += tempb;
        a_3 = tempPOS.z / tempPOS.x;
        b_3 = myVect.z - a_3 * myVect.x;
        (new GameObject($"right2: {myVect.ToString()}")).transform.position = myVect;
        //point 4
        myVect.x -= 2 * tempa;
        myVect.z -= 2 * tempb;
        a_4 = tempPOS.z / tempPOS.x;
        b_4 = myVect.z - a_4 * myVect.x;
        (new GameObject($"left2: {myVect.ToString()}")).transform.position = myVect;
        // intersections
        // 1-3
        myVect.x = (b_1 - b_3) / (a_3 - a_1);
        myVect.z = a_1 * myVect.x + b_1;
        (new GameObject($"iter1-3: {myVect.ToString()}")).transform.position = myVect;
        // 1-4
        myVect.x = (b_1 - b_4) / (a_4 - a_1);
        myVect.z = a_1 * myVect.x + b_1;
        (new GameObject($"iter1-4: {myVect.ToString()}")).transform.position = myVect;
        // 2-3
        myVect.x = (b_3 - b_2) / (a_2 - a_3);
        myVect.z = a_3 * myVect.x + b_3;
        (new GameObject($"iter2-3: {myVect.ToString()}")).transform.position = myVect;
        // 2-4
        myVect.x = (b_4 - b_2) / (a_2 - a_4);
        myVect.z = a_4 * myVect.x + b_4;
        (new GameObject($"iter2-4: {myVect.ToString()}")).transform.position = myVect;
        */
        #endregion
    }
}
