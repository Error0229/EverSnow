public class Axe : Weapon
{

    public void Awake()
    {
        this.Entity.SetActive(false);
        QuestManager.Instance.evtQuestFinished.AddListener(OnQuestFinish);
    }
    private void OnQuestFinish(string questName)
    {
        if (questName == "AxeQuest")
        {
            this.Entity.SetActive(true);
        }
    }
}
