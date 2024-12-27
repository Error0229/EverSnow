using UnityEngine;

public abstract class Item
{
    [SerializeField]
    protected string name;
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

    public virtual void Equip()
    {
    }

    public virtual void Use()
    {
    }


}
