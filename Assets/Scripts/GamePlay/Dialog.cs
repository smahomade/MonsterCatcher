using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] List<string> linesOfDialog;

    public List <string> LinesOfDialog
    {
        get { return linesOfDialog; }
    }
}
