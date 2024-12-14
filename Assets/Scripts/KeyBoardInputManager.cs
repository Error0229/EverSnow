using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputManager : InputManager
{
    private Vector2 axis;
    private bool dialogClick;
    private InputAction move;
    private InputAction look;
    private InputAction sprint;
    private InputAction jump;
    private bool run;

    protected override void Init()
    {
        move = InputSystem.actions.FindAction("move");
        look = InputSystem.actions.FindAction("Look");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");
    }

    protected override void CalculateMove()
    {
        axis = move.ReadValue<Vector2>();
        evtDpadAxis?.Invoke(axis);
    }

    protected override void CalculateJump()
    {
        var j = jump.WasPressedThisFrame();
        evtJump?.Invoke(j);
    }

    protected override void CalculateRun()
    {
        var run = sprint.ReadValue<float>() > 0;
        evtRun?.Invoke(run);
    }

    protected override void CalculateDialogClick()
    {
        dialogClick = Mouse.current.leftButton.wasPressedThisFrame;
        evtDialogClick?.Invoke(dialogClick);
    }

    protected override void PostProcessDpadAxis()
    {
    }

    protected override void CalculateLook()
    {
        var lookValue = look.ReadValue<Vector2>();
        evtLook?.Invoke(lookValue);
    }
}