using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float x = transform.position[0];
        float z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - x;
        float vector_z = direction_z - z;
        viewModel.AddEndLine(x, z, transform.localScale[2], vector_x, vector_z);
    }
}
