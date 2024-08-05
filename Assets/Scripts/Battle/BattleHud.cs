using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text lvlText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject xpBar;

    Monster _monster;
  
    public void ImplementData(Monster monster)
    {
        _monster = monster;

        nameText.text = monster.Base.GetMonsterName();
        ImplementLvl();
        hpBar.ImplementHP((float)monster.HP / monster.MaxHP);
        ImplementXp();
    }
    public void ImplementXp()
    {
        if (xpBar == null)
        {
            return;
        }
        float modifyXp = GetModifiedExp();
        xpBar.transform.localScale = new Vector3(modifyXp, 1, 1);
    }

    public string ImplementLvl() {   return lvlText.text = "Lvl " + _monster.Lvl; }


    public IEnumerator ImplementXpTransitionSmoothly(bool retry=false)
    {
        if (xpBar == null)
            yield break;
    
        if (retry)
            xpBar.transform.localScale = new Vector3(0, 1, 1);

        float modifyXp = GetModifiedExp();
        yield return xpBar.transform.DOScaleX(modifyXp, 1.5f).WaitForCompletion();
    }
    float GetModifiedExp()
    {
        int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Lvl + 1);
        int currentLevelExp = _monster.Base.GetExpForLevel(_monster.Lvl);
        float normalizedExp = (float)(_monster.Xp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator UpdateHP()
    {
        if (_monster.HPChanged)
        {
            yield return hpBar.ImplementHPSmoothly((float)_monster.HP / _monster.MaxHP);
            _monster.HPChanged = false;
        }
    }

}

