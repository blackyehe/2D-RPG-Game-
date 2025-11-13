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
    //public Player player;
    private bool descriptionBool;

    public override void _Ready()
    {
        canvasLayer.Visible = false;
        descriptionPanel.Visible = false;
        endTurnButton.Pressed += EndTurnButton_Pressed;
        TurnManager.Instance.OnCombatStarted += Instance_OnCombatStarted;
        GlobalEvents.Instance.OnSkillBarChanged += OnSkillBarChanged;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
    }

    private void OnMainPlayerChanged(Player previous, Player current)
    {
        GlobalEvents.Instance.EmitOnSkillBarChanged(current.runtimeAbilities,current);
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
            var currentDescription = slot.currentItem.ItemResource.GetDescription();
            descriptionPanel.HideAllControlsInDescriptionPanel();
            descriptionPanel.SetDescriptionPanel(currentDescription);
            descriptionPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-60, 60);
            descriptionBool = true;
            GetTree().CreateTimer(0.8).Timeout += IsDescriptionBoolTrue;
            return;
        }

        if (slot.currentItem == null && slot.currentAbilityAction != null)
        {
            var CurrentDescription = slot.currentAbilityAction.GetDescription(PartyManager.Instance.MainPlayer);
            descriptionPanel.HideAllControlsInDescriptionPanel();
            descriptionPanel.SetDescriptionPanel(CurrentDescription);
            fakeContainer.GlobalPosition = slot.GlobalPosition + new Vector2(360, 330);
            descriptionBool = true;
            GetTree().CreateTimer(0.8).Timeout += IsDescriptionBoolTrue;
        }
    }

    public void IsDescriptionBoolTrue()
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
        PartyManager.Instance.MainPlayer.EndTurn();
    }

    public override void _Process(double delta)
    {
    }
}