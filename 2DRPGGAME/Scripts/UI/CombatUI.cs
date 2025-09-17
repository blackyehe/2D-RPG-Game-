using Godot;
using System;
using System.Collections.Generic;

public partial class CombatUI : Control
{
    [Export] public Button endTurnButton;
    [Export] public CanvasLayer canvasLayer;
    [Export] public GridContainer spellContainer;
    [Export] public PackedScene SlotScene;
    public Player player;

    public override void _Ready()
    {
        canvasLayer.Visible = false;
        endTurnButton.Pressed += EndTurnButton_Pressed;
        TurnManager.Instance.OnCombatStarted += Instance_OnCombatStarted;
        GlobalEvents.Instance.OnSkillBarChanged += OnSkillBarChanged;
    }

    public void ClearGridContainer()
    {
        while (spellContainer.GetChildCount() > 0)
        {
            var child = spellContainer.GetChild(0);
            spellContainer.RemoveChild(child);
            Slot slot = child as Slot;
            if (slot != null)
            {
                slot.OnSlotEntered -= OnSkillButtonMouseEntered;
                slot.OnSlotExited -= OnSkillButtonMouseExited;
                slot.OnSlotPressed -= OnSkillButtonPressed;
            }

            child.QueueFree();
        }
    }

    private void OnSkillBarChanged(List<WeaponBaseAction> abilities)
    {
        ClearGridContainer();
        foreach (var ability in player.allAbilities)
        {
            var scene = SlotScene.Instantiate();
            spellContainer.AddChild(scene);
            Slot slot = scene as Slot;
            if (ability != null)
            {
                slot.SetCustomMinimumSize(new Vector2(64, 64));
                slot.SetAbility(ability);
            }
            else
            {
                slot.SetSlotsEmpty();
            }

            slot.OnSlotEntered += OnSkillButtonMouseEntered;
            slot.OnSlotExited += OnSkillButtonMouseExited;
            slot.OnSlotPressed += OnSkillButtonPressed;
        }
    }

    public void OnSkillButtonPressed(Slot slot)
    {
        player.currentAbility = slot.currentAbilityAction;
    }

    public void OnSkillButtonMouseEntered(Slot slot)
    {
    }

    public void OnSkillButtonMouseExited(Slot slot)
    {
    }

    private void Instance_OnCombatStarted(object sender, EventArgs e)
    {
        TurnManager.Instance.OnCombatEnded += Instance_OnCombatEnded;
        canvasLayer.Visible = true;
    }

    private void Instance_OnCombatEnded(object sender, EventArgs e)
    {
        canvasLayer.Visible = false;
        TurnManager.Instance.OnCombatEnded -= Instance_OnCombatEnded;
    }

    private void EndTurnButton_Pressed()
    {
        player.EndTurn();
    }

    public override void _Process(double delta)
    {
    }
}