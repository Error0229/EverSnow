using UnityEngine;
public class Weapon : Item
{
    public int damage;
    [SerializeField]
    private Collider cldr;
    private void Awake()
    {
        cldr = gameObject.GetComponent<Collider>();
    }
    public void DisableCollider()
    {
        cldr.enabled = false;
    }

    public override void Equip()
    {
        GameManager.Instance.PlayerInstance.Equip(this);
    }
}
