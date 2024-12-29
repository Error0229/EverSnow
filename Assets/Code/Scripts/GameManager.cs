using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player player;

    private readonly IList<Npc> npcs = new List<Npc>();

    public Player PlayerInstance
    {
        get => player;
    }
    public bool IsPlayerInGame => player.IsInGame();

    public Npc GetNpcByName(string npcName)
    {
        return npcs.FirstOrDefault(npc => npc.RealName == npcName);
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
}
