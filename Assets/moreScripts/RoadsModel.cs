using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector2 middle = new Vector2((point_1.x + point_2.x) / 2, (point_1.y + point_2.y) / 2);
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
    public int middleIdx;
    public bool isFirstRoad; // first road corresponds to: 1_2 back 3_4 front. seconnd road corresponds to: 1_4 back 2_3 front.
    public bool isCounterClockWise;
    public Intersection(Square square, int middleIdx, bool isFirstRoad, bool isCounterClockWise)
    {
        this.square = square;
        this.middleIdx = middleIdx;
        this.isFirstRoad = isFirstRoad;
        this.isCounterClockWise = isCounterClockWise;
    }
}

public class RoadsModel
{
    private Vector2 carLocation;
    private Square carSquare;
    private List<SplineModel> allroads; // all splines model
    private List<Square> squaremap; // square map
    private List<List<int>> notTrivialSquaresIdx; // intersection squares when current road is the second
    private static float triangleArea(Vector2 a, Vector2 b, Vector2 c)
    {
        //Debug.Log($"before - a:{a}, b:{b}, c:{c}");
        b = b - a;
        c = c - a;
        a = a - a;
        // calculate area
        var ab_dist = Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        var ab_slope = (a.y - b.y) / (a.x - b.x);
        var ab_y_intercept = a.y - ab_slope * a.x;
        var c_perpendicular_slope = -1f / ab_slope;
        var c_perpendicular_y_intercept = c.y - c_perpendicular_slope * c.x;
        var intersection_x = (c_perpendicular_y_intercept - ab_y_intercept) / (ab_slope - c_perpendicular_slope);
        var intersection_y = ab_slope * intersection_x + ab_y_intercept;
        var c_height = Mathf.Sqrt((c.x - intersection_x) * (c.x - intersection_x) + (c.y - intersection_y) * (c.y - intersection_y));
        var abc_area = ab_dist * c_height / 2;
        return abc_area;
    }

    private static bool isInsisdeTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        var abc_area = triangleArea(a, b, c);
        var pbc_area = triangleArea(point, b, c);
        var apc_area = triangleArea(a, point, c);
        var abp_area = triangleArea(a, b, point);
        // is approximately equal?
        return Mathf.Abs(abc_area - pbc_area - apc_area - abp_area) < 0.00001f;
    }

    private Square findSquare(Vector2 point)
    {
        for (var i = 0; i < squaremap.Count; i++)
        {
            Square current = squaremap[i];
            int maxloop = 10000; // avoid endless loop
            int squareCounter = 0;
            int ntsiIdx = 0;
            int nextSpecialSquare = notTrivialSquaresIdx[i].Count > ntsiIdx ? notTrivialSquaresIdx[i][ntsiIdx++] : -1;
            while (current != null && maxloop-- > 0)
            {
                if (squareCounter != nextSpecialSquare)
                {
                    // check if point inside current square
                    if (isInsisdeTriangle(point, current.point_1, current.point_2, current.point_3) || isInsisdeTriangle(point, current.point_1, current.point_3, current.point_4))
                    {
                        return current;
                    }
                    current = current.s_3_4;
                }
                else
                {
                    nextSpecialSquare = notTrivialSquaresIdx[i].Count > ntsiIdx ? notTrivialSquaresIdx[i][ntsiIdx++] : -1;
                    // check if point inside current square
                    if (isInsisdeTriangle(point, current.point_1, current.point_2, current.point_3) || isInsisdeTriangle(point, current.point_1, current.point_3, current.point_4))
                    {
                        return current;
                    }
                    current = current.s_2_3;
                }
                squareCounter++;
            }
            if (maxloop <= 0)
            {
                Debug.LogError("max loop reached in findSquare");
            }
        }
        // not found
        return null;
    }

    public void MoveCar(float x, float z)
    {
        Vector2 oldLocation = carLocation;
        carLocation = new Vector2(x, z);
        if (carSquare == null)
        {
            Debug.Log("Car outside of the road");
            return;
        }
        carSquare = carSquare.calculate(oldLocation, carLocation);
    }
    public RoadsModel(List<List<Vector3>> allRoadsNodes)
    {
        allroads = new List<SplineModel>(); // all splines model
        List<List<Intersection>> tempallintersections = new List<List<Intersection>>(); // temporary list of intersections
        squaremap = new List<Square>(); // square map
        notTrivialSquaresIdx = new List<List<int>>();

        // add all roads
        foreach (List<Vector3> nodes in allRoadsNodes)
        {
            SplineModel road = new SplineModel(nodes);
            // debug 
            //Debug.Log($"{nodes[nodes.Count - 1]} {road.tangents[road.tangents.Length - 1]}");
            allroads.Add(road);
            tempallintersections.Add(new List<Intersection>());
            notTrivialSquaresIdx.Add(new List<int>());
        }
        // intersections
        // find intersections
        for (var i = 0; i < allroads.Count - 1; i++)
        {
            for (var k = i + 1; k < allroads.Count; k++)
            {
                for (var n = 0; n < allroads[i].nodes.Count; n++)
                {
                    for (var m = 0; m < allroads[k].nodes.Count; m++)
                    {
                        if (allroads[i].nodes[n] == allroads[k].nodes[m])
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
                            if (Vector3.Angle(allroads[k].tangents[m], temp) > 90) // sort the vertices order
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
                            if (Vector3.Angle(allroads[i].tangents[n], temp) > 90) // sort the vertices order
                            {
                                temp = road1middle1;
                                road1middle1 = road1middle2;
                                road1middle2 = temp;
                                //isCounterClockWise = !isCounterClockWise;
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

                            #region debug
                            //if(float.IsNaN(road1middle1.x) || float.IsNaN(road1middle1.y) || float.IsNaN(road1middle2.x) || float.IsNaN(road1middle2.y) || float.IsNaN(road2middle1.x) || float.IsNaN(road2middle1.y) || float.IsNaN(road2middle2.x) || float.IsNaN(road2middle2.y) ||
                            //float.IsNaN(inter1_3.x) || float.IsNaN(inter2_4.x) || float.IsNaN(inter2_3.x) || float.IsNaN(inter1_4.x) || float.IsNaN(inter1_3.y) || float.IsNaN(inter2_4.y) || float.IsNaN(inter2_3.y) || float.IsNaN(inter1_4.y))
                            //if(i==0 && n==3 && k == 1 && m == allroads[k].nodes.Count-1){
                                //Debug.Log($"i: {i}, n: {n}, k: {k}, m: {m}");
                                //Debug.Log($"middle: {middle}");
                                //Debug.LogError("NaN");
                                //tempPOS = allroads[i].tangents[n];
                                //tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
                                //tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
                                //tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
                                //if (tempPOS.x > 0) tempb = -1 * tempb;
                                //if (tempPOS.z < 0) tempa = -1 * tempa;
                                //point 1
                                //road2middle1 = new Vector3(middle.x + tempa, middle.y, middle.z + tempb);
                                //point 2
                                //road2middle2 = new Vector3(middle.x - tempa, middle.y, middle.z - tempb);
                                //(new GameObject($"road2middle1")).transform.position = road2middle1;
                                //(new GameObject($"road2middle2")).transform.position = road2middle2;
                                //Debug.Log($"middle: {middle}, tempPOS1: {allroads[i].tangents[n]}, tempPOS2: {tempPOS}, tempangle: {tempangle}, (tempa,tempb): ({tempa},{tempb})");
                                //Debug.Log($"inter1_3: {inter1_3}, inter1_4: {inter1_4}, inter2_3: {inter2_3}, inter2_4: {inter2_4}");
                                //Debug.Log($"road1middle1: {road1middle1}, road1middle2: {road1middle2}, road2middle1: {road2middle1}, road2middle2: {road2middle2}");
                            //}
                            #endregion
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
            float startInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth / 2 : float.MaxValue;
            float endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth / 2 : float.MinValue;
            int squareCounter = 0;
            for (var n = 1; n < allroads[i].RoadMiddlePoints.Length; n++)
            {
                if (allroads[i].RoadDefValuesArray[n] < startInterDist-1)
                {
                    // add simple square
                    if (finalRoadSquares == null)
                    {
                        finalRoadSquares = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                            allroads[i].RoadLeftPoints[n], allroads[i].RoadRightPoints[n], null, null, null, null);
                        currentRoadSquare = finalRoadSquares;
                    }
                    else
                    {
                        currentRoadSquare.s_3_4 = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                            allroads[i].RoadLeftPoints[n], allroads[i].RoadRightPoints[n], currentRoadSquare, null, null, null);
                        currentRoadSquare = currentRoadSquare.s_3_4;
                    }
                    squareCounter++;
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
                            //Debug.Log("isCounterClockWise");
                            point_1 = nextInter.square.point_1;
                            point_2 = nextInter.square.point_4;
                            point_3 = nextInter.square.point_3;
                            point_4 = nextInter.square.point_2;
                        }
                        else
                        {
                            //Debug.Log("ClockWise");
                            point_1 = nextInter.square.point_4;
                            point_2 = nextInter.square.point_1;
                            point_3 = nextInter.square.point_2;
                            point_4 = nextInter.square.point_3;
                        }
                    }
                    // add the intersection and the next square if exists
                    if (finalRoadSquares == null)
                    {
                        if (allroads[i].RoadDefValuesArray[n - 1] < startInterDist-1)
                        {
                            finalRoadSquares = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                                point_2, point_1, null, null, null, null);
                            squareCounter++;
                            currentRoadSquare = finalRoadSquares;
                            currentRoadSquare.s_3_4 = nextInter.square;
                            if (nextInter.isFirstRoad)
                            {
                                nextInter.square.s_1_2 = currentRoadSquare;
                            }
                            else
                            {
                                nextInter.square.s_1_4 = currentRoadSquare;
                            }
                        }
                        else
                        {
                            finalRoadSquares = nextInter.square;

                        }
                        if (!nextInter.isFirstRoad) { notTrivialSquaresIdx[i].Add(squareCounter); }
                        squareCounter++;
                        while (n < allroads[i].RoadMiddlePoints.Length && allroads[i].RoadDefValuesArray[n] <= endInterDist+1)
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
                                nextInter.square.s_2_3 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_2_3;
                            }
                            squareCounter++;
                        }
                        nextInter = tempallintersections[i].Count > intersectionIdx ? tempallintersections[i][intersectionIdx++] : null;
                        startInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth / 2 : float.MaxValue;
                        endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth / 2 : float.MinValue;
                    }
                    else
                    {
                        currentRoadSquare.s_3_4 = new Square(allroads[i].RoadRightPoints[n - 1], allroads[i].RoadLeftPoints[n - 1],
                            point_2, point_1, currentRoadSquare, null, null, null);
                        squareCounter++;
                        currentRoadSquare = currentRoadSquare.s_3_4;
                        currentRoadSquare.s_3_4 = nextInter.square;
                        if (nextInter.isFirstRoad)
                        {
                            nextInter.square.s_1_2 = currentRoadSquare;
                        }
                        else
                        {
                            nextInter.square.s_1_4 = currentRoadSquare;
                            notTrivialSquaresIdx[i].Add(squareCounter);
                            //if (i == 1 && squareCounter == 83)
                            //{
                            //    Debug.Log($"nextInter.isCounterClockWise: {nextInter.isCounterClockWise}, tangent1: {allroads[0].tangents[3]}, tangent2: {allroads[i].tangents[allroads[i].tangents.Length-1]}");
                            //}
                        }
                        squareCounter++;
                        while (n < allroads[i].RoadMiddlePoints.Length && allroads[i].RoadDefValuesArray[n] <= endInterDist+1)
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
                                nextInter.square.s_2_3 = new Square(point_4, point_3, allroads[i].RoadLeftPoints[n],
                                    allroads[i].RoadRightPoints[n], nextInter.square, null, null, null);
                                currentRoadSquare = nextInter.square.s_2_3;
                            }
                            squareCounter++;
                        }
                        nextInter = tempallintersections[i].Count > intersectionIdx ? tempallintersections[i][intersectionIdx++] : null;
                        startInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] - temproadWidth / 2 : float.MaxValue;
                        endInterDist = nextInter != null ? allroads[i].distances[nextInter.middleIdx] + temproadWidth / 2 : float.MinValue;
                    }
                }
            }
            squaremap.Add(finalRoadSquares);
        }
        #region debug
        /*for (var i = 0; i < allroads.Count; i++)
        {
            Square current = squaremap[i];
            int maxloop = 1000;
            int squareCounter = 0;
            int ntsiIdx = 0;
            int nextSpecialSquare = notTrivialSquaresIdx[i].Count > ntsiIdx ? notTrivialSquaresIdx[i][ntsiIdx++] : -1;
            while (current != null && maxloop-->0)
            {
                if (squareCounter != nextSpecialSquare)
                {
                    (new GameObject($"{i}-{squareCounter}-1")).transform.position = new Vector3(current.point_1.x, 0f, current.point_1.y);
                    (new GameObject($"{i}-{squareCounter}-2")).transform.position = new Vector3(current.point_2.x, 0f, current.point_2.y);
                    (new GameObject($"{i}-{squareCounter}-3")).transform.position = new Vector3(current.point_3.x, 0f, current.point_3.y);
                    (new GameObject($"{i}-{squareCounter}-4")).transform.position = new Vector3(current.point_4.x, 0f, current.point_4.y);
                    current = current.s_3_4;
                }
                else
                {
                    nextSpecialSquare = notTrivialSquaresIdx[i].Count > ntsiIdx ? notTrivialSquaresIdx[i][ntsiIdx++] : -1;
                    //Debug.Log($"road {i} special intersection");
                    (new GameObject($"{i}-{squareCounter}-1")).transform.position = new Vector3(current.point_1.x, 0f, current.point_1.y);
                    (new GameObject($"{i}-{squareCounter}-2")).transform.position = new Vector3(current.point_2.x, 0f, current.point_2.y);
                    (new GameObject($"{i}-{squareCounter}-3")).transform.position = new Vector3(current.point_3.x, 0f, current.point_3.y);
                    (new GameObject($"{i}-{squareCounter}-4")).transform.position = new Vector3(current.point_4.x, 0f, current.point_4.y);
                    //Debug.Log($"{current.s_1_2.point_1}, {current.s_1_2.point_2}, {current.s_1_2.point_3}, {current.s_1_2.point_4}");
                    //Debug.Log($"{current.s_2_3.point_1}, {current.s_2_3.point_2}, {current.s_2_3.point_3}, {current.s_2_3.point_4}");
                    //Debug.Log($"{current.s_3_4.point_1}, {current.s_3_4.point_2}, {current.s_3_4.point_3}, {current.s_3_4.point_4}");
                    //Debug.Log($"{current.s_1_4.point_1}, {current.s_1_4.point_2}, {current.s_1_4.point_3}, {current.s_1_4.point_4}");
                    current = current.s_2_3;
                }
                squareCounter++;
            }
            if (maxloop <= 0)
            {
                Debug.LogError("max loop reached");
            }
        }*/
        #endregion
    }
    public void setCarStartingPoint(float carStart_x, float carStart_z)
    {
        // car start point
        carLocation = new Vector2(carStart_x, carStart_z);
        carSquare = findSquare(carLocation);
        #region debug
        //Debug.Log($"carLocation:{carLocation}, carSquare.point_1:{carSquare.point_1}, carSquare.point_2:{carSquare.point_2}, carSquare.point_3:{carSquare.point_3}, carSquare.point_4:{carSquare.point_4}");
        //Debug.Log(isInsisdeTriangle(carLocation, carSquare.point_1, carSquare.point_2, carSquare.point_3) || isInsisdeTriangle(carLocation, carSquare.point_1, carSquare.point_3, carSquare.point_4));
        //Debug.Log($"{triangleArea(carSquare.point_1, carSquare.point_3, carSquare.point_4)} {triangleArea(carLocation, carSquare.point_1, carSquare.point_3)} {triangleArea(carLocation, carSquare.point_1, carSquare.point_4)} {triangleArea(carLocation, carSquare.point_3, carSquare.point_4)}");
        (new GameObject($"carSquare-1")).transform.position = new Vector3(carSquare.point_1.x, 0f, carSquare.point_1.y);
        (new GameObject($"carSquare-2")).transform.position = new Vector3(carSquare.point_2.x, 0f, carSquare.point_2.y);
        (new GameObject($"carSquare-3")).transform.position = new Vector3(carSquare.point_3.x, 0f, carSquare.point_3.y);
        (new GameObject($"carSquare-4")).transform.position = new Vector3(carSquare.point_4.x, 0f, carSquare.point_4.y);
        //(new GameObject($"carLocation")).transform.position = new Vector3(carLocation.x, 0f, carLocation.y);
        #endregion
    }
}
