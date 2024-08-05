using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fence : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] int fencePrice;

    public static Action OnRemoveFence;
    public void Interact(Transform initiator)
    {
            StartCoroutine(DialogManager.Instance.DisplayDialog(dialog, () => {
                if (Input.GetKeyDown(KeyCode.Z)) { SpendCoin(); }
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

}
