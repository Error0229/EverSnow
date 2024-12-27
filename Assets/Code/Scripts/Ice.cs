public class Ice : Item
{
    public override void Use()
    {
        GameManager.Instance.PlayerInstance.Consume(this);
    }
}
