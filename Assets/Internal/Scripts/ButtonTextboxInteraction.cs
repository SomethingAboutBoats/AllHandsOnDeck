using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTextboxInteraction : MonoBehaviour
{
    public TMP_Text mTextboxText;
    public TMP_Text mButtonText;

    public DialogueBase mCurrentDialogue;
    public string NextScene = "1_CalmSea";


    public void Start()
    {
        if (mCurrentDialogue != null)
        {
            mCurrentDialogue.ResetConvo();
        }
        UpdateText();
    }

    public void UpdateText()
    {
        Debug.Log(mButtonText.text);
        if (mButtonText.text == "**SET SAIL**")
        {
            SceneManager.LoadScene(sceneName:NextScene);
            return;
        }

        if (mCurrentDialogue != null)
        {
            Tuple<string, string> convo = mCurrentDialogue.GetNextCallResponse();
            if (convo != null)
            {
                if (convo.Item1 != null)
                {
                    mTextboxText.SetText(convo.Item1);
                }
                else
                {
                    mTextboxText.SetText("");
                }

                if (convo.Item2 != null)
                {
                    mButtonText.SetText(convo.Item2);
                }
                else
                {
                    mButtonText.SetText("");
                }
            }
        }
    }
}
