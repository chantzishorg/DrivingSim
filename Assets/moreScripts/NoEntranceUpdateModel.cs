using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEntranceUpdateModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float noentrance_x = transform.position[0];
        float noentrance_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - noentrance_x;
        float vector_z = direction_z - noentrance_z;
        /*Debug.Log($"no-entrance position: ({noentrance_x},{noentrance_z})");
        Debug.Log($"no-entrance width: {transform.localScale[2]}");
        Debug.Log($"no-entrance vector: ({vector_x},{vector_z})");*/
        App.AddNoEntrance(noentrance_x, noentrance_z, transform.localScale[2], vector_x, vector_z);
    }
}
