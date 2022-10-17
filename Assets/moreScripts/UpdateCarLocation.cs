using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCarLocation : MonoBehaviour
{
    public static float tangent2Yrotation(Vector3 POS)
    {
        float angle = Mathf.Atan(POS.x / POS.z) * 180 / Mathf.PI;
        if (POS.z < 0)
        {
            if (POS.x > 0) angle += 180;
            else angle -= 180; // if x<=0
        }
        return angle;
    }

    public static Vector2 yRotation2Tangent(float angle)
    {
        angle = angle * Mathf.PI / 180;
        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
    }
    
    // Start is called before the first frame update
    void Start()
    {
        App.SetInitialCarLocation(transform.position[0], transform.position[2]);
    }

    // Update is called once per frame
    void Update()
    {
        App.MoveCar(transform.position[0], transform.position[2], yRotation2Tangent(transform.rotation.eulerAngles.y));
        App.ReportSpeed(GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
    }
}
