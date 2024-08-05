using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MonsterBase;

[System.Serializable]
public class Monster
{
    [SerializeField] MonsterBase _base;
    [SerializeField] int lvl;

    public Monster(MonsterBase _Base, int _Lvl)
    {
        _base = _Base;
        lvl = _Lvl;
        Init();
    }

    public MonsterBase Base { get { return _base; } }

    public int Lvl
    {
        get { return lvl; }
        set { lvl = value; }
    }
    public int Xp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; } 
    public bool HPChanged { get; set; }
    /*public event System.Action OnStatusChanged;*/
    public void Init()
    {

        Moves = new List<Move>();
        foreach (var move in Base.GetLearnableMoves())
        {
            if (move.GetLvl() <= Lvl)
                Moves.Add(new Move(move.GetBase()));

            if (Moves.Count >= MonsterBase.MaxNumOfMoves)
                break;
        }
        EvaluateStats();
        Xp = Base.GetExpForLevel(Lvl);
        HP = MaxHP;
        StatusChanges = new Queue<string>();
        RestartAllBoostedStats();
        Status = null;
        VolatileStatus = null;
    }

    public void AfterTheTurnEnds()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void WhenTurnBasedBattleOver()
    {
        RestartAllBoostedStats();
        VolatileStatus = null;
    }

    int ObtainStats(Stat stat)
    {
        int statsValues = Stats[stat];

        int boost = StatBoosts[stat];
        float[] boostValues = new float[] { 1, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        statsValues = boost >= 0 ? Mathf.FloorToInt(statsValues * boostValues[boost]) : Mathf.FloorToInt(statsValues / boostValues[-boost]);
        return statsValues;
    }

    void EvaluateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.GetAtk() * Lvl) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.GetDef() * Lvl) / 100f) + 5);
        Stats.Add(Stat.SPAttack, Mathf.FloorToInt((Base.GetSpecialAttack() * Lvl) / 100f) + 5);
        Stats.Add(Stat.SPDefence, Mathf.FloorToInt((Base.GetSpecialDefence() * Lvl) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.GetSpd() * Lvl) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((Base.GetMaxHP() * Lvl / 100f) + 10 + Lvl);
    }

    void RestartAllBoostedStats()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0 },
            {Stat.Defence, 0 },
            {Stat.SPAttack, 0 },
            {Stat.SPDefence, 0 },
            {Stat.Speed, 0 },
            {Stat.Accuracy, 0 },
            {Stat.Evasion, 0 }
        };
    }

    public void RegisterTheIncreasedStats(List<StatBoost> increaseStats)
    {
        foreach (var statBoost in increaseStats)
        {
            Stat stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            
            if (boost > 0)
                StatusChanges.Enqueue($"{Base.GetMonsterName()}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.GetMonsterName()}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp()
    {
       
        if (Xp > Base.GetExpForLevel(lvl + 1))
        {
            ++lvl;
            //CalculateStats();
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrLevel() {  return Base.GetLearnableMoves().Where(x => x.GetLvl() == lvl).FirstOrDefault(); }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > MaxNumOfMoves)
            return;
        Moves.Add(new Move(moveToLearn.GetBase()));
    }
    public int MaxHP { get; private set; }
    public float Critical { get; internal set; }

    public int GetAttack()
    { return ObtainStats(Stat.Attack); }
    public int GetDefence()
    { return ObtainStats(Stat.Defence); }
    public int GetSPAttack()
    { return ObtainStats(Stat.SPAttack); }
    public int GetSPDefence()
    { return ObtainStats(Stat.SPDefence); }
    public int GetSpeed()
    { return ObtainStats(Stat.Speed); }

    public DamageDetails RecieveDamageFromMonster(Move move, Monster AttackingMonster)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.GetMoveType(), this.Base.GetMonsterType());
        DamageDetails damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.GetCategory() == MoveCategory.Special) ? AttackingMonster.GetSPAttack() : AttackingMonster.GetAttack();
        float defence = (move.Base.GetCategory() == MoveCategory.Special) ? GetSPDefence() : GetDefence();

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float n = (2 * AttackingMonster.Lvl + 10) / 250f;
        float z = n * move.Base.GetPower() * ((float)attack / defence) + 2;
        int damage = Mathf.FloorToInt(z * modifiers);

        DecreaseHP(damage);
        return damageDetails;
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HPChanged = true;
    }

    public void CapHPAtTenPercent()
    {
        HP = MaxHP/10;
        HPChanged = true;
    }

    public void ResetHealthWhenLifeLost()
    {
        HP = MaxHP;
        HPChanged = true;
    }



    public void ImplementStatus(ConditionID conditionId)
    {
        if (Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        // the question mark is a null condition operator
        // this prevents the code from crashing if the status doesnt have a "Status" or "OnStart" action 
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{ Base.GetMonsterName()} { Status.StartMessage}");
        
    }
    public void RemoveStatusEffects(){ Status = null; }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        // the question mark is a null condition operator
        // this prevents the code from crashing if the status doesnt have a "Status" or "OnStart" action 
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{ Base.GetMonsterName()} {VolatileStatus.StartMessage}");
    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        // Error if enemy runs out of PP (replace "movesWithPP" to "Moves", allowing the enemy to have unlimited PP)
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int randomMoveGenerated = Random.Range(0, movesWithPP.Count);
        return movesWithPP[randomMoveGenerated];
    }

    public bool OnBeforeMove()
    {
        bool moveCanBePerformed = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                moveCanBePerformed = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                moveCanBePerformed = false;
        }
        return moveCanBePerformed;
    }

}
    public class DamageDetails
    {
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }

    }

