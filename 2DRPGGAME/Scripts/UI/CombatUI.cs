using Godot;
using System;
using System.Collections.Generic;

public partial class CombatUI : Control
{
    [Export] public Button endTurnButton;
    [Export] public CanvasLayer canvasLayer;
    [Export] public GridContainer spellContainer;
    [Export] public PackedScene SlotScene;
    [Export] public DescriptionPanelUI descriptionPanel;
    public Player player;
    private bool descriptionBool;

    public override void _Ready()
    {
        canvasLayer.Visible = false;
        descriptionPanel.Visible = false;
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
        if (slot.currentItem != null)
        {
            var currentDescription = slot.currentItem.ItemResource.GetDescription();
            descriptionPanel.HideAllControlsInDescriptionPanel();
            descriptionPanel.SetDescriptionPanel(currentDescription);
            descriptionPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-60, 60);
            descriptionBool = true;
            GetTree().CreateTimer(0.8).Timeout += isDescriptionBoolTrue;
            return;
        }

        if (slot.currentItem == null && slot.currentAbilityAction != null)
        {
            var CurrentDescription = slot.currentAbilityAction.GetDescription();
            descriptionPanel.HideAllControlsInDescriptionPanel();
            descriptionPanel.SetDescriptionPanel(CurrentDescription);
            descriptionPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-60, -260);
            descriptionBool = true;
            GetTree().CreateTimer(0.8).Timeout += isDescriptionBoolTrue;
        }
    }
    public void isDescriptionBoolTrue()
    {
        if (descriptionBool == false) return;
        descriptionPanel.Visible = true;
    }

    public void OnSkillButtonMouseExited(Slot slot)
    {
        descriptionBool = false;
        descriptionPanel.Visible = false;
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