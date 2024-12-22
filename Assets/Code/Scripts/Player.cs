using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField]
    private BetterPlayerController playerEntity;
    [SerializeField]
    private Story story;

    private State state;
    private bool triggerEnter;
    public string RealName { get; } = "Kissimi";


    public string StoryState
    {
        get => story.State;
        set => story.State = value;
    }

    public void Awake()
    {
        PlayerInputManager.Instance.evtInteract.AddListener(Interact);
        StoryManager.Instance.evtEnterDialog.AddListener(EnterDialog);
    }

    public void Update()
    {
        switch (state)
        {
            case State.InDialog:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.enabled = false;
                }
                break;
            case State.InGame:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.enabled = true;
                }
                break;
            case State.Dead:
                break;
            case State.Inventory:
                break;
        }

    }

    public void Interact()
    {
        // check start dialog
        var npc = playerEntity.CheckLookAtNpc();
        if (npc)
        {
            StoryManager.Instance.InvokePlot(npc);
        }
        // check pick up item
    }

    private void EnterDialog()
    {
        GoToState(State.InDialog);
    }

    private void GoToState(State newState)
    {
        state = newState;
        triggerEnter = true;
    }

    private enum State
    {
        InDialog,
        InGame,
        Dead,
        Inventory
    }
}
