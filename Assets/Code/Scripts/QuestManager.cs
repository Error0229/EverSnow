using System.Collections.Generic;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    public List<Quest> quests;
    protected override void Init()
    {
        quests = new List<Quest>{
            new MistletoeQuest(),
            new BranchQuest(),
            new AxeQuest()
        };
    }
    public void RegisterQuest(Quest quest)
    {
        quests.Add(quest);
    }
    public Quest GetQuestByName(string questName)
    {
        return quests.FirstOrDefault(q => q.QuestName == questName);
    }

    public void StartQuest(string questName)
    {
        var quest = GetQuestByName(questName);
        QuestUI.Instance.StartQuest(quest);
        quest?.Accept();
    }
    public void Update()
    {
        foreach (var quest in quests)
        {
            quest.SyncProgressions();
        }
    }
    public void FinishQuest(string questName)
    {
        var quest = GetQuestByName(questName);
        QuestUI.Instance.FinishQuest(quest);
        quest?.Finish();
    }
}
