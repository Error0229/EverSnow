using UnityEngine;

public class StoryAutoTriggerPoint : MonoBehaviour
{
    [SerializeField]
    private string label;
    private bool triggered;
    private void Awake()
    {
        triggered = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (!GameManager.Instance.IsPlayerInGame || triggered) return;
        if (other.CompareTag("Player"))
        {
            StoryManager.Instance.TryInvokePlotByLabel(label);
        }
    }
}
