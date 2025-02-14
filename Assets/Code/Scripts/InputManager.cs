using UnityEngine;
using UnityEngine.Events;
public abstract class InputManager : Singleton<InputManager>
{
    public UnityEvent<Vector2> evtMoveAxis;
    public UnityEvent<bool> evtJump;
    public UnityEvent<bool> evtRun;
    public UnityEvent evtDialogClick;
    public UnityEvent<Vector2> evtLook;
    public UnityEvent<bool> evtDodge;
    public UnityEvent<bool> evtLockOn;
    public UnityEvent<bool> evtNormalAttack;
    public UnityEvent<bool> evtStrongAttack;
    public UnityEvent evtInteract;
    public UnityEvent evtInventory;
    public UnityEvent evtUseItem;
    public UnityEvent evtSkip;

    private void Update()
    {
        CalculateMove();
        CalculateJump();
        CalculateRun();
        CalculateDodge();
        CalculateDialogClick(); // Highlighted section 3
        PostProcessDpadAxis();
        CalculateLook();
        CalculateLockOn();
        CalculateAttack();
        CalculateInteract();
        CalculateDialogClick();
        CalculateInventory();
        CalculateUseItem();
        CalculateSkip();
    }
    private void FixedUpdate()
    { }

    protected abstract void CalculateInteract();
    protected abstract void CalculateMove();
    protected abstract void CalculateJump();
    protected abstract void CalculateRun();
    protected abstract void CalculateDialogClick(); // Highlighted section 2
    protected abstract void PostProcessDpadAxis();
    protected abstract void CalculateLook();
    protected abstract void CalculateDodge();
    protected abstract void CalculateLockOn();
    protected abstract void CalculateAttack();
    protected abstract void CalculateInventory();
    protected abstract void CalculateUseItem();
    protected abstract void CalculateSkip();
}
