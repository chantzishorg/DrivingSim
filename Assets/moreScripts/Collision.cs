using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision 
{
    private static (Vector2, Vector2, Vector2, Vector2) caluclateRectangle(Vector2 centerPoint, Vector2 vector, float width, float length)
    {
        // vector_m = slope of the vector
        double vector_m;
        // check if vector_x isnt zero
        if (Mathf.Abs(vector.x) > 0.00005)
        {
            vector_m = vector.y / vector.x;
        }
        else vector_m = double.PositiveInfinity;
        //calucate equation, y=mx+b , b=y-mx
        // double vector_b = centerPoint.y - vector_m * centerPoint.x;
        //calucate equation line perpendicular that throught the center point  
        double m;
        if (double.IsInfinity(vector_m))
        {
            m = 0;
        }
        else if (System.Math.Abs(vector_m) > 0.00005)
        {
            m = -1 / vector_m;
        }
        else
        {
            vector_m = 0;
            m = double.PositiveInfinity;
        }
        // double b = centerPoint.y - m * centerPoint.x;
        // point on the line
        // float yPoint;
        // yLine is y=mx+b
        //yPoint = m * centerPoint.x + b;
        float distance = width / 2;
        // find two middle points on two lines of rectangle
        var twoMiddlePoints = TwoPoints(centerPoint, distance, m);
        Vector2 firstMiddlePoint = twoMiddlePoints.Item1;
        Vector2 secondMiddlePoint = twoMiddlePoints.Item2;
        distance = length / 2;
        //two corners of the rectangle
        var twoPointsOfRectangle = TwoPoints(firstMiddlePoint, distance, vector_m);
        var otherPointsOfRectangle = TwoPoints(secondMiddlePoint, distance, vector_m);
        // lentghs: pointRectangle[0]_pointRectangle[1], pointRectangle[2]_pointRectangle[3]
        // widths: pointRectangle[0]_pointRectangle[2], pointRectangle[1]_pointRectangle[3]
        return (twoPointsOfRectangle.Item1, twoPointsOfRectangle.Item2, otherPointsOfRectangle.Item1, otherPointsOfRectangle.Item2);
    }
    // function find points at a given distance on a line of given slope
    private static (Vector2, Vector2) TwoPoints(Vector2 centerPoint, float distance, double m)
    {
        // p1 and p2 there are the points of a given distance on a line 
        Vector2 p1 = new Vector2();
        Vector2 p2 = new Vector2();
        // Slope is 0, the equation y=number
        if (m == 0)
        {
            p1.x = centerPoint.x + distance;
            p1.y = centerPoint.y;
            p2.x = centerPoint.x - distance;
            p2.y = centerPoint.y;
        }

        // If slope is infinite, x=number
        else if (double.IsInfinity(m))
        {
            p1.x = centerPoint.x;
            p1.y = centerPoint.y + distance;
            p2.x = centerPoint.x;
            p2.y = centerPoint.y - distance;
        }
        else
        {
            /*
            first equation: (y-y0)^2+(x-x0)^2=distance^2
            second equation: y= y0+m(x-x0)
            from two equation get: x=x0+-sqrt(d^2/1+m^2), y=y0+-sqrt(d^2/1+m^2)
            */
            double dx = distance / System.Math.Sqrt(1 + (m * m));
            double dy = m * dx;
            p1.x = centerPoint.x + (float)dx;
            p1.y = centerPoint.y + (float)dy;
            p2.x = centerPoint.x - (float)dx;
            p2.y = centerPoint.y - (float)dy;
        }
        return (p1, p2);
    }

    // function that find the instersction point between two lines 
    private static Vector2 PointIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        // Line AB : a1x + b1y = c1
        double a1 = B.y - A.y;
        double b1 = A.x - B.x;
        double c1 = a1 * (A.x) + b1 * (A.y);

        // Line CD : a2x + b2y = c2
        double a2 = D.y - C.y;
        double b2 = C.x - D.x;
        double c2 = a2 * (C.x) + b2 * (C.y);

        double determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            // The lines are parallel. This is simplified
            // by returning a pair of FLT_MAX
            return new Vector2(float.NaN, float.NaN);
        }
        else
        {
            double x = (b2 * c1 - b1 * c2) / determinant;
            double y = (a1 * c2 - a2 * c1) / determinant;
            return new Vector2((float)x, (float)y);
        }
    }
    // check if the point between other two points on line
    private static bool BetweenTwoPoints(Vector2 p1, Vector2 p2, Vector2 point)
    {
        // Check whether p is on the line or not
        if (point.x <= Mathf.Max(p1.x, p2.x)
            && point.x >= Mathf.Min(p1.x, p2.x)
            && (point.y <= Mathf.Max(p1.y, p2.y)
                && point.y >= Mathf.Min(p1.y, p2.y)))
            return true;

        return false;
    }
    // check whether two bounded lines intersects
    private static bool boundedIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
    {
        var p = PointIntersection(A, B, C, D);
        return BetweenTwoPoints(A, B, p) && BetweenTwoPoints(C, D, p);
    }

    private static bool crossRectangles((Vector2, Vector2, Vector2, Vector2) rectangle1, (Vector2, Vector2, Vector2, Vector2) rectangle2)
    {
        // check for every two points from rectangle1 and rectangle 2 if the lines intersection
        // if line intersection check if the point between two corners of the rectangle
        (Vector2, Vector2)[] rect1 = { (rectangle1.Item1, rectangle1.Item2), (rectangle1.Item1, rectangle1.Item3), (rectangle1.Item3, rectangle1.Item4), (rectangle1.Item2, rectangle1.Item4) };
        (Vector2, Vector2)[] rect2 = { (rectangle2.Item1, rectangle2.Item2), (rectangle2.Item1, rectangle2.Item3), (rectangle2.Item3, rectangle2.Item4), (rectangle2.Item2, rectangle2.Item4) };
        foreach (var side1 in rect1)
        {
            foreach (var side2 in rect2)
            {
                if (boundedIntersect(side1.Item1, side1.Item2, side2.Item1, side2.Item2))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool isIntersection(Vector2 p1, Vector2 p2, Vector2 vector1, Vector2 vector2)
    {
        var rect1 = caluclateRectangle(p1, vector1, 1.896f, 4.375f);
        var rect2 = caluclateRectangle(p2, vector2, 1.896f, 4.375f);
        return crossRectangles(rect1, rect2);
    }

}
