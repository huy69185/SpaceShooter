using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGameButton()
    {
        SceneManager.LoadScene("GamePlay"); // Chuyển đến màn chơi chính
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void InstructionButton()
    {
        SceneManager.LoadScene("Instruction"); // Chuyển đến scene hướng dẫn
    }

    public void StoryButton()
    {
        SceneManager.LoadScene("Story"); // Chuyển đến story 
    }
}
