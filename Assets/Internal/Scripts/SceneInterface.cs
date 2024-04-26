using UnityEngine;

public enum GameStates
{
    STARTING,
    SAILING,
    IN_MENU,
    SUNK,
    SURVIVED
}

public class SceneInterface : MonoBehaviour
{

    public static SceneInterface Instance;

    [SerializeField] private GameStates mGameState = GameStates.STARTING;
    [SerializeField] private DialogueBase mNextSuccessDialogue = null;
    [SerializeField] private DialogueBase mNextFailureDialogue = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public GameStates GameState
    {
        get { return mGameState; }
        set { mGameState = value; }
    }

    public DialogueBase NextSuccessDialogue
    {
        get { return mNextSuccessDialogue; }
        set { mNextSuccessDialogue = value; }
    }

    public DialogueBase NextFailureDialogue
    {
        get { return mNextFailureDialogue; }
        set { mNextFailureDialogue = value; }
    }

}
