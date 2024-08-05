using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameTxt;
    [SerializeField] HPBar hpBar;
    [SerializeField] Text lvlTxt;
    [SerializeField] Color colourHightlighted;

    Monster _monster;

    public void ImplementData(Monster monster)
    {
        _monster = monster;

        nameTxt.text = monster.Base.GetMonsterName();
        lvlTxt.text = "Lvl " + monster.Lvl;
        hpBar.ImplementHP((float)monster.HP / monster.MaxHP);
    }

    public void ImplementChosenMonster(bool chosen){ nameTxt.color = chosen ? colourHightlighted : Color.black; }

}
