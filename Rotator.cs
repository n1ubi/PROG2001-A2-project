using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float x, y, z;

    [SerializeField] float floatAmplitude = 0.2f;  //浮动高度
    [SerializeField] float floatSpeed = 2f;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the object by x,y,z following the time
        transform.Rotate(new Vector3(x, y, z)* Time.deltaTime);

        //平滑上下浮动
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startPos + new Vector3(0, offsetY, 0);
    }
}
