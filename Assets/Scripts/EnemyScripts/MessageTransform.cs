using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MessageTransform : MonoBehaviour
{
    public int num;
    private void Update()
    {
        message();
    }
    void message()
    {
        num = GameManager.gm.towerShapes.Count;
    }
}
