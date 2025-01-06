using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<BetterPlayerController>();
        if (controller != null)
        {
            var player = controller.GetComponentInParent<Player>();
            if (player != null)
            {
                player.UpdateCheckpoint(transform.position);
                InGameUI.Instance.ShowNotification("復活點已更新", 2f);
                if (!GameManager.Instance.IsPlayerInGame) return;
                AudioManager.Instance.PlaySFX("UseItem");
            }
        }
    }
}
