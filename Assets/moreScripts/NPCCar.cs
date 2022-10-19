using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class that reperesents a car not the player's
public class NPCCar : MonoBehaviour
{
    // returns the vector of the car
    public static Vector2 yRotation2Tangent(float angle)
    {
        angle = angle * Mathf.PI / 180;
        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    void Start()
    {
        var vector = yRotation2Tangent(transform.rotation.eulerAngles.y);
        viewModel.AddNpcCars(transform.position.x, transform.position.z,vector);
    }
}
