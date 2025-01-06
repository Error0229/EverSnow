
using UnityEngine;
public class Ice : Thing
{
    private Vector3 home;
    public override void Use()
    {
        GameManager.Instance.PlayerInstance.Consume(this);
    }
    private void Awake()
    {
        home = transform.position;
    }
    public void Respawn()
    {
        Entity.SetActive(true);
        GetComponent<Collider>().enabled = true;
    }
}
