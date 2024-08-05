using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Currency : MonoBehaviour
{
   public static Currency Instance;
   [SerializeField]private int _coin;
   public int coin
   {
      get
      {
         return _coin;
      }
      set
      {
         _coin = value;
         GetComponentInChildren<TextMeshProUGUI>()
            .DOTextFloat(int.Parse(GetComponentInChildren<TextMeshProUGUI>().text), _coin, 0.5f);
         PlayerPrefs.SetInt("Currency",_coin);
      }
   }

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      coin = PlayerPrefs.GetInt("Currency", 0);
   }
}
