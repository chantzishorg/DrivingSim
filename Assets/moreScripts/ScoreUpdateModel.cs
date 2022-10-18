using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUpdateModel : MonoBehaviour
{
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        float score_x = transform.position[0];
        float score_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - score_x;
        float vector_z = direction_z - score_z;
        viewModel.AddScoreVector(score_x, score_z, transform.localScale[2], vector_x, vector_z,score);
    }
}
