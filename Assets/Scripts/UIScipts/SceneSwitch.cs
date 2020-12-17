using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void ToScene(string SceneName)
    {
        Debug.Log("to " + SceneName);
        SceneLoader._instance.LoadScene(SceneName);
    }
}