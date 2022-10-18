using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpdateModel : MonoBehaviour
{
    public string nameImage;
    // Start is called before the first frame update
    void Start()
    {
        float signLine_x = transform.position[0];
        float signLine_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - signLine_x;
        float vector_z = direction_z - signLine_z;
        viewModel.AddSign(signLine_x, signLine_z, transform.localScale[2], vector_x, vector_z, nameImage);
    }
}
