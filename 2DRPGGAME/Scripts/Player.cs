using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;
using GodotPlugins.Game;
using Array = Godot.Collections.Array;

public partial class Player : CombatActor
{
    [Export] private bool CreatedCharacter;
    [Export] private Camera2D globalCamera;
    [Export] public InventoryUI inventoryUI;
    [Export] public CombatUI combatUI;
    [Export] public LevelUpUI levelUpUI;
    [Export] public Area2D swingArea2D;
    [Export] public AnimationPlayer CharacterEffectPlayer;
    [Export] public Sprite2D LevelUp2D;
    [Export] public StartingInventory startingInventory;
    [Export] public bool isControllable;
    [Export] public TextureRect PlayerPortrait;
    [Export] public Node PlayerParent;
    public Vector2 direction;
    public PlayerState ActiveState;
    public System.Collections.Generic.Dictionary<State, PlayerState> States = new();
    public List<BaseSkill> LearnedTalents = new();
    public Array<WeaponBaseAction> selectableAbilities = new();
    public List<BaseSkill> selectableTalents = new();
    public IInteractable _interactable;
    [Export] public CombatActor cursorTarget;
    public int FollowIndex;
    public bool LevelUpAvailable = false;

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

    public const float WalkSpeed = 85f;
    public Inventory inventory = new Inventory();

    public override void _Ready()
    {
        SnapToClosestTile(this);
        Stats = new RuntimeStats(actorStats);
        States = new System.Collections.Generic.Dictionary<State, PlayerState>()
        {
            { State.Intro, new IntroState() { Player = this } },
            { State.Idle, new IdleState() { Player = this } },
            { State.FollowParty, new FollowPartyState() { Player = this } },
            { State.Death, new DeathState() { Player = this } },
            { State.Walk, new WalkState() { Player = this } },
            { State.Attack, new AttackState() { Player = this } },
            { State.Combat, new CombatState() { Player = this } },
        };

        SetPlayerState(isControllable ? State.Idle : State.FollowParty);
        LevelUp2D.Visible = false;

        foreach (var enumType in Enum.GetValues<EquipSlot>())
        {
            EquippedItems.Add(enumType, null);
        }

        InitialLearnedTalents();
        EquipAfterGlobalEvents();

        GlobalEvents.Instance.GetExperience += OnExperienceGained;
        GlobalEvents.Instance.OnLevelUp += OnLevelUp;
    }

    private void InitialLearnedTalents()
    {
        foreach (var talent in Stats.StartingSkills)
        {
            LearnedTalents.Add(talent);
            if (talent.PassivesAdded != true && talent.IsMain)
            {
                var talentPassives = talent.GetPassivesBySkillSchool(talent);
                selectableTalents.AddRange(talentPassives);
                talent.PassivesAdded = true;
            }
        }

        selectableTalents =
            selectableTalents.Where(x => !LearnedTalents.Contains(x)).ToList();

        var learnedMainSkills = LearnedTalents.Where(x => x.IsMain).ToList();
        var learnedPassives = LearnedTalents.Where(x => !x.IsMain).ToList();
        for (int i = 0; i < learnedMainSkills.Count(); i++)
        {
            var passiveList = learnedPassives.Where(x =>
                x.SkillLevel == learnedMainSkills[i].SkillLevel).ToList();
            if (passiveList.Count > learnedMainSkills[i].SkillLevel)
            {
                selectableTalents.Add(learnedMainSkills[i]);
            }
        }
        GD.Print(selectableTalents);
    }

    private void OnExperienceGained(double experience)
    {
        Stats.XP += experience;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        if (Stats.XP >= Stats.XPToNextLevel)
        {
            GlobalEvents.Instance.EmitOnLevelUp();
        }
    }

    private void OnLevelUp(object sender, EventArgs e)
    {
        LevelUpAvailable = true;
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
        GlobalEvents.Instance.EmitOnSkillBarChanged(runtimeAbilities, PartyManager.Instance.MainPlayer);
    }

    public void EmitProjectileSignal()
    {
        EmitOnProjectileNeeded();
    }

    public void EquipStartingItems()
    {
        foreach (PackedScene equipabbleItem in startingInventory.EquippableItemScenes)
        {
            var itemScene = equipabbleItem.Instantiate<EquipableItem>();
            AddChild(itemScene);
            inventory.AddItem(itemScene);
            EquippedItems[itemScene.ItemResource.equipSlot] = itemScene;
            GlobalEvents.Instance.EmitEquipSlotChanged(this, itemScene);
            itemScene.OnEquip(this);
            inventory.RemoveItem(itemScene);
            RemoveChild(itemScene);
        }
    }

    public void SetPlayerState(State newState)
    {
        ActiveState?.ExitState();
        ActiveState = States[newState];
        ActiveState.EnterState();
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
        ((CombatState)ActiveState).currentSubState = CombatSubState.CheckStatusEffect;
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