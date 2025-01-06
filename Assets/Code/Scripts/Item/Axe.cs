public class Axe : Weapon
{

    public void Awake()
    {
        DisableCollider();
        this.Entity.SetActive(false);
        QuestManager.Instance.evtQuestFinished.AddListener(OnQuestFinish);
    }
    private void OnQuestFinish(string questName)
    {
        if (questName == "AxeQuest")
        {
            EnableCollider();
            this.Entity.SetActive(true);
        }
    }
}
