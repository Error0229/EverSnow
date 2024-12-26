public abstract class ITypewriterStrategy
{
    public abstract string PrepareText(string fullText, int currentIndex, float elapsedTime);
    public abstract string ProcessText(string fullText, int currentIndex, float elapsedTime);
    public abstract bool IsComplete(int currentIndex, string fullText);
    public abstract float GetTypeSpeed();
}
