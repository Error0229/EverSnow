using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class KeyboardInputManager : InputManager
{
    private Vector2 axis;
    private bool dialogClick;
    private InputAction jump;
    private InputAction look;
    private InputAction move;
    private InputAction normalAttack;
    private InputAction strongAttack;
    private InputAction interact;
    private InputAction previous;
    private InputAction next;
    private InputAction block;
    private InputAction run;
    private InputAction dodge;
    private InputAction cameraSet;
    private InputAction useItem;
    private InputAction art;
    private InputAction walk;
    private InputAction nextWeapon;

    protected override void Init()
    {
        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
        run = InputSystem.actions.FindAction("Run");
        jump = InputSystem.actions.FindAction("Jump");
        normalAttack = InputSystem.actions.FindAction("NormalAttack");
        strongAttack = InputSystem.actions.FindAction("StrongAttack");
        interact = InputSystem.actions.FindAction("Interact");
        previous = InputSystem.actions.FindAction("Previous");
        next = InputSystem.actions.FindAction("Next");
        block = InputSystem.actions.FindAction("Block");
        dodge = InputSystem.actions.FindAction("Dodge");
        cameraSet = InputSystem.actions.FindAction("CameraSet");
        useItem = InputSystem.actions.FindAction("UseItem");
        art = InputSystem.actions.FindAction("Art");
        walk = InputSystem.actions.FindAction("Walk");
    }

    protected override void CalculateMove()
    {
        axis = move.ReadValue<Vector2>();
        evtMoveAxis?.Invoke(axis);
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