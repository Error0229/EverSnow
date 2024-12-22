public class NormalTypewriter : ITypewriterStrategy
{
    private readonly float typeSpeed;

    // Highlighted section 1
    public NormalTypewriter(float typeSpeed = 0.05f)
    {
        this.typeSpeed = typeSpeed;
    }

    // Highlighted section 2
    public override string ProcessText(
        string fullText, int currentIndex, float elapsedTime)
    {
        return fullText.Substring(0, currentIndex);
    }

    public override string PrepareText(
        string fullText, int currentIndex, float elapsedTime)
    {
        return fullText.Substring(0, currentIndex);
    }

    public override bool IsComplete(
        int currentIndex, string fullText)
    {
        return currentIndex >= fullText.Length;
    }

    // Highlighted section 3
    // This method is defined as a getter. 
    // It is equivalent to:
    // public override float GetTypeSpeed() { return typeSpeed; }
    public override float GetTypeSpeed()
    {
        return typeSpeed;
    }
}
