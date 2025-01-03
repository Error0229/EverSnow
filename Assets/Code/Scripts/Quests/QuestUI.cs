using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [SerializeField] private GameObject questPanel;
    [SerializeField] private GameObject questInfoPanel;
    [SerializeField] private GameObject questInfoPrefab;

    private List<QuestInfoHandler> quests = new List<QuestInfoHandler>();

    protected override void Init()
    {
        base.Init();
        questPanel.SetActive(false);
    }

    public void StartQuest(Quest quest)
    {
        if (!questPanel.activeSelf)
        {
            questPanel.SetActive(true);
        }

        var questInfo = Instantiate(questInfoPrefab, questInfoPanel.transform).GetComponent<QuestInfoHandler>();
        questInfo.SetQuest(quest);
        quests.Add(questInfo);
    }

    public void FinishQuest(Quest quest)
    {
        var questInfo = quests.Find(q => q.GetQuestName() == quest.QuestName);
        if (questInfo != null)
        {
            questInfo.FinishQuest();
            quests.Remove(questInfo);

            if (quests.Count == 0)
            {
                // Hide panel after the animation completes
                Invoke(nameof(HideQuestPanel), 0.5f);
            }
        }
    }

    private void HideQuestPanel()
    {
        questPanel.SetActive(false);
    }
}
