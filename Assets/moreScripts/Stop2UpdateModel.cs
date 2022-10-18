using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop2UpdateModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float stop_x = transform.position[0];
        float stop_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - stop_x;
        float vector_z = direction_z - stop_z;
        viewModel.AddSecondStop(stop_x, stop_z, transform.localScale[2], vector_x, vector_z);
    }
}
