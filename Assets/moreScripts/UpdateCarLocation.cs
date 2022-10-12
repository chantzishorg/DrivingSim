using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCarLocation : MonoBehaviour
{
    public App appObject;
    // Start is called before the first frame update
    void Start()
    {
        App.SetInitialCarLocation(transform.position[0], transform.position[2]);
    }

    // Update is called once per frame
    void Update()
    {
        App.MoveCar(transform.position[0], transform.position[2]);
        appObject.ReportSpeed(GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
    }
}
