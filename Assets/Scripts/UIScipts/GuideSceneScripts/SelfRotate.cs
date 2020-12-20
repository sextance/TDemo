using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    public float speed = 4.0f;
    public float rotateRangeXMin = 0.0f;
    public float rotateRangeXMax = 1.0f;
    public float rotateRangeYMin = 0.0f;
    public float rotateRangeYMax = 1.0f;
    public float rotateRangeZMin = 0.0f;
    public float rotateRangeZMax = 1.0f;

    float timer = 1.0f;
    bool timerDown = false;
    Vector3 direction = Vector3.up;

    void Update()
    {
        if (timerDown == false )
        {
            direction.x = Random.Range(rotateRangeXMin, rotateRangeXMax);
            direction.y = Random.Range(rotateRangeYMin, rotateRangeYMax);
            direction.z = Random.Range(rotateRangeZMin, rotateRangeZMax);
            timer = Random.Range(3.0f, 5.0f);
            timerDown = true;
        } else {
            this.transform.Rotate(direction * speed * Time.deltaTime * speed, Space.World);
            timer -= Time.deltaTime;
            if(timer <= 0.0f)
                timerDown = false;
        }
    }
}