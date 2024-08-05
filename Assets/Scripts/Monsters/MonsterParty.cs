using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterParty : MonoBehaviour
{
    [SerializeField] List<Monster> monsters;

    public List<Monster> Monsters
    {
        get { return monsters;}
    }

    private void Start()
    {
        foreach (var monster in monsters)
            monster.Init();
    }

    public Monster GetHealthyMonster()
    {
       return monsters.Where(x => x.HP > 0).FirstOrDefault();
    }

    public Monster GetFaintedMonster(int test)
    {
        return monsters[test];
    }


    public void AddMonster(Monster newMonster)
    {
        if (monsters.Count < 4)
            monsters.Add(newMonster);

    }
}
