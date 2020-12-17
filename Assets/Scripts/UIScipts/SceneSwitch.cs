using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void OnStartGame(string SceneName)
    {
        Debug.Log("PVE");
        SceneLoader._instance.LoadScene(SceneName);
    }
}