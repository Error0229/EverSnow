using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private Player player;

    private IList<Npc> npcs;

    public Player PlayerInstance
    {
        get => player;
    }

    public Npc GetNpcByName(string npcName)
    {
        return npcs.FirstOrDefault(npc => npc.RealName == npcName);
    }
    public void UpdateStoryState(IList<PlotDialogEndState> states)
    {
        player.StoryState = states.First(s => s.Name == player.RealName).State;
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
