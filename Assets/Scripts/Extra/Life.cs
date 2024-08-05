using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    public static Life Instance;
    public GameObject OverPopUp;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private int _lifeCount;

    public int lifeCount
    {
        get
        {
            return _lifeCount;
        }
        set
        {
            _lifeCount = value;
            for (int i = 1; i <= 3; i++)
            {
                if (i <= _lifeCount)
                    transform.GetChild(i-1).gameObject.SetActive(true);
                else
                    transform.GetChild(i-1).gameObject.SetActive(false);
            }

            if (_lifeCount <= 0)
                OverPopUp.SetActive(true);

        }
    }

    private void OnEnable()
    {
        lifeCount = 3;
    }
}
