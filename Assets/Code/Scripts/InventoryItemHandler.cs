using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemHandler : MonoBehaviour
{
    [SerializeField]
    private Item item;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Image equipMask;
    [SerializeField]
    private TextMeshProUGUI countText;
    [SerializeField]
    private Button button;

    private void Awake()
    {
        image.sprite = null;
        countText.text = "";
        button.enabled = false;
    }

    public void SetUp(Item item, int count, bool isEquipped)
    {
        equipMask.enabled = isEquipped;
        image.enabled = true;
        this.item = item;
        image.sprite = item.Icon;
        countText.text = (count == 0 || item.IsEquipment) ? "" : (item.IsStackable ? "x" : "") + count.ToString();
        button.enabled = count > 0;
        button.onClick.AddListener(() =>
        {
            InventoryUI.Instance.OnItemClick(item);
        });
    }
    public void Clear()
    {
        equipMask.enabled = false;
        image.sprite = null;
        image.enabled = false;
        countText.text = "";
        button.enabled = false;
    }




}
