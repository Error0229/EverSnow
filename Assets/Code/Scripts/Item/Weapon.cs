using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int damage;
    [SerializeField]
    protected Collider cldr;
    private List<ParticleSystem> weaponParticle;

    private void Start()
    {
        cldr = gameObject.GetComponent<Collider>();
        weaponParticle = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
        weaponParticle?.ForEach(e => e.Stop());
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

    public void PlayEffect()
    {
        if (weaponParticle == null) return;
        weaponParticle.ForEach(e => e.Play());
    }

    public void StopEffect()
    {
        if (weaponParticle == null) return;
        weaponParticle.ForEach(e => e.Stop());
    }
}
