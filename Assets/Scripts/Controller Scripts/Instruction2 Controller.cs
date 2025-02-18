using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instruction2Controller : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("Instruction");
    }

}
