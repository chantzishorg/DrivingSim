using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Quad
{
    bool isIntersection;
    NormalQuad normalquad;
    IntersectionQuad intersectionquad;
}

struct NormalQuad
{
    int index;
    Vector3[] points; // in the order of: left, right, back, forward
}

class IntersectionQuad
{
    public Vector3[] points; // in the order of: left, right, back, forward of road1
    // 0: back left
    // 1: back right
    // 2: front right
    // 3: front left
    public RoadModel adj0_1 = null;
    public int index0_1;
    public RoadModel adj0_3 = null;
    public int index0_3;
    public RoadModel adj1_2 = null;
    public int index1_2;
    public RoadModel adj2_3 = null;
    public int index2_3;
}