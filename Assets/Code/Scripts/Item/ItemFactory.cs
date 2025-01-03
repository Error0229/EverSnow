using UnityEngine;
public class ItemFactory : Singleton<ItemFactory>
{
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject mistletoePrefab;
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private GameObject knifePrefab;
    public Item CreateItem(string itemName)
    {
        switch (itemName)
        {
            case "Axe":
                return Instantiate(axePrefab).GetComponent<Item>();
            case "Branch":
                return Instantiate(branchPrefab).GetComponent<Item>();
            case "Mistletoe":
                return Instantiate(mistletoePrefab).GetComponent<Item>();
            case "Ice":
                return Instantiate(icePrefab).GetComponent<Item>();
            case "Knife":
                return Instantiate(knifePrefab).GetComponent<Item>();
            default:
                return null;
        }
    }
}
