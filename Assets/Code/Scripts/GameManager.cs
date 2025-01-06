using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player player;

    private readonly IList<Npc> npcs = new List<Npc>();

    private State state = State.MainMenu;
    private string ending;
    private State pendingState;
    private bool hasStateChange;

    public Player PlayerInstance
    {
        get => player;
    }
    public bool IsPlayerInGame => player.IsInGame();

    public Npc GetNpcByName(string npcName)
    {
        return npcs.FirstOrDefault(npc => npc.RealName == npcName);
    }

    public List<Npc> GetNpcs()
    {
        return npcs.ToList();
    }

    public void RegisterNpc(Npc npc)
    {
        npcs.Add(npc);
    }

    public void UpdateStoryState(IList<PlotDialogEndState> states)
    {
        var nextPlayerState = states.FirstOrDefault(s => s.Name == player.RealName);
        if (nextPlayerState != null)
            player.StoryState = nextPlayerState.State;
        foreach (var npc in npcs)
        {
            var newState = states.FirstOrDefault(s => s.Name == npc.RealName);
            if (newState != null)
            {
                npc.StoryState = newState.State;
            }
        }
    }
    protected override void Init()
    {
        base.Init();
        SceneManager.sceneLoaded += OnSceneLoaded;
        GoToState(State.MainMenu);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasStateChange) return;

        if (pendingState == State.Ending)
        {
            Cursor.visible = true;
            var endingObj = GameObject.FindFirstObjectByType<Ending>();
            endingObj.ShowEnding(ending);
            state = pendingState;
            hasStateChange = false;
        }
        else if (pendingState == State.MainMenu)
        {
            player = GameObject.FindFirstObjectByType<Player>();
            MainMenuUI.Instance.ShowMainMenu();
            AudioManager.Instance.PlayMusic("Opening");
            state = pendingState;
            hasStateChange = false;
        }
    }

    private void GoToState(State newState)
    {
        pendingState = newState;
        hasStateChange = true;

        // Handle immediate state transitions for same-scene states
        if (newState == State.InGame)
        {
            MainMenuUI.Instance.HideMainMenu();
            AudioManager.Instance.PlayMusic("Normal");
            state = newState;
            hasStateChange = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Restart()
    {
        GoToState(State.MainMenu);
        SceneManager.LoadScene("Latest");
    }

    public void StartGame()
    {
        player.StartGame();
        foreach (var npc in npcs)
        {
            npc.StartGame();
        }
        GoToState(State.InGame);
    }
    public void GoToEnding(string endingType)
    {
        npcs.Clear();
        GoToState(State.Ending);
        SceneManager.LoadScene("Ending");
        ending = endingType;
    }
    public enum State
    {
        InGame,
        MainMenu,
        Ending
    }
}
