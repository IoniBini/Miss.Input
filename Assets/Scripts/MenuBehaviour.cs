using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject beginButton;
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject introText;

    public void Begin()
    {
        startButton.SetActive(false);
        beginButton.SetActive(true);
        title.SetActive(false);
        introText.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
