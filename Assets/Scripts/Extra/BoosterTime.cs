using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoosterTime : MonoBehaviour
{
    public float NextLevelUpTime;
    private float MonsterTimeLevelUp;
    private float CurrentTime;
    public static Action OnTimerComplete;

    private void Update()
    {
        if (MonsterTimeLevelUp >= NextLevelUpTime)
        {
            OnTimerComplete?.Invoke();
            MonsterTimeLevelUp = 0;
        }

        MonsterTimeLevelUp += Time.deltaTime;
        CurrentTime += Time.deltaTime;
        DisplayTime(CurrentTime);
    }
    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay >= 0)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}