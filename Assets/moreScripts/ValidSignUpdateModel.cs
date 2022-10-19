using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidSignUpdateModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float ValidSign_x = transform.position[0];
        float ValidSign_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - ValidSign_x;
        float vector_z = direction_z - ValidSign_z;
        viewModel.AddValidSignVector(ValidSign_x, ValidSign_z, transform.localScale[2], vector_x, vector_z);
    }
}
