using System.Linq;
public class BranchQuest : Quest
{
    public BranchQuest()
    {
        questName = "BranchQuest";
        description = "來自 Aguta 的委託，在附近的矮樹叢區域取得五根樹枝。";
        AddGoal("Branch", 5);
    }

    public override void SyncProgressions()
    {
        if (state != State.Accepted) return;
        progressions["Branch"] = GameManager.Instance.PlayerInstance.GetInventory().Count(i => i.Name == "Branch");
        if (progressions["Branch"] >= goals["Branch"])
        {
            Complete();
        }
    }

    public override void Complete()
    {
        base.Complete();
        GameManager.Instance.PlayerInstance.StoryState = "CompletedBranchQuest";
    }

    public override void Finish()
    {
        base.Finish();
        var branches = GameManager.Instance.PlayerInstance.GetInventory().Where(i => i.Name == "Branch").Take(5).ToList();
        foreach (var item in branches)
        {
            GameManager.Instance.PlayerInstance.GetInventory().Remove(item);
        }
        for (var i = 0; i < 3; i++)
            GameManager.Instance.PlayerInstance.Obtain(ItemFactory.Instance.CreateItem("Ice"));
    }
}
