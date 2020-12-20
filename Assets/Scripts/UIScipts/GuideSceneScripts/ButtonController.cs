using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    [SerializeField]
    public GameObject[] layers;

    public GameObject nextPageButton;
    //public GameObject returnButton;

    int pressButtonCount;
    void Start()
    {
        for (int i = 0; i < layers.Length; i++) 
        {
            Debug.Log(layers[i].name);
            layers[i].SetActive(false);
        }
        pressButtonCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushBottonNextPage()
    {
        //右移100
        CameralControl o = this.gameObject.GetComponent<CameralControl>();
        if(o.onMove == false)
        {
            layers[pressButtonCount - 1].SetActive(false);
            o.onMove = true;
        }

    }

    public void SwitchCanvasToNextPage()
    {
        pressButtonCount += 1;
        layers[pressButtonCount - 1].SetActive(true);
        if (pressButtonCount == 5)
            nextPageButton.SetActive(false);
    }
}
