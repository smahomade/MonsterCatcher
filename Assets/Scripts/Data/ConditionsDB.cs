using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHP / 8);
                    monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} hurt itself due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHP / 16);
                    monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} hurt itself due to burn");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                OnBeforeMove = (Monster monster) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()}'s paralyzed and can't move");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMessage = "has been frozen",
                OnBeforeMove = (Monster monster) =>
                {
                    if  (Random.Range(1, 5) == 1)
                    {
                        monster.RemoveStatusEffects();
                        monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()}'s is not frozen anymore");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep",
                OnStart = (Monster monster) =>
                {
                    // Sleep for 1-3 turns
                    monster.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be asleep for {monster.StatusTime} moves");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.StatusTime <= 0)
                    {
                        monster.RemoveStatusEffects();
                        monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} woke up!");
                        return true;
                    }

                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} is sleeping");
                    return false;
                }
            }
        },

        // Volatile Status Conditions
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMessage = "has been confused",
                OnStart = (Monster monster) =>
                {
                    // Confused for 1 - 4 turns
                    monster.VolatileStatusTime = Random.Range(1, 5);
                    Debug.Log($"Will be confused for {monster.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} kicked out of confusion!");
                        return true;
                    }
                    monster.VolatileStatusTime--;

                    // 50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return true;

                    // Hurt by confusion
                    monster.StatusChanges.Enqueue($"{monster.Base.GetMonsterName()} is confused");
                    monster.DecreaseHP(monster.MaxHP / 8);
                    monster.StatusChanges.Enqueue($"It hurt itself due to confusion");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
            return 2f;
        else if (condition.Id == ConditionID.par || condition.Id == ConditionID.psn || condition.Id == ConditionID.brn)
            return 1.5f;

        return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}

