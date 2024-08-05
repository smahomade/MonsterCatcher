using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private bool ConfirmKeyPressed => Input.GetKeyDown(confirmKey) || Input.GetKeyDown(confirmKeyAlt);
    private KeyCode confirmKey = KeyCode.Z, confirmKeyAlt = KeyCode.KeypadEnter;
    private bool BackKeyPressed => Input.GetKeyDown(backKey) || Input.GetKeyDown(backKeyAlt);
    private KeyCode backKey= KeyCode.X, backKeyAlt = KeyCode.Delete;

    [SerializeField] GameObject dialogBoxSprite;
    [SerializeField] int lettersPerSec;
    [SerializeField] Text dialogTxt;
    Dialog dialog;
    bool isTyping;
    Action onDialogCompletion;
    int curLine = 0;
    public event Action onDisplayDialog;
    public event Action onCeaseDialog;

    public static DialogManager Instance { get; private set; }

    public bool isShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CeaseDialog()
    {
        curLine = 0;
        isShowing = false;
        dialogBoxSprite.SetActive(false);
        onDialogCompletion?.Invoke();
        onCeaseDialog?.Invoke();
    }
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogTxt.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogTxt.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSec);
        }
        isTyping = false;
    }

    public IEnumerator DisplayDialog(Dialog dialog, Action onCompletition = null)
    {
        yield return new WaitForEndOfFrame();

        onDisplayDialog?.Invoke();
        this.dialog = dialog;
        onDialogCompletion = onCompletition;
        dialogBoxSprite.SetActive(true);
        StartCoroutine(TypeDialog(dialog.LinesOfDialog[0]));
    }
    public void HandleUpdate()
    {
        if (ConfirmKeyPressed && !isTyping)
        {
            ++curLine;
            if(curLine < dialog.LinesOfDialog.Count)
                StartCoroutine(TypeDialog(dialog.LinesOfDialog[curLine]));
            else
            {
                curLine = 0;
                isShowing = false;
                dialogBoxSprite.SetActive(false);
                onDialogCompletion?.Invoke();
                onCeaseDialog?.Invoke();
            }
        }
        else if (BackKeyPressed)
        {
            curLine = 0;
            isShowing = false;
            dialogBoxSprite.SetActive(false);
            onDialogCompletion?.Invoke();
            onCeaseDialog?.Invoke();
        }
    }


}
