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
    [Export] public Container fakeContainer;
    public override void _Ready()
    {
        canvasLayer.Visible = false;
        endTurnButton.Pressed += EndTurnButton_Pressed;
        TurnManager.Instance.OnCombatStarted += Instance_OnCombatStarted;
        GlobalEvents.Instance.OnSkillBarChanged += OnSkillBarChanged;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
        descriptionPanel.OnDescriptionAppeared += OnDescriptionAppeared;
    }
    
    private void OnMainPlayerChanged(Player previous, Player current)
    {
        GlobalEvents.Instance.EmitOnSkillBarChanged(current.runtimeAbilities, current);
    }

    public void ClearGridContainer()
    {
        while (spellContainer.GetChildCount() > 0)
        {
            var child = spellContainer.GetChild(0);
            spellContainer.RemoveChild(child);
            if (child is Slot slot)
            {
                slot.OnSlotEntered -= OnSkillButtonMouseEntered;
                slot.OnSlotExited -= OnSkillButtonMouseExited;
                slot.OnSlotPressed -= OnSkillButtonPressed;
            }

            child.QueueFree();
        }
    }

    private void OnSkillBarChanged(List<WeaponBaseAction> abilities, Player current)
    {
        ClearGridContainer();
        foreach (var ability in current.runtimeAbilities)
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
        PartyManager.Instance.MainPlayer.currentAbility = slot.currentAbilityAction;
    }

    public void OnSkillButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null)
        {
            
            slot.GetDescription(descriptionPanel,slot, slot.currentItem.ItemResource, slot.GlobalPosition, 360, 330);
            return;
        }

        if (slot.currentItem == null && slot.currentAbilityAction != null)
        {
            descriptionPanel.currentlyHoveredSlot = slot;
            slot.GetDescription(descriptionPanel,slot, slot.currentAbilityAction, slot.GlobalPosition, -100, -250);
            descriptionPanel.GlobalPosition = slot.GlobalPosition +
                                              new Vector2(-100, -descriptionPanel.ContainerToResize.Size.Y);
        }
    }
    private void OnDescriptionAppeared(object sender, EventArgs e)
    {
        GD.Print("After appeared Event" + descriptionPanel.ContainerToResize.Size);
        var descPanelHeight = descriptionPanel.ContainerToResize.Size.Y;
        var scaledPanelHeight = 0.75*descriptionPanel.ContainerToResize.Size.Y; //A scale miatt kell
        float panelHeight = (float)scaledPanelHeight;
        float padding = 50f;
        if (descriptionPanel.currentlyHoveredSlot is null) return;
        descriptionPanel.GlobalPosition = descriptionPanel.currentlyHoveredSlot.GlobalPosition +
                                          new Vector2(-100, -panelHeight - padding);
    }
    
    public void OnSkillButtonMouseExited(Slot slot)
    {
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
        PartyManager.Instance.MainPlayer.EndTurn();
    }

    public override void _Process(double delta)
    {
    }
}