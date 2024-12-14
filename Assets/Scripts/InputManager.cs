using UnityEngine;
using UnityEngine.Events;

public abstract class InputManager : Singleton<InputManager>
{
    public UnityEvent<Vector2> evtDpadAxis;
    public UnityEvent<bool> evtJump;
    public UnityEvent<bool> evtRun;
    public UnityEvent<bool> evtDialogClick; // Highlighted section 0
    public UnityEvent<Vector2> evtLook;

    private void Update()
    {
        CalculateMove();
        CalculateJump();
        CalculateRun();
        CalculateDialogClick(); // Highlighted section 3
        PostProcessDpadAxis();
        CalculateLook();
    }

    protected abstract void CalculateMove();
    protected abstract void CalculateJump();
    protected abstract void CalculateRun();
    protected abstract void CalculateDialogClick(); // Highlighted section 2
    protected abstract void PostProcessDpadAxis();
    protected abstract void CalculateLook();
}