using UnityEngine;

public class StoryAutoTriggerPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StoryManager.Instance.TryInvokePlot();
        }
    }
}
