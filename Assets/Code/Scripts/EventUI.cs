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
        cancelButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
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
