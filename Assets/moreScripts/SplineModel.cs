using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RootUtils
{
    // not clear. just returns t for the default _k1=0, _k2=1 values
    /// <summary>
    /// Smooths the input parameter t.
    /// If less than k1 ir greater than k2, it uses a sin.
    /// Between k1 and k2 it uses linear interp.
    /// </summary>
    public static float Ease(float _t, float _k1 = 0f, float _k2 = 1f)
    {
        float f;
        float s;


        f = _k1 * 2 / Mathf.PI + _k2 - _k1 + (1.0f - _k2) * 2 / Mathf.PI;

        if (_t < _k1)
        {
            Debug.Log($"_t < _k1(0): {_t}");
            s = _k1 * (2 / Mathf.PI) * (Mathf.Sin((_t / _k1) * Mathf.PI / 2 - Mathf.PI / 2) + 1);
        }
        else if (_t <= _k2)
        {
            s = (2 * _k1 / Mathf.PI + _t - _k1);
        }
        else
        {
            Debug.Log($"_t > _k2(1): {_t}");
            s = 2 * _k1 / Mathf.PI + _k2 - _k1 + ((1 - _k2) * (2 / Mathf.PI)) * Mathf.Sin(((_t - _k2) / (1.0f - _k2)) * Mathf.PI / 2);
        }

        return (s / f);
    }
    #region "Float comparisons"
    public static bool IsApproximately(float _a, float _b, float _tolerance = 0.01f)
    {
        return Mathf.Abs(_a - _b) < _tolerance;
    }
    #endregion
}

public class SplineModel
{
    #region model
    //nodes
    public List<Vector3> nodes;
    public float[] times;
    public Vector3[] tangents;
    public float[] distances;
    // a lot of points along the middle of the road
    public int[] RoadDefKeysArray; // times of points (represented by ints)
    public float[] RoadDefValuesArray; // distances of each point from the start of road
    public Vector2[] RoadMiddlePoints;
    public Vector2[] RoadRightPoints;
    public Vector2[] RoadLeftPoints;
    //public Queue<>;



    public SplineModel(List<Vector3> nodes)
    {
        this.nodes = nodes;
        calculateTimes();
    }
    public Vector3 GetSplineValue(float _value, bool _isTangent = false)
    {
        int index;
        int idx = -1;


        if (nodes.Count == 0)
        {
            return default(Vector3);
        }
        if (nodes.Count == 1)
        {
            return nodes[0];
        }

        // This Code was outcommented, but it takes care about values above and below 0f and 1f and clamping them.
        // This Fixes the Bug descripted by embeddedt/RoadArchitect/issues/4 
        if (RootUtils.IsApproximately(_value, 0f, 0.00001f))
        {
            if (_isTangent)
            {
                return tangents[0];
            }
            else
            {
                return nodes[0];
            }
        }
        else if (RootUtils.IsApproximately(_value, 1f, 0.00001f) || _value > 1f)
        {
            if (_isTangent)
            {
                return tangents[nodes.Count - 1];
            }
            else
            {
                return nodes[nodes.Count - 1];
            }
        }
        else
        {
            // ?
            //RootUtils.IsApproximately(_value, 1f, 0.00001f);

            for (index = 0; index < nodes.Count; index++)
            {
                if (index == nodes.Count - 1)
                {
                    idx = index - 1;
                    break;
                }
                if (times[index] > _value)
                {
                    idx = index - 1;
                    break;
                }
            }
            if (idx < 0)
            {
                idx = 0;
            }
        }

        float param = (_value - times[idx]) / (times[idx + 1] - times[idx]);
        param = RootUtils.Ease(param, 0, 1);
        return GetHermiteInternal(idx, param, _isTangent);
    }

    public Vector3 GetSplineValueSkipOpt(float _value, bool _isTangent = false)
    {
        int index;
        int idx = -1;

        if (nodes.Count == 0)
        {
            return default(Vector3);
        }
        if (nodes.Count == 1)
        {
            return nodes[0];
        }

        for (index = 1; index < nodes.Count; index++)
        {
            if (index == nodes.Count - 1)
            {
                idx = index - 1;
                break;
            }
            if (times[index] > _value)
            {
                idx = index - 1;
                break;
            }
        }
        if (idx < 0)
        {
            idx = 0;
        }

        float param = (_value - times[idx]) / (times[idx + 1] - times[idx]);
        param = RootUtils.Ease(param, 0, 1);
        return GetHermiteInternal(idx, param, _isTangent);
    }

    public void GetSplineValueBoth(float _value, out Vector3 _vect1, out Vector3 _vect2)
    {
        int index;
        int idx = -1;
        int nodeCount = nodes.Count;

        if (_value < 0f)
        {
            _value = 0f;
        }
        if (_value > 1f)
        {
            _value = 1f;
        }


        if (nodeCount == 0)
        {
            _vect1 = default(Vector3);
            _vect2 = default(Vector3);
            return;
        }

        if (nodeCount == 1)
        {
            //if (nodes[0]!=null)
            {
                _vect1 = nodes[0];
                _vect2 = default(Vector3);
            }
            /*else
            {
                _vect1 = default(Vector3);
                _vect2 = default(Vector3);
            }*/
            return;
        }


        // This Code was outcommented, but it takes care about values above and below 0f and 1f and clamping them.
        // This code needs to be reevealuated if this isn't taken care of by the function above this one. GetSplineValue() 
        // part of embeddedt/RoadArchitect/issues/4 ?
        if (RootUtils.IsApproximately(_value, 1f, 0.0001f))
        {
            _vect1 = nodes[nodes.Count - 1];
            _vect2 = tangents[nodes.Count - 1];
            return;
        }
        else if (RootUtils.IsApproximately(_value, 0f, 0.0001f))
        {
            _vect1 = nodes[0];
            _vect2 = tangents[0];
            return;
        }
        // Do Note: This does prevent EdgeObjects from being placed before or after
        // 0f/1f on the Spline, but also causes the EdgeObject to be placed at the same Position at the End of the Spline.

        for (index = 1; index < nodeCount; index++)
        {
            if (index == nodeCount - 1)
            {
                idx = index - 1;
                break;
            }
            if (times[index] > _value)
            {
                idx = index - 1;
                break;
            }
        }
        if (idx < 0)
        {
            idx = 0;
        }

        float param = (_value - times[idx]) / (times[idx + 1] - times[idx]);
        param = RootUtils.Ease(param, 0, 1);

        _vect1 = GetHermiteInternal(idx, param, false);
        _vect2 = GetHermiteInternal(idx, param, true);
    }

    private Vector3 GetHermiteInternal(int _i, double _t, bool _isTangent = false)
    {
        double t2, t3;
        float BL0, BL1, BL2, BL3, tension;

        if (!_isTangent)
        {
            t2 = _t * _t;
            t3 = t2 * _t;
        }
        else
        {
            t2 = _t * _t;
            _t = _t * 2.0;
            t2 = t2 * 3.0;
            //Prevent compiler error.
            t3 = 0;
        }

        //Vectors:
        Vector3 P0 = nodes[NGI(_i, NI[0])];
        Vector3 P1 = nodes[NGI(_i, NI[1])];
        Vector3 P2 = nodes[NGI(_i, NI[2])];
        Vector3 P3 = nodes[NGI(_i, NI[3])];

        //Tension:
        tension = 0.5f;

        //Tangents:
        Vector3 xVect1 = (P1 - P2) * tension;
        Vector3 xVect2 = (P3 - P0) * tension;
        float road_magnitudeThreshold = 300f;
        float tMaxMag = road_magnitudeThreshold;

        if (Vector3.Distance(P1, P3) > tMaxMag)
        {
            if (xVect1.magnitude > tMaxMag)
            {
                xVect1 = Vector3.ClampMagnitude(xVect1, tMaxMag);
            }
            if (xVect2.magnitude > tMaxMag)
            {
                xVect2 = Vector3.ClampMagnitude(xVect2, tMaxMag);
            }
        }
        else if (Vector3.Distance(P0, P2) > tMaxMag)
        {
            if (xVect1.magnitude > tMaxMag)
            {
                xVect1 = Vector3.ClampMagnitude(xVect1, tMaxMag);
            }
            if (xVect2.magnitude > tMaxMag)
            {
                xVect2 = Vector3.ClampMagnitude(xVect2, tMaxMag);
            }
        }


        if (!_isTangent)
        {
            BL0 = (float)(CM[0] * t3 + CM[1] * t2 + CM[2] * _t + CM[3]);
            BL1 = (float)(CM[4] * t3 + CM[5] * t2 + CM[6] * _t + CM[7]);
            BL2 = (float)(CM[8] * t3 + CM[9] * t2 + CM[10] * _t + CM[11]);
            BL3 = (float)(CM[12] * t3 + CM[13] * t2 + CM[14] * _t + CM[15]);
        }
        else
        {
            BL0 = (float)(CM[0] * t2 + CM[1] * _t + CM[2]);
            BL1 = (float)(CM[4] * t2 + CM[5] * _t + CM[6]);
            BL2 = (float)(CM[8] * t2 + CM[9] * _t + CM[10]);
            BL3 = (float)(CM[12] * t2 + CM[13] * _t + CM[14]);
        }

        Vector3 tVect = BL0 * P0 + BL1 * P1 + BL2 * xVect1 + BL3 * xVect2;

        if (!_isTangent)
        {
            if (tVect.y < 0f)
            {
                tVect.y = 0f;
            }
        }
        /*if(float.IsNaN(tVect.x) || float.IsNaN(tVect.y) || float.IsNaN(tVect.z))
        {
            Debug.Log($"BL0: {BL0}, P0: {P0}, BL1: {BL1}, P1: {P1}, BL2: {BL2}, xVect1: {xVect1}, BL3: {BL3}, xVect2: {xVect2}, _i: {_i}");
            Debug.Log($"CM[0] ({CM[0]}) * t2 ({t2}) + CM[1] ({CM[1]}) * _t ({_t}) + CM[2] ({CM[2]}) = {CM[0] * t2 + CM[1] * _t + CM[2]}");
            Debug.Log($"CM[4] ({CM[4]}) * t2 ({t2}) + CM[5] ({CM[5]}) * _t ({_t}) + CM[6] ({CM[6]}) = {CM[4] * t2 + CM[5] * _t + CM[6]}");
            Debug.Log($"CM[8] ({CM[8]}) * t2 ({t2}) + CM[9] ({CM[9]}) * _t ({_t}) + CM[10] ({CM[10]}) = {CM[8] * t2 + CM[9] * _t + CM[10]}");
            Debug.Log($"CM[12] ({CM[12]}) * t2 ({t2}) + CM[13] ({CM[13]}) * _t ({_t}) + CM[14] ({CM[14]}) = {CM[12] * t2 + CM[13] * _t + CM[14]}");
        }*/
        return tVect;
    }


    private static readonly double[] CM = new double[] {
         2.0, -3.0,  0.0,  1.0,
        -2.0,  3.0,  0.0,  0.0,
         1.0, -2.0,  1.0,  0.0,
         1.0, -1.0,  0.0,  0.0
    };


    private static readonly int[] NI = new int[] { 0, 1, -1, 2 };

    private int NGI(int _i, int _o)
    {
        int NGITI = _i + _o;
        //		if(bClosed){
        //			return (NGITI % mNodes.Count + mNodes.Count) % mNodes.Count;
        //		}else{
        return Mathf.Clamp(NGITI, 0, nodes.Count - 1);
        //		}
    }

    /*
     * main calculation
     */
    void calculateTimes()
    {
        float distance;
        int nodeCount = nodes.Count;
        times = new float[nodeCount];
        tangents = new Vector3[nodeCount];
        distances = new float[nodeCount];

        //First lets get the general distance, node to node:
        times[0] = 0f;
        times[nodeCount - 1] = 1f;
        Vector3 node1 = new Vector3(0f, 0f, 0f);
        Vector3 node2 = new Vector3(0f, 0f, 0f);
        float roadLength = 0f;
        float roadLengthOriginal = 0f;

        // Calculate accumulated distance between nodes
        for (int j = 0; j < nodeCount; j++)
        {
            node2 = nodes[j];
            if (j > 0)
            {
                roadLength += Vector3.Distance(node1, node2);
            }
            node1 = node2;
        }


        roadLengthOriginal = roadLength;


        //Get a slightly more accurate portrayal of the time:
        float nodeTime = 0f;
        for (int j = 0; j < (nodeCount - 1); j++)
        {
            node2 = nodes[j];
            if (j > 0)
            {
                nodeTime += (Vector3.Distance(node1, node2) / roadLengthOriginal);
                times[j] = nodeTime;
            }
            node1 = node2;
        }

        //Using general distance and calculated step, get an accurate distance:
        float splineDistance = 0f;
        Vector3 prevPos = nodes[0];
        Vector3 currentPos = new Vector3(0f, 0f, 0f);

        roadLength = roadLength * 1.05f;
        float step = 0.5f / roadLength;
        prevPos = GetSplineValue(0f);
        for (float i = 0f; i < 1f; i += step)
        {
            currentPos = GetSplineValue(i);
            splineDistance += Vector3.Distance(currentPos, prevPos);
            prevPos = currentPos;
        }

        distance = splineDistance;


        //Now get fine distance between nodes:
        float newTotalDistance = 0f;
        step = 0.5f / distance;
        prevPos = GetSplineValue(0f, false);
        float[] tempSegmentTime = new float[nodeCount];
        for (int j = 1; j < nodeCount; j++)
        {

            if (j == 1)
            {
                prevPos = GetSplineValue(times[j - 1]);
            }
            splineDistance = 0.00001f;

            for (float i = times[j - 1]; i < times[j]; i += step)
            {
                currentPos = GetSplineValue(i);
                if (!float.IsNaN(currentPos.x))
                {
                    if (float.IsNaN(prevPos.x))
                    {
                        prevPos = currentPos;
                    }
                    splineDistance += Vector3.Distance(currentPos, prevPos);
                    prevPos = currentPos;
                }
            }
            tempSegmentTime[j] = (splineDistance / distance);
            newTotalDistance += splineDistance;
            distances[j] = newTotalDistance;
        }
        distances[0] = 0f;

        #region Debugging
        //nodes[0].dist = 0f;
        /*prevNode = nodes[nodeCount - 2];
        currentNode = nodes[nodeCount - 1];
        splineDistance = 0.00001f;
        for (float i = prevNode.time; i < currentNode.time; i += step)
        {
            currentPos = GetSplineValue(i, false);
            if (!float.IsNaN(currentPos.x))
            {
                if (float.IsNaN(prevPos.x))
                {
                    prevPos = currentPos;
                }
                splineDistance += Vector3.Distance(currentPos, prevPos);
                prevPos = currentPos;
            }
        }
        currentNode.tempSegmentTime = (splineDistance / distance);
        newTotalDistance += splineDistance;
        currentNode.dist = newTotalDistance;*/
        #endregion
        distance = newTotalDistance;

        //Debug.Log(times[0]);
        // Set node data
        //SplineN node;
        nodeTime = 0f;
        for (int j = 1; j < (nodeCount - 1); j++)
        {
            //node = nodes[j];
            nodeTime += tempSegmentTime[j];
            //node.oldTime = node.time;
            times[j] = nodeTime;
            tangents[j] = GetSplineValueSkipOpt(nodeTime, true);
            //Debug.Log(nodeTime);
        }
        tangents[0] = GetSplineValueSkipOpt(0f, true);
        tangents[nodeCount - 1] = GetSplineValueSkipOpt(1f, true);
        //Debug.Log(times[nodeCount-1]);

        //RoadDefCalcs
        float tMod = Mathf.Lerp(0.05f, 0.2f, distance / 9000f);
        step = tMod / distance;
        currentPos = GetSplineValue(0f);
        prevPos = currentPos;
        float road_roadDefinition = 5f;
        float tempDistanceModMax = road_roadDefinition - step;
        float tempDistanceMod = 0f;
        float tempTotalDistance = 0f;
        float tempDistanceHolder = 0f;
        if (RoadDefKeysArray != null)
        {
            RoadDefKeysArray = null;
        }
        if (RoadDefValuesArray != null)
        {
            RoadDefValuesArray = null;
        }

        List<int> RoadDefKeys = new List<int>();
        List<float> RoadDefValues = new List<float>();

        RoadDefKeys.Add(0);
        RoadDefValues.Add(0f);

        for (float index = 0f; index < 1f; index += step)
        {
            currentPos = GetSplineValue(index);
            tempDistanceHolder = Vector3.Distance(currentPos, prevPos);
            tempTotalDistance += tempDistanceHolder;
            tempDistanceMod += tempDistanceHolder;
            if (tempDistanceMod > tempDistanceModMax)
            {
                tempDistanceMod = 0f;
                RoadDefKeys.Add(TranslateParamToInt(index));
                RoadDefValues.Add(tempTotalDistance);
            }
            prevPos = currentPos;
        }

        RoadDefKeysArray = RoadDefKeys.ToArray();
        RoadDefValuesArray = RoadDefValues.ToArray();

        //calculate right / middle / left points
        int kFinalCount = RoadDefKeysArray.Length;
        RoadMiddlePoints = new Vector2[kFinalCount];
        RoadRightPoints = new Vector2[kFinalCount];
        RoadLeftPoints = new Vector2[kFinalCount];
        //Debug.Log($"kFinalCount == road.spline.RoadDefKeysArray.Length: {kFinalCount == road.spline.RoadDefKeysArray.Length}");
        for (var kCount = 0; kCount < kFinalCount; kCount++)
        {
            //Debug.Log($"{road.spline.RoadDefKeysArray[i]}, {road.spline.RoadDefValuesArray[i]}");
            //Debug.Log(RoadDefKeysArray[kCount] == road.spline.RoadDefKeysArray[kCount]);
            var i = TranslateInverseParamToFloat(RoadDefKeysArray[kCount]);
            Vector3 myVect, tempPOS;
            GetSplineValueBoth(i, out myVect, out tempPOS);
            //(new GameObject($"{kCount}: {myVect.ToString()}")).transform.position = myVect;
            float tempangle = Mathf.PI / 2 - Mathf.Atan(Mathf.Abs(tempPOS.x) / Mathf.Abs(tempPOS.z));
            float temproadWidth = 10f;
            float tempa = (temproadWidth / 2) * Mathf.Sin(tempangle);
            float tempb = (temproadWidth / 2) * Mathf.Cos(tempangle);
            if (tempPOS.x > 0) tempb = -1 * tempb;
            if (tempPOS.z < 0) tempa = -1 * tempa;
            RoadMiddlePoints[kCount] = new Vector2(myVect.x, myVect.z);
            RoadRightPoints[kCount] = new Vector2(myVect.x + tempa, myVect.z + tempb);
            RoadLeftPoints[kCount] = new Vector2(myVect.x - tempa, myVect.z - tempb);
        }
    }

    public int TranslateParamToInt(float _value)
    {
        return Mathf.Clamp((int)(_value * 10000000f), 0, 10000000);
    }

    public float TranslateInverseParamToFloat(int _value)
    {
        return Mathf.Clamp(((float)(float)_value / 10000000f), 0f, 1f);
    }

    #endregion

}

