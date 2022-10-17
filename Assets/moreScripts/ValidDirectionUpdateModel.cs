using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidDirectionUpdateModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float ValidDirection_x = transform.position[0];
        float ValidDirection_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - ValidDirection_x;
        float vector_z = direction_z - ValidDirection_z;
        App.AddValidDirectionVector(ValidDirection_x, ValidDirection_z, transform.localScale[2], vector_x, vector_z);
    }
}
