using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventoryUI : Singleton<InventoryUI>
{
    [SerializeField]
    private GameObject inventoryPanel;
    [SerializeField]
    private List<InventoryItemHandler> inventoryItems;
    [SerializeField]
    private TextMeshProUGUI itemDescription;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private Button useButton;
    [SerializeField]
    private TextMeshProUGUI useButtonText;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private TextMeshProUGUI cancelButtonText;
    [SerializeField]
    private Image snowman;


    public void OnItemClick(Item item)
    {
        itemName.text = item.RealName;
        itemDescription.text = item.Description;
        useButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        if (item is Weapon weapon)
        {
            useButtonText.text = "裝備";
            cancelButtonText.text = "卸除";
            if (GameManager.Instance.PlayerInstance.GetEquippedWeapon() == weapon)
            {
                useButton.interactable = false;
                cancelButton.interactable = true;
                cancelButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.PlayerInstance.Remove(item as Weapon);
                    RefreshInventory();
                    OnItemClick(item);
                    AudioManager.Instance.PlaySFX("CancelEquip");
                });
            }
            else
            {
                useButton.onClick.AddListener(() =>
                {
                    GameManager.Instance.PlayerInstance.Equip(item as Weapon);
                    RefreshInventory();
                    OnItemClick(item);
                    AudioManager.Instance.PlaySFX("UseItem");
                });
                useButton.interactable = true;
                cancelButton.interactable = false;
            }
        }
        else if (item is Thing thing)
        {
            useButtonText.text = "使用";
            cancelButtonText.text = "取消";
            if (item.IsConsumable)
            {
                useButton.interactable = true;
                useButton.onClick.AddListener(() =>
                {
                    item.Use();
                    RefreshInventory();
                    AudioManager.Instance.PlaySFX("UseItem");
                });
            }
            else
            {
                useButton.interactable = false;
            }
        }
        AudioManager.Instance.PlaySFX("SelectItem");
    }
    protected override void Init()
    {
        PlayerInputManager.Instance.evtInventory.AddListener(InvokeInventory);
    }
    private void InvokeInventory()
    {
        if (!GameManager.Instance.PlayerInstance.IsInGame()) return;
        if (inventoryPanel.activeSelf)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    public void OpenInventory()
    {
        RefreshInventory();
        inventoryPanel.SetActive(true);
        if (GameManager.Instance.PlayerInstance.GetInventory().Any())
        {
            EventSystem.current.SetSelectedGameObject(inventoryItems[0].gameObject);
        }
        AudioManager.Instance.PlaySFX("OpenInventory");
    }
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }
    public void RefreshInventory()
    {
        var inventory = GameManager.Instance.PlayerInstance.GetInventory().GroupBy(i => i.Name).Select(g => new
        {
            Item = g.First(),
            Count = g.Count()
        });

        for (var i = 0; i < 9; i++)
        {
            if (i >= inventory.Count())
            {
                inventoryItems[i].Clear();
            }
            else
            {
                var ie = inventory.ElementAt(i);
                inventoryItems[i].SetUp(item: ie.Item, count: ie.Count, ie.Item == GameManager.Instance.PlayerInstance.GetEquippedWeapon());
            }
        }
    }
    private void Update()
    {
        if (inventoryPanel.activeSelf)
            snowman.sprite = InGameUI.Instance.GetHealthSprite();
    }
}
