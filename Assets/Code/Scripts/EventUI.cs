using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EventUI : Singleton<EventUI>
{
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private UnityAction onConfirm;

    protected override void Init()
    {
        eventPanel.SetActive(false);
        confirmButton.onClick.AddListener(HandleConfirm);
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(false);
        }
    }

    public void ShowDeathPanel(UnityAction onRespawn)
    {
        onConfirm = onRespawn;
        eventPanel.SetActive(true);
    }

    private void HandleConfirm()
    {
        eventPanel.SetActive(false);
        onConfirm?.Invoke();
    }
}
