using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader._instance.LoadScene("TowerScene");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneLoader._instance.LoadScene("StartScene");
        }
    }
}
