using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexRotate : MonoBehaviour
{
    float x, y, z;
    void Start()
    {
        x = 0f;
        y = 0f;
        z = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        y++;
        transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
    }
}
