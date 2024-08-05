using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Monster> wildCrazyMonsters;

    private void OnEnable() {BoosterTime.OnTimerComplete += LvlUp;}

    private void OnDisable(){BoosterTime.OnTimerComplete -= LvlUp;}

    public Monster GetRandomWildCrazyMonster()
    {
        Monster wildMonster = wildCrazyMonsters[Random.Range(0, wildCrazyMonsters.Count)];
        wildMonster.Init();
        return wildMonster;
    }

    public void LvlUp(){ foreach (Monster VARIABLE in wildCrazyMonsters) { VARIABLE.Lvl++; }}
}
