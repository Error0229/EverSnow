using UnityEngine;
public class Weapon : Item
{
    public int damage;
    [SerializeField]
    protected Collider cldr;
    private void Awake()
    {
        cldr = gameObject.GetComponent<Collider>();
    }
    public void DisableCollider()
    {
        cldr.enabled = false;
        Entity.GetComponent<Collider>().enabled = false;
    }
    public void EnableCollider()
    {
        cldr.enabled = true;
        Entity.GetComponent<Collider>().enabled = true;
    }
    public void Attack()
    {
        Entity.GetComponent<Collider>().enabled = true;
    }
    public void OnAttackFinish()
    {
        Entity.GetComponent<Collider>().enabled = false;
    }

    public override void Equip()
    {
        GameManager.Instance.PlayerInstance.Equip(this);
    }
}
