using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionUpdateModel : MonoBehaviour
{
   // public string directionTurn;
    public InstructionDirection instruction;
    // Start is called before the first frame update
    void Start()
    {
        float directionTurnLine_x = transform.position[0];
        float directionTurnLine_z = transform.position[2];
        float direction_x = transform.GetChild(0).position[0];
        float direction_z = transform.GetChild(0).position[2];
        float vector_x = direction_x - directionTurnLine_x;
        float vector_z = direction_z - directionTurnLine_z;
        viewModel.AddDirectionInstruction(directionTurnLine_x, directionTurnLine_z, transform.localScale[2], vector_x, vector_z, instruction);
    }
}
