using System.Linq;
public class AxeQuest : Quest
{
    public AxeQuest()
    {
        questName = "AxeQuest";
        description = "來自 Inuksuk 的委託，在附近的樹林找到五株槲寄生";
        AddGoal("Mistletoe", 5);
    }

    public override void SyncProgressions()
    {
        if (state != State.Accepted) return;
        progressions["Mistletoe"] = GameManager.Instance.PlayerInstance.GetInventory().Count(i => i.Name == "Mistletoe");
        if (progressions["Mistletoe"] >= goals["Mistletoe"])
        {
            Complete();
        }
    }
    public override void Complete()
    {
        base.Complete();
        GameManager.Instance.PlayerInstance.StoryState = "CompletedAxeQuest";
    }

    public override void Finish()
    {
        base.Finish();
        var mistletoe = GameManager.Instance.PlayerInstance.GetInventory().Where(i => i.Name == "Mistletoe").Take(5).ToList();
        foreach (var item in mistletoe)
        {
            GameManager.Instance.PlayerInstance.GetInventory().Remove(item);
        }
    }
}
