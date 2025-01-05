using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int damage;
    [SerializeField] private Collider cldr;
    private List<ParticleSystem> particleSystem;

    private void Awake()
    {
        cldr = gameObject.GetComponent<Collider>();
        particleSystem = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    public void DisableCollider()
    {
        cldr.enabled = false;
    }

    public override void Equip()
    {
        GameManager.Instance.PlayerInstance.Equip(this);
    }

    public void PlayEffect()
    {
        particleSystem.ForEach(e => e.Play());
    }

    public void StopEffect()
    {
        particleSystem.ForEach(e => e.Stop());
    }
}