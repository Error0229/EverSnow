using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using MongoDB.Driver.Linq;
using UnityEngine;
using UnityEngine.Events;
public class InventoryUI : Singleton<InventoryUI>
{
    [SerializeField]
    private GameObject inventoryPanel;
    [SerializeField]
    private List<InventoryItemHandler> inventoryItems;

    public void OnItemClick(Item item)
    {
        Debug.Log("Item clicked: " + item.Name);
    }
    protected override void Init()
    {
        PlayerInputManager.Instance.evtInventory.AddListener(InvokeInventory);
    }
    private void InvokeInventory()
    {
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
                inventoryItems[i].SetUp(item: ie.Item, count: ie.Count);
            }
        }
    }
}
