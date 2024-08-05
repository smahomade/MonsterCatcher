using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonsterBase;



[CreateAssetMenu(fileName = "Move", menuName = "Monster/Create New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;
    [TextArea]
    [SerializeField] string moveDescription;
    [SerializeField] MonsterType moveType;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool guaranteeHit;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffect> secondaryEffect;
    [SerializeField] MoveTarget target;


    public string GetMoveName()
    {
        return moveName;
    }
    public string GetMoveDescription()
    {
        return moveDescription;
    }
    public MonsterType GetMoveType()
    {
        return moveType;
    }
    public int GetPower()
    {
        return power;
    }
    public int GetAccuracy()
    {
        return accuracy;
    }
    public bool GetGuaranteeHit()
    {
        return guaranteeHit;
    }
    public int GetPP()
    {
        return pp;
    }
    public int GetMovePriority()
    {
       return priority;
    }
    public MoveCategory GetCategory()
    {
        return category;
    }

    public MoveEffects GetEffects()
    {
        return effects;
    }

    public List<SecondaryEffect>GetSecondaryEffect()
    {
        return secondaryEffect;
    }

    public MoveTarget GetTarget()
    {
        return target;
    }
}


    [System.Serializable]
    public class MoveEffects
    {
        [SerializeField] List<StatBoost> boosts;
        [SerializeField] ConditionID status;
        [SerializeField] ConditionID volatileStatus;

        public List<StatBoost> GetBoosts()
        {
            return boosts;
        }

        public ConditionID GetStatus()
        {
            return status;
        }

        public ConditionID GetVolatileStatus()
        {
            return volatileStatus; 
        }
    }

[System.Serializable]
public class SecondaryEffect : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int GetChance()
    {
        return chance;
    }

    public MoveTarget GetTarget()
    {
        return target; 
    }
}

[System.Serializable]
    public class StatBoost
    {
        public Stat stat;
        public int boost;
    }

    public enum MoveCategory
    {
        Physical, Special, Status
    }

    public enum MoveTarget
    {
        Foe, Self
    }



