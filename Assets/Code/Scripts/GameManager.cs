using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;  // Add this

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player player;

    private readonly IList<Npc> npcs = new List<Npc>();

    private State state = State.MainMenu;
    private bool triggerEnter;

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
        // Register to scene loading events
        GoToState(State.MainMenu);
    }
    private void Update()
    {
        if (triggerEnter)
        {
            switch (state)
            {
                case State.MainMenu:
                    MainMenuUI.Instance.ShowMenu();
                    AudioManager.Instance.PlayMusic("Opening");
                    break;
                case State.InGame:
                    MainMenuUI.Instance.HideMainMenu();
                    AudioManager.Instance.PlayMusic("Normal");
                    break;
                case State.Ending:
                    break;
            }
            triggerEnter = false;
        }
    }
    private void GoToState(State newState)
    {
        triggerEnter = true;
        state = newState;
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
    public enum State
    {
        InGame,
        MainMenu,
        Ending
    }
}
