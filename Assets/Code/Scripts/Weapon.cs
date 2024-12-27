using UnityEngine;
public class Weapon : Item
{
    public int damage;

    [SerializeField] private GameObject entity;
    [SerializeField] private Sprite sprite;

    public Sprite Icon { get => sprite; }

    public override void Equip()
    {
        GameManager.Instance.PlayerInstance.Equip(this);
    }
}
