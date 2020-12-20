using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginCell : MonoBehaviour
{

    float x,y,z;
    // Start is called before the first frame update
    void Start()
    {
        x = 0f;
        y = 0f;
        z = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        x += 2;
        y++;
        z += 1;
        transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
    }
}
