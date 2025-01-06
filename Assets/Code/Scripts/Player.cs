using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField]
    private BetterPlayerController playerEntity;
    [SerializeField]
    private Story story;

    private readonly List<Item> inventory = new List<Item>();
    public BetterPlayerController Entity => playerEntity;

    private Weapon equippedWeapon;

    private State state;
    private bool triggerEnter;
    private Vector3 lastCheckpoint;
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (_health <= 0 && state != State.Dead)
            {
                Die();
            }
        }
    }
    public int MaxHealth { get; set; }
    public string RealName { get; } = "Kissimi";
    public string StoryState
    {
        get => story.State;
        set
        {
            switch (value)
            {
                case "FinishMistletoeQuest":
                    QuestManager.Instance.FinishQuest("MistletoeQuest");
                    break;
                case "DoingMistletoeQuest":
                    QuestManager.Instance.StartQuest("MistletoeQuest");
                    break;

                case "DoingAxeQuest":
                    QuestManager.Instance.StartQuest("AxeQuest");
                    break;
                case "FinishAxeQuest":
                    QuestManager.Instance.FinishQuest("AxeQuest");
                    break;

                case "DoingBranchQuest":
                    QuestManager.Instance.StartQuest("BranchQuest");
                    break;
                case "FinishBranchQuest":
                    QuestManager.Instance.FinishQuest("BranchQuest");
                    break;
            }
            story.State = value;
        }
    }

    public void Awake()
    {
        PlayerInputManager.Instance.evtInteract.AddListener(Interact);
        StoryManager.Instance.evtEnterDialog.AddListener(EnterDialog);
        StoryManager.Instance.evtLeaveDialog.AddListener(LeaveDialog);
        PlayerInputManager.Instance.evtInventory.AddListener(InvokeInventory);
        PlayerInputManager.Instance.evtUseItem.AddListener(UseItem);
        MaxHealth = 3;
        Health = MaxHealth;
        state = State.InGame;
        lastCheckpoint = transform.position;
    }

    private void UseItem()
    {
        if (state != State.InGame) return;
        var ice = inventory.Find(item => item is Ice);
        if (ice != null)
        {
            Consume(ice as Ice);
        }
    }
    public bool IsInGame()
    {
        return state == State.InGame;
    }
    private void InvokeInventory()
    {
        if (state == State.Inventory)
        {
            InventoryUI.Instance.CloseInventory();
            GoToState(State.InGame);
        }
        else if (state == State.InGame)
        {
            GoToState(State.Inventory);
        }
    }

    public void StartGame()
    {
        inventory.Add(ItemFactory.Instance.CreateItem("Knife"));
        GoToState(State.InGame);
    }

    public void Update()
    {
        if (Health <= 0 && state != State.Dead)
        {
            Die();
            return;
        }

        switch (state)
        {
            case State.InDialog:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.Deactivate();
                    playerEntity.enabled = false;
                }
                break;
            case State.InGame:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.enabled = true;
                    playerEntity.Activate();
                }
                string hint = playerEntity.GetInteractionHint();
                if (!string.IsNullOrEmpty(hint))
                {
                    InGameUI.Instance.ShowHint(hint);
                }
                else
                {
                    InGameUI.Instance.HideHint();
                }
                break;
            case State.Dead:
                if (triggerEnter)
                {
                    playerEntity.PlayDeathAnimation();
                    AudioManager.Instance.PlayMusic("SnowmanDead");
                    EventUI.Instance.ShowDeathPanel(Respawn);
                }
                break;
            case State.Inventory:
                if (triggerEnter)
                {
                    triggerEnter = false;
                    playerEntity.Deactivate();
                    InventoryUI.Instance.OpenInventory();
                }
                break;
        }

    }

    private void Die()
    {
        GoToState(State.Dead);
    }

    public void Respawn()
    {
        AudioManager.Instance.PlayMusic("Normal");
        Health = MaxHealth;
        GoToState(State.InGame);
        playerEntity.Respawn(lastCheckpoint);
        ItemFactory.Instance.RespawnItem();
        inventory.RemoveAll(item => item is Ice);
        for (var i = 0; i < 3; i++)
        {
            inventory.Add(ItemFactory.Instance.CreateItem("Ice"));
        }
    }

    public void UpdateCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public void Obtain(Item item)
    {
        AudioManager.Instance.PlaySFX("UseItem");
        item.OnObtained();
        if (item is Weapon weapon)
        {
            weapon.DisableCollider();
        }
        inventory.Add(item);
    }

    public void Interact()
    {
        if (state != State.InGame)
        {
            return;
        }
        // check start dialog
        var npc = playerEntity.CheckLookAtNpc();
        if (npc)
        {
            StoryManager.Instance.TryInvokePlot(npc);
            return;
        }

        // check pick up item
        var item = playerEntity.CheckLookAtItem();
        if (item != null)
        {
            Obtain(item);
        }
    }

    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public void Consume(Ice ice)
    {
        // if (Health == MaxHealth) return;
        Health = Mathf.Min(MaxHealth, Health + 1);
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

    public List<Item> GetInventory()
    {
        return inventory;
    }

    public void Equip(Weapon weapon)
    {
        equippedWeapon = weapon;
        Entity.Equip(weapon);
    }
    public void Remove(Weapon weapon)
    {
        Entity.Remove(weapon);
        equippedWeapon = null;
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
