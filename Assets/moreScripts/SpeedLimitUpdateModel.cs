using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimitUpdateModel : MonoBehaviour
{
    public float oldSpeedLimit;
    public float newSpeedLimit;
    // Start is called before the first frame update
    void Start()
    {
        float speedLimitLine_x = transform.position[0];
        float speedLimitLine_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - speedLimitLine_x;
        float vector_z = direction_z - speedLimitLine_z;
        App.AddSpeedLimit(speedLimitLine_x, speedLimitLine_z, transform.localScale[2], vector_x, vector_z,newSpeedLimit,oldSpeedLimit);
    }
}
