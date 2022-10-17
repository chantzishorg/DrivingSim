using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCar : MonoBehaviour
{
    public static Vector2 yRotation2Tangent(float angle)
    {
        angle = angle * Mathf.PI / 180;
        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
    }
    void Start()
    {
        var vector = yRotation2Tangent(transform.rotation.eulerAngles.y);
        App.AddNpcCars(transform.position.x, transform.position.z,vector);
    }
}
