using UnityEngine;
public class Weapon : Item
{
    public int damage;


    public override void Equip()
    {
        GameManager.Instance.PlayerInstance.Equip(this);
    }
}
