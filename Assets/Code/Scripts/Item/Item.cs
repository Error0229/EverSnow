using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField]
    protected string itemName;
    [SerializeField]
    protected string description;
    [SerializeField]
    protected bool isStackAble;
    [SerializeField]
    protected bool isConsumable;
    [SerializeField]
    protected bool isQuestItem;
    [SerializeField]
    protected bool isEquipment;
    [SerializeField]
    protected Sprite icon;
    [SerializeField]
    private GameObject entity;
    [SerializeField]
    private string realName;

    public string Name
    {
        get => itemName;
    }

    public string RealName
    {
        get => realName;
    }
    public string Description
    {
        get => description;
    }
    public bool IsStackable
    {
        get => isStackAble;
    }
    public bool IsConsumable
    {
        get => isConsumable;
    }
    public bool IsEquipment
    {
        get => isEquipment;
    }
    public GameObject Entity
    {
        get => entity;
    }

    public void OnObtained()
    {
        Entity.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }
    public Sprite Icon { get => icon; }

    public virtual void Equip()
    {
    }

    public virtual void Use()
    {
    }


}
