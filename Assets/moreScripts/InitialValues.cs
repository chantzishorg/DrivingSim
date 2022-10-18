using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialValues : MonoBehaviour
{
    public float initialSpeed = 30f;
    // Start is called before the first frame update
    void Start()
    {
        viewModel.SetInitialSpeed(initialSpeed);
    }
}
