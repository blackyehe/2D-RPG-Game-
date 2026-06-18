using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class Enemy : CombatActor, IInteractable
{
    public const float EnemyWalkSpeed = 100f;
    [Export] public Area2D swingArea2D;
    [Export] public Behaviour behaviour;
    [Export] public InventoryUI inventoryUI;
    [Export] public LootUI lootUI;
    [Export] public StartingInventory startingInventory;
    public Inventory  inventory = new Inventory();

    public int radius = 3;
    public Player _player;
    public EnemyState ActiveState;
    public Dictionary<StateEnemy, EnemyState> States;
    public Vector2[] MovementPath;

    public Queue<CombatAction> combatActions = new();
    public override void _Ready()
    {
        inventoryUI.enemy = this;
        Stats = new RuntimeStats(actorStats);
        _player = GetThePlayer();
        deadArea2D.BodyEntered += DeadArea2DOnBodyEntered;
        deadArea2D.BodyExited += DeadArea2DOnBodyExited;
        
        foreach (var enumType in Enum.GetValues<EquipSlot>())
        {
            EquippedItems.Add(enumType,null);
        }
        foreach(PackedScene equipableItem in startingInventory.EquippableItemScenes)
        {
            var itemScene = equipableItem.Instantiate<EquipableItem>();
            AddChild(itemScene);
            inventory.AddItem(itemScene);
            RemoveChild(itemScene);
            EquippedItems[itemScene.ItemResource.equipSlot] = itemScene;
        }
        
        SetActiveWeapon();

        States = new Dictionary<StateEnemy, EnemyState>()
        {
            {StateEnemy.EnemyIdle,new EnemyIdleState() {Enemy = this} },
            {StateEnemy.Alert, new AlertState() {Enemy = this }},
            {StateEnemy.EnemyAttack,new  EnemyAttackState(){Enemy = this}},
            {StateEnemy.EnemyCombat,new EnemyCombatState(){Enemy = this}},
            {StateEnemy.EnemyDead,new EnemyDeadState(){Enemy = this}},
        };
        SetEnemyState(StateEnemy.EnemyIdle);
    }

    private Player GetThePlayer()
    {
        return PartyManager.Instance.PlayerParty.Find(x => x.isControllable);
    }
    private void DeadArea2DOnBodyEntered(Node2D body)
    {
        _player = GetThePlayer();
        switch (body)
        {
            case Player player:
                _player = player;
                if (IsAlive) return;
                player.SelectInteractable(this);
                break;
        }
    }
    private void DeadArea2DOnBodyExited(Node2D body)
    {
        switch (body)
        {
            case Player player:
                player.DeselectInteractable(this);
                break;
        }
    }
    public void Interact()
    {
        if (IsAlive) return;

        List<Item> disposeItemsList = inventory.GetItemList();
        GlobalEvents.Instance.EmitInventoryChanged(inventory.GetItemList());
        disposeItemsList.Clear();
        
        if (Input.IsActionJustPressed("Interact"))
        {
            GlobalEvents.Instance.EmitOnInteract();
        }
        
    }
    public int GetDistanceToTile(CombatActor target)
    {
        var targetsPos = TurnManager.Instance.GetTilePosition(target.GlobalPosition);
        var ownerEntityPos = TurnManager.Instance.GetTilePosition(GlobalPosition);
        Vector2I[] tilePath = Pathfinding.Instance.FindTilePath(ownerEntityPos, targetsPos);
        return tilePath.Length-2;
    }

    public void ViewCircle()
    {
        var center = TurnManager.Instance.GetTilePosition(GlobalPosition);
        var playerTile = TurnManager.Instance.GetTilePosition(GetThePlayer().GlobalPosition);
        int startVal = radius * 2 - 1;
        
        for (int x = 1; x <= radius; x++)
        {
            int direction = facingLeft ? -x : x;
            int boundsY = startVal / 2;
            
            for (int y = -boundsY; y <= boundsY; y++)
            {
                Vector2I tilePos = new Vector2I(center.X + direction, center.Y + y);
                
                if (tilePos == playerTile)
                {
                    RegisterEnemyGroup();
                    RegisterPlayerGroup();
                    TurnManager.Instance.CombatTransition();
                    TurnManager.Instance.StartCombat();
                    return;
                }
            }
            startVal -= 2;
        }
    }

    private void RegisterEnemyGroup()
    {
       Node parentGroup = GetParent();
       foreach (var enemy in parentGroup.GetChildren())
       {
           TurnManager.Instance.RegisterCombatant(enemy as Enemy);
       }
    }

    private void RegisterPlayerGroup()
    {
        Node parentGroup = PartyManager.Instance.MainPlayer.GetParent();
        foreach (var player in parentGroup.GetChildren())
        {
            TurnManager.Instance.RegisterCombatant(player as Player);
        }
    }

    
    public void SetEnemyState(StateEnemy newState)
    {
        ActiveState?.ExitState();
        ActiveState = States[newState];
        ActiveState.EnterState();
    }
    public override void EnterCombat()
    {
        SetEnemyState(StateEnemy.EnemyCombat);
        SnapToClosestTile(this);
        behaviour.ownerEntity = this;
    }
    public override void StartTurn()
    {
        IsTurnActive = true;
        ((EnemyCombatState)ActiveState).currentSubState = CombatSubState.CheckStatusEffect;
        Pathfinding.Instance.SetTileSolid(GlobalPosition,false,this);
    }
    public void AddAction(CombatAction action)
    {
        combatActions.Enqueue(action);
    }
    public override void ExitCombat()
    {
        IsTurnActive = false;
    }
    public override void _Process(double delta)
	{
        ActiveState.PhysicsProcess(delta);
        MoveAndSlide();
    }
}
