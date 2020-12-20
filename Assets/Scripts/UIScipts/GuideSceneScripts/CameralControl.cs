using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameralControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    public bool onMove = false;

    //二次曲线
    float speed = 0.0f;
    float timer = 1.0f;
    float accelaration1 = 395f;
    float accelaration2 = 405f;

    Vector3 direction = new Vector3(1, 0, 0);

    bool isLock;
    float lastX;

    void Start()
    {
        lastX = camera.transform.localPosition.x;
    }

    void FixedUpdate()
    {
        if (onMove == true)
        {
            OnMove();
        }
    }

    void OnMove()
    {
        if (isLock == false)
        {
            lastX = camera.transform.localPosition.x;
            isLock = true;
        }

        if (camera.transform.localPosition.x >= lastX + 100.0f)
        {
            onMove = false;
            isLock = false;
            timer = 1.0f;
            speed = 0.0f;
            ButtonController o = this.gameObject.GetComponent<ButtonController>();
            o.SwitchCanvasToNextPage();
        }
        else if (timer >= 0.5f)
        {
            // accelerate
            speed += accelaration1 * Time.deltaTime;
            timer -= Time.deltaTime;
            camera.transform.Translate(direction * Time.deltaTime * speed, Space.World);
        } else if (timer < 0.5f && timer >= 0.0f)
        {  // decelerate
            speed -= accelaration2 * Time.deltaTime;
            timer -= Time.deltaTime;
            camera.transform.Translate(direction * Time.deltaTime * speed, Space.World);
        }
        else if (timer <= 0.0f)
        { // reset para
            onMove = false;
            isLock = false;
            timer = 1.0f;
            speed = 0.0f;
            ButtonController o = this.gameObject.GetComponent<ButtonController>();
            o.SwitchCanvasToNextPage();
        }
    }
}
