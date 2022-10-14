using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoadArchitect;

public class updateRoadsLocations : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //var roadSystem = GetComponent<RoadSystem>();
        Road[] roads = GetComponentsInChildren<Road>();
        //int i = 0;
        //Debug.Log(roads[i].spline.nodes.Count);
        List<List<Vector3>> allRoadsNodes = new List<List<Vector3>>();
        foreach (var road in roads)
        {
            var roadNodes = new List<Vector3>();
            foreach (var node in road.spline.nodes)
            {
                //Debug.Log($"{node.transform.position}, isSpecialEndNode: {node.isSpecialEndNode}");
                if (!node.isSpecialEndNode)
                {
                    roadNodes.Add(node.transform.position);
                    //(new GameObject($"{i++}")).transform.position = node.transform.position;
                }
            }
            allRoadsNodes.Add(roadNodes);
            //Debug.Log(road.spline.nodes[road.spline.nodes.Count - 1].tangent);
        }
        App.SetRoadsLocation(allRoadsNodes);
    }
}
