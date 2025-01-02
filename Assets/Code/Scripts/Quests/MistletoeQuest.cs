using System.Linq;
public class MistletoeQuest : Quest
{
    public MistletoeQuest()
    {
        questName = "MistletoeQuest";
        description = "來自獵人的委託，在附近的樹林找到五株槲寄生";
        AddGoal("Mistletoe", 5);
    }

    public override void SyncProgressions()
    {
        progressions["Mistletoe"] = GameManager.Instance.PlayerInstance.GetInventory().Count(i => i.Name == "Mistletoe");
        if (progressions["Mistletoe"] >= goals["Mistletoe"])
        {
            Complete();
        }
    }
    public override void Complete()
    {
        base.Complete();
        GameManager.Instance.PlayerInstance.StoryState = "CompletedMistletoeQuest";
    }

    public override void Finish()
    {
        base.Finish();
        GameManager.Instance.PlayerInstance.GetInventory().RemoveAll(i => i.Name == "Mistletoe");
    }
}
