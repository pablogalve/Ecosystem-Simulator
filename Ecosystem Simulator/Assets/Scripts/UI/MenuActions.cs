using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void StartSimulation()
    {
        SceneManager.LoadScene("Simulator");
    }

    public void StartSandbox()
    {
        SceneManager.LoadScene("Sandbox");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
