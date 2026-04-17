using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the object by x,y,z following the time
        transform.Rotate(new Vector3(x, y, z)* Time.deltaTime);
    }
}
