using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    //public Rigidbody vehicleBody;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("printSpeed", 0f, 1f);
    }

    void printSpeed()
    {
        Debug.Log(Mathf.Round(GetComponent<Rigidbody>().velocity.magnitude * 3.6f));
    }

    void Update()
    {
        //test2.MoveCar();
        test2.MoveCar(transform.position.x, transform.position.z);
        //App.ReportSpeed(GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
    }
}
