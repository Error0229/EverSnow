using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InGameUI : Singleton<InGameUI>
{
    [SerializeField]
    private TextMeshProUGUI iceCountText;
    [SerializeField]
    private GameObject iceImage;
    [SerializeField]
    private Image currentWeapon;
    [SerializeField]
    private Image healthSnowman;

    [SerializeField]
    private List<Sprite> healthSprites;

    private void Update()
    {
        var iceCount = GameManager.Instance.PlayerInstance.GetIceCount();
        if (iceCount == 0)
        {
            iceImage.SetActive(false);
        }
        else
        {
            iceImage.SetActive(true);
            iceCountText.text = "x" + iceCount.ToString();
        }

        var weapon = GameManager.Instance.PlayerInstance.GetEquippedWeapon();
        if (weapon != null)
            currentWeapon.sprite = weapon.Icon;
        switch (GameManager.Instance.PlayerInstance.Health)
        {
            case 3:
                healthSnowman.sprite = healthSprites[0];
                break;
            case 2:

                healthSnowman.sprite = healthSprites[1];
                break;
            case 1:
                healthSnowman.sprite = healthSprites[2];
                break;
        }
    }

}
