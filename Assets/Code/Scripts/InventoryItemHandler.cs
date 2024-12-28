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
    private TextMeshProUGUI countText;
    [SerializeField]
    private Button button;

    private void Awake()
    {
        image.sprite = null;
        countText.text = "";
        button.enabled = false;
    }

    public void SetUp(Item item, int count)
    {
        this.item = item;
        image.sprite = item.Icon;
        countText.text = count == 0 ? "" : (item.IsStackable ? "x" : "") + count.ToString();
        button.enabled = count > 0;
        button.onClick.AddListener(() =>
        {
            InventoryUI.Instance.OnItemClick(item);
        });
    }
    public void Clear()
    {
        image.sprite = null;
        countText.text = "";
        button.enabled = false;
    }




}
