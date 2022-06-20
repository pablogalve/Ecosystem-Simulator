using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void StartSimulation()
    {
        SceneManager.LoadScene("Simulator");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
