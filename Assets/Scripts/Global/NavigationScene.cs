using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScene : MonoBehaviour
{
    public void MoveScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
