using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForceBar : MonoBehaviour
{
    public Slider forceSlider;
    private float force;

    void Start()
    {
        force = 0.5F;
        //forceSlider = transfrom.GetComponent<Slider>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            force += 0.1F;
        }
        forceSlider.value = force;
    }
}