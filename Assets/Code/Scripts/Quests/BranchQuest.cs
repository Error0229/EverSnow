using System.Linq;
public class BranchQuest : Quest
{
    public BranchQuest()
    {
        questName = "BranchQuest";
        description = "Find the branch and bring it to the NPC";
        AddGoal("Branch", 5);
    }

    public override void SyncProgressions()
    {
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
        GameManager.Instance.PlayerInstance.GetInventory().RemoveAll(i => i.Name == "Branch");
    }
}
