using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnStartGame(int SceneNumber)
    {
        Debug.Log("pve");
        SceneManager.LoadScene(SceneNumber);
    }
}