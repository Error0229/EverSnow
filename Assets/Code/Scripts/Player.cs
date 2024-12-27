using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField]
    private BetterPlayerController playerEntity;
    [SerializeField]
    private Story story;

    private readonly List<Item> inventory = new List<Item>();

    private Weapon equippedWeapon;

    private State state;
    private bool triggerEnter;
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public string RealName { get; } = "Kissimi";
    public string StoryState
    {
        get => story.State;
        set => story.State = value;
    }

    public void Awake()
    {
        PlayerInputManager.Instance.evtInteract.AddListener(Interact);
        StoryManager.Instance.evtEnterDialog.AddListener(EnterDialog);
        StoryManager.Instance.evtLeaveDialog.AddListener(LeaveDialog);
        MaxHealth = 3;
        Health = MaxHealth;
        state = State.InGame;
    }

    public void Update()
    {
        switch (state)
        {
            case State.InDialog:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.EnterDialog();
                    playerEntity.enabled = false;
                }
                break;
            case State.InGame:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.enabled = true;
                }
                break;
            case State.Dead:
                break;
            case State.Inventory:
                break;
        }

    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
    }

    public void Interact()
    {
        if (state == State.InDialog)
        {
            return;
        }
        // check start dialog
        var npc = playerEntity.CheckLookAtNpc();
        if (npc)
        {
            StoryManager.Instance.InvokePlot(npc);
        }

        // check pick up item
        var item = playerEntity.CheckLookAtItem();
        if (item != null)
        {
            AddItem(item);
        }
    }

    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public void Consume(Ice ice)
    {
        if (Health == MaxHealth) return;
        Health++;
        inventory.Remove(ice);
    }

    public int GetIceCount()
    {
        var count = 0;
        foreach (var item in inventory)
        {
            if (item is Ice)
            {
                count++;
            }
        }
        return count;
    }


    public void Equip(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    private void EnterDialog()
    {
        GoToState(State.InDialog);
    }
    private void LeaveDialog()
    {
        GoToState(State.InGame);
    }

    private void GoToState(State newState)
    {
        state = newState;
        triggerEnter = true;
    }

    private enum State
    {
        InDialog,
        InGame,
        Dead,
        Inventory
    }
}
