using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int damage;
    [SerializeField]
    protected Collider cldr;
    private List<ParticleSystem> particleSystem;

    private void Start()
    {
        cldr = gameObject.GetComponent<Collider>();
        particleSystem = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
        particleSystem?.ForEach(e => e.Stop());
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
        if (particleSystem == null) return;
        particleSystem.ForEach(e => e.Play());
    }

    public void StopEffect()
    {
        if (particleSystem == null) return;
        particleSystem.ForEach(e => e.Stop());
    }
}
