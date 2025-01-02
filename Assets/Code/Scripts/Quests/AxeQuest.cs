using System.Linq;
public class AxeQuest : Quest
{
    public AxeQuest()
    {
        questName = "AxeQuest";
        description = "Find the axe and bring it to the NPC";
        AddGoal("Axe", 1);
    }

    public override void SyncProgressions()
    {
        progressions["Axe"] = GameManager.Instance.PlayerInstance.GetInventory().Count(i => i.Name == "Axe");
        if (progressions["Axe"] >= goals["Axe"])
        {
            Complete();
        }
    }
    public override void Complete()
    {
        base.Complete();
        GameManager.Instance.PlayerInstance.GetInventory().RemoveAll(i => i.Name == "Axe");
        GameManager.Instance.PlayerInstance.StoryState = "CompletedAxeQuest";
    }

    public override void Finish()
    {
        base.Finish();
        GameManager.Instance.PlayerInstance.Obtain(ItemFactory.Instance.CreateItem("Axe"));
    }
}
