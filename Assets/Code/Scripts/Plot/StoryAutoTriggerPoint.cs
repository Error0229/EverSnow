using UnityEngine;

public class StoryAutoTriggerPoint : MonoBehaviour
{
    [SerializeField]
    private string label;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StoryManager.Instance.TryInvokePlotByLabel(label);
        }
    }
}
