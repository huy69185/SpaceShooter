using UnityEngine;
using UnityEngine.SceneManagement;

public class Instruction : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }

    public void Next()
    {
        SceneManager.LoadScene("Instruction2"); 
    }
}