using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueBase : ScriptableObject
{
    [SerializeField] [TextArea] private string[] mDialogue;
    private int mDialoguePointer = 0;
    public string[] Dialogue
    {
        get { return mDialogue; }
    }

    public Tuple<string, string> GetNextCallResponse()
    {
        if (mDialoguePointer < mDialogue.Length)
        {
            if (mDialoguePointer < mDialogue.Length - 1)
            {
                return new(mDialogue[mDialoguePointer++], mDialogue[mDialoguePointer++]);
            }
            else
            {
                return new(mDialogue[mDialoguePointer++], null);
            }
        }
        return null;
    }

    public void ResetConvo()
    {
        mDialoguePointer = 0;
    }
}
