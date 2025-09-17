using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Godot.Collections;
using Range = Godot.Range;

public partial class Player : CombatActor
{
    public const float WalkSpeed = 100.0f;
    public Inventory inventory = new Inventory();
    [Export] public InventoryUI inventoryUI;
    [Export] public CombatUI combatUI;
    [Export] public LevelUpUI levelUpUI;
    [Export] public Area2D swingArea2D;
    [Export] public AnimationPlayer CharacterEffectPlayer;
    [Export] public Sprite2D LevelUp2D;
    [Export] public StartingInventory startingInventory;
    [Export] public Array<WeaponBaseAction> selectableAbilities;
    public Vector2 direction;
    public bool isControllable = true;
    public PlayerState ActiveState;
    public System.Collections.Generic.Dictionary<State, PlayerState> States = new();
    public IInteractable _interactable;
    public static Player Instance { get; private set; }

    public void SelectInteractable(IInteractable interactable)
    {
        _interactable = interactable;
    }

    public void DeselectInteractable(IInteractable interactable)
    {
        if (interactable == _interactable)
        {
            _interactable = null;
        }
    }

    public override void _Ready()
    {
        SnapToClosestTile(this);
        Stats = new RuntimeStats(actorStats);
        combatUI.player = this;
        levelUpUI.player = this;
        Instance = this;
        LevelUp2D.Visible = false;

        States = new System.Collections.Generic.Dictionary<State, PlayerState>()
        {
            { State.Intro, new IntroState() { Player = this }},
            { State.Idle, new IdleState() { Player = this }},
            { State.Death, new DeathState() { Player = this }},
            { State.Walk, new WalkState() { Player = this }},
            { State.Attack, new AttackState() { Player = this }},
            { State.Combat, new CombatState() { Player = this }},
        };
        SetPlayerState(State.Idle);

        foreach (var enumType in Enum.GetValues<EquipSlot>())
        {
            EquippedItems.Add(enumType, null);
        }

        EquipAfterGlobalEvents();
        GlobalEvents.Instance.GetExperience += OnExperienceGained;
        GlobalEvents.Instance.OnLevelUp += OnLevelUp;
    }
    private void OnExperienceGained(double experience)
    {
        Stats.XP += experience;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(this);
        if (Stats.XP >= Stats.XPToNextLevel)
        {
            GlobalEvents.Instance.EmitOnLevelUp();
        }
    }
    private void OnLevelUp(object sender, EventArgs e)
    {
        LevelUp2D.Visible = true;
        CharacterEffectPlayer.Play(AnimTags.LevelUp);
        GetTree().CreateTimer(1.7).Timeout += () => LevelUp2D.Visible = false;
    }

    public async Task WaitForASec()
    {
        await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
    }
    
    public async void EquipAfterGlobalEvents()
    {
        while (inventoryUI.isready != true)
        {
            await WaitForASec();
        }

        EquipStartingItems();
        SetActiveWeapon();
        GetCurrentAbilities();
        GlobalEvents.Instance.EmitOnSkillBarChanged(allAbilities);
    }
    public void GetCurrentAbilities()
    {
        allAbilities.AddRange(GetItemBySlotType(EquipSlot.MainHand).weaponResource.Actions);
        allAbilities.AddRange(GetItemBySlotType(EquipSlot.Ranged).weaponResource.Actions);
    }

    public void GetCurrentAbilityByItem(EquipSlot itemType) => 
        allAbilities.AddRange(GetItemBySlotType(itemType).weaponResource.Actions);
    
    public void RemoveCurrentAbilities(EquipSlot itemType)=>
        allAbilities.RemoveAll(ability=>
           GetItemBySlotType(itemType).weaponResource.Actions.Contains(ability));
    
    public void EquipStartingItems()
    {
        foreach (PackedScene equipabbleItem in startingInventory.EquippableItemScenes)
        {
            var itemScene = equipabbleItem.Instantiate<EquipableItem>();
            AddChild(itemScene);
            EquippedItems[itemScene.ItemResource.equipSlot] = itemScene;
            GlobalEvents.Instance.EmitEquipSlotChanged(itemScene);
            itemScene.OnEquip(this);
            GlobalEvents.Instance.EmitInventoryStatsUpgraded(this);
            RemoveChild(itemScene);
        }
    }

    public void SetPlayerState(State newState)
    {
        ActiveState?.ExitState();
        ActiveState = States[newState];
        ActiveState.EnterState();
    }

    public void CheckForStatusEffects()
    {
        if (statusEffectList.Count == 0) return;
        foreach (StatusEffectBase statusEffect in statusEffectList)
        {
            switch (statusEffect)
            {
                case Buff effect:
                    effect.BuffEffect(this);
                    break;
                
                case Debuff effect:
                    effect.DebuffEffect(this);
                    break;
            } 
        }
    }

    public override void EnterCombat()
    {
        SetPlayerState(State.Combat);
        SnapToClosestTile(this);
        ResetActionPoints();
        SetActiveWeapon();
    }

    public override void StartTurn()
    {
        IsTurnActive = true;
        ((CombatState)ActiveState).currentSubState = CombatSubState.WaitForInput;
        Pathfinding.Instance.SetTileSolid(GlobalPosition, false, this);
    }

    public override void ExitCombat()
    {
        IsTurnActive = false;
        SetPlayerState(State.Idle);
        ResetActionPoints();
    }

    public override void _PhysicsProcess(double delta)
    {
        ActiveState.HandleInputs();
        ActiveState.PhysicsProcess(delta);
        
        if (Input.IsActionJustPressed("GetTile"))
        {
            var playerPos = GlobalPosition;
            GD.Print(TurnManager.Instance.GetTilePosition(playerPos));
        }

        MoveAndSlide();
    }
}