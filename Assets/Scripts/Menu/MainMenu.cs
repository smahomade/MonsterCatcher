using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator transition;
    public void PlayGame()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(levelIndex);
    }
}
