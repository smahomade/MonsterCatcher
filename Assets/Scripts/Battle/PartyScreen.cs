using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text msgTxt;

    PartyMemberUI[] monsterPartySlots;
    List<Monster> monsters;
    //MonsterParty party;
    //int selection = 0;

    public void Init()
    {
        monsterPartySlots = GetComponentsInChildren<PartyMemberUI>(true);

    }

    public void UpdatePartyMonsterSelection(int selectedMonsters)
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == selectedMonsters)
                monsterPartySlots[i].ImplementChosenMonster(true);
            else
                monsterPartySlots[i].ImplementChosenMonster(false);
        }
    }

    public void ImplementMsgText(string message)
    {
        msgTxt.text = message;
    }

    public void ImplementPartyData(List<Monster> monsters)
    {
        this.monsters = monsters; 

        for (int n = 0; n < monsterPartySlots.Length; n++)
        {
            
            if (n < monsters.Count)
            {
                monsterPartySlots[n].gameObject.SetActive(true);
                monsterPartySlots[n].ImplementData(monsters[n]);
            }
            else
                monsterPartySlots[n].gameObject.SetActive(false);
        }
        msgTxt.text = "Select a Monster";
    }
}
