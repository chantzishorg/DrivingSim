using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoValidDirectionUpdateModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float NoValidDirection_x = transform.position[0];
        float NoValidDirection_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - NoValidDirection_x;
        float vector_z = direction_z - NoValidDirection_z;
        App.AddnoValidDirectionVector(NoValidDirection_x, NoValidDirection_z, transform.localScale[2], vector_x, vector_z);
    }
}
