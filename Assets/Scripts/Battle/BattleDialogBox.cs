using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSec;
    [SerializeField] Color colourHighlighted;
    [SerializeField] Text txtDialog;
    [SerializeField] GameObject eventSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> eventTxt;
    [SerializeField] List<Text> moveTxt;

    [SerializeField] Text moveAmountTxt;
    [SerializeField] Text monsterTypeTxt;
    public void ImplementDialog(string dialog)
    {
        txtDialog.text = dialog;
    }

    public IEnumerator WriteDialog(string dialog)
    {
        txtDialog.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            txtDialog.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSec);
        }

        yield return new WaitForSeconds(1f);
    }

    public void ActivateEventSelector(bool enabled)
    {
        eventSelector.SetActive(enabled);
    } 
    public void ActivateMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void ActivateDialogText(bool enabled)
    {
        txtDialog.enabled = enabled;
    }

    public void UpdateEventSelection(int selectedAction)
    {
        for (int n = 0; n < eventTxt.Count; ++n)
        {
            eventTxt[n].color = n == selectedAction ? colourHighlighted : Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int n = 0; n < moveTxt.Count; ++n)
        {
            moveTxt[n].color = n == selectedMove ? colourHighlighted : Color.black;
        }

        moveAmountTxt.text = $"PP {move.PP}/{move.Base.GetPP()}";
        monsterTypeTxt.text = move.Base.GetMoveType().ToString();

        moveAmountTxt.color = move.PP == 0 || move.PP <= move.Base.GetPP() / 4 ? Color.red : (
            move.PP <= move.Base.GetPP() / 2 ? new Color(1f, 0.647f, 0f) : Color.black);
    }

    public void ImplementMoveNames(List<Move> moves)
    {
        for (int n = 0; n < moveTxt.Count; ++n)
        {
            moveTxt[n].text = n < moves.Count ? moves[n].Base.GetMoveName() :  "-";
        }
    }

}
