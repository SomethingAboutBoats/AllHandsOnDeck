using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTextboxInteraction : MonoBehaviour
{
    public TMP_Text mTextboxText;
    public TMP_Text mButtonText;

    public DialogueBase mStartDialogue;
    public DialogueBase mCurrentDialogue;
    public string NextScene = "1_CalmSea";


    public void Start()
    {
        if (SceneInterface.Instance.GameState == GameStates.STARTING)
        {
            mCurrentDialogue = mStartDialogue;
            SceneInterface.Instance.NextSuccessDialogue = mCurrentDialogue.NextSuccessDialogue;
            SceneInterface.Instance.NextFailureDialogue = mCurrentDialogue.NextFailureDialogue;
        }
        else if (SceneInterface.Instance.GameState == GameStates.SURVIVED)
        {
            mCurrentDialogue = SceneInterface.Instance.NextSuccessDialogue;
            SceneInterface.Instance.NextSuccessDialogue = mCurrentDialogue.NextSuccessDialogue;
            SceneInterface.Instance.NextFailureDialogue = mCurrentDialogue.NextFailureDialogue;
        }
        else if (SceneInterface.Instance.GameState == GameStates.SUNK)
        {
            mCurrentDialogue = SceneInterface.Instance.NextFailureDialogue;
            SceneInterface.Instance.NextSuccessDialogue = mCurrentDialogue.NextSuccessDialogue;
            SceneInterface.Instance.NextFailureDialogue = mCurrentDialogue.NextFailureDialogue;
        }

        SceneInterface.Instance.GameState = GameStates.IN_MENU;

        if (mCurrentDialogue != null)
        {
            mCurrentDialogue.ResetConvo();
        }
        UpdateText();
    }

    public void UpdateText()
    {
        if (mButtonText.text == "**SET SAIL**")
        {
            SceneInterface.Instance.GameState = GameStates.SAILING;
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
