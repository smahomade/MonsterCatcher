using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monsters", menuName = "Monster/Create New Monster")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] string monsterName;
    [TextArea]
    [SerializeField] string monsterDescription;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] MonsterType monsterType;

    [SerializeField] int maxHP;
    [SerializeField] int atk;
    [SerializeField] int def;
    [SerializeField] int specialAttack;
    [SerializeField] int specialDefence;
    [SerializeField] int spd;
    [SerializeField] int expYield;
    [SerializeField] GrowthRate monsterGrowthRate;
    [SerializeField] int monsterCatchChance = 255;
    [SerializeField] List<LearnableMove> learnableMoves;
    public static int MaxNumOfMoves { get; set; } = 4;

    public int GetExpForLevel(int level)
    {
        if (monsterGrowthRate == GrowthRate.Normal)
            return 4 * (level * level * level) / 5;
        else if (monsterGrowthRate == GrowthRate.Fast)
            return level * level * level;

        return -1;
    }

    public string GetMonsterName() {
        return monsterName;
    }
    public string GetMonsterDescription()
    {
        return monsterDescription;
    }
    public Sprite GetFrontSprite()
    {
        return frontSprite;
    }
    public Sprite GetBackSprite()
    {
        return backSprite;
    }
    public MonsterType GetMonsterType()
    {
        return monsterType;
    }
    public int GetMaxHP()
    {
        return maxHP;
    }
    public int GetAtk()
    {
        return atk;
    }
    public int GetDef()
    {
        return def;
    }
    public int GetSpecialAttack()
    {
        return specialAttack;
    }
    public int GetSpecialDefence()
    {
        return specialDefence;
    }
    public int GetSpd()
    {
        return spd;
    }

    public int GetExpYield()
    {
        return expYield;
    }
    public int CatchChance()
    {
        return monsterCatchChance;
    }

    public GrowthRate MonsterGrowthRate()
    {
        return monsterGrowthRate;
    }

    public List<LearnableMove> GetLearnableMoves()
    {
        return learnableMoves;
    }

    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] MoveBase moveBase;
        [SerializeField] int lvl;

        public MoveBase GetBase()
        {
            return moveBase;
        }
        public int GetLvl()
        {
            return lvl;
        }
    }

    public enum MonsterType
    {
        Normal,
        Fire,     //Strong: Plant, Wind     ---  Weak: Water, Earth
        Plant,    //Strong: Water, Electric ---  Weak: Fire, Wind
        Earth,    //Strong: Fire, Electric  ---  Weak: Wind, Water
        Wind,     //Strong: Plant, Earth    ---  Weak: Electric, Fire
        Water,    //Strong: Fire, Earth     ---  Weak: Plant, Electric
        Electric, //Strong: Water, Wind     ---  Weak: Earth, Plant
        Light,    //Strong: Dark            ---  Weak: Dark
        Dark      //Strong: Light           ---  Weak: Light
    }

    public enum GrowthRate 
    { 
        Normal,Fast
    }




    public enum Stat
    {
        Attack,
        Defence,
        SPAttack,
        SPDefence,
        Speed,

        Accuracy,
        Evasion
    }


    public class TypeChart
    {
        static float[][] chart =
        {   //                        NOR  FIR PLA EAR WIN WAT ELC LIG  DAR
            /*NOR*/      new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
            /*FIR*/      new float[] { 1f, .5f, 2f, .5f, 2f, .5f, 1f, 1f, 1f },
            /*PLA*/      new float[] { 1f, .5f, .5f, 1f, .5f, 2f, 2f, 1f, 1f },
            /*EAR*/      new float[] { 1f, 2f, 1f, .5f, .5f, .5f, 2f, 1f, 1f },
            /*WIN*/      new float[] { 1f, .5f, 2f, 2f, .5f, 1f, .5f, 1f, 1f },
            /*WAT*/      new float[] { 1f, 2f, .5f, 2f, 1f, .5f, .5f, 1f, 1f },
            /*ELC*/      new float[] { 1f, 1f, .5f, .5f, 2f, 2f, .5f, 1f, 1f },
            /*LIG*/      new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, .5f, 2f },
            /*DAR*/      new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, .5f }
        };

        public static float GetEffectiveness(MonsterType attackType, MonsterType defenceType)
        {
            

            int row = (int)attackType;
            int col = (int)defenceType;

            return chart[row][col];
        }
    }
}
