using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] int fencePrice;
    public GameObject winPopUp;
    public void Interact(Transform initiator)
    {
        StartCoroutine(DialogManager.Instance.DisplayDialog(dialog, () => {
            if (Input.GetKeyDown(KeyCode.Z))
            { 
                SpendCoin();
                RestartGamePopUp();
            }
        }));
    }

    void SpendCoin()
    {
        if (Currency.Instance.coin >= fencePrice)
        {
            this.gameObject.SetActive(false);
            Currency.Instance.coin -= fencePrice;

        }
    }

    void RestartGamePopUp()
    {
        winPopUp.SetActive(true);
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
