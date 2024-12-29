public class Ice : Thing
{
    public override void Use()
    {
        GameManager.Instance.PlayerInstance.Consume(this);
    }
}
