using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InGameUI : Singleton<InGameUI>
{
    [SerializeField]
    private TextMeshProUGUI iceCount;
    [SerializeField]
    private Image currentWeapon;
    [SerializeField]
    private Image wealthSnowman;

    private void Update()
    {
        iceCount.text = GameManager.Instance.PlayerInstance.GetIceCount().ToString();
        currentWeapon.sprite = GameManager.Instance.PlayerInstance.GetEquippedWeapon().Icon;
    }
}
