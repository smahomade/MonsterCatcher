using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public Animator transition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
                //StartCoroutine(CloseLevel());
            }
            else
            {
                Pause();
            }
        }
    }


    public void Resume()
    {
        
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Menu() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Time.timeScale = 1f; //bugged, animations stopped because of this
    }

    IEnumerator CloseLevel()
    {
        transition.SetTrigger("Close");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1.5f);
        Resume();
    }
}
