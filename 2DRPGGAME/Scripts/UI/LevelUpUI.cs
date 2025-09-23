using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelUpUI : Control
{
    [Export] public Panel levelUpPanel;
    [Export] public RichTextLabel GainedStatsLabel;
    [Export] public Slot SelectedSkillSlot;
    [Export] public GridContainer SelectableSkillsContainer;
    [Export] public Panel SelectableSkillPanel;
    [Export] public Button SelectOKButton;
    [Export] public Button LevelUPConfirmButton;
    [Export] public CanvasLayer canvasLayer;
    [Export] public Button LevelUpButton;
    [Export] public PackedScene slotScene;
    [Export] public DescriptionPanelUI  descriptionPanel;
    public Player player;
    private bool descriptionBool = false;
    public override void _Ready()
    {
        canvasLayer.Visible = false;
        LevelUpButton.Visible = false;
        SelectableSkillPanel.Visible = false;
        SelectOKButton.Visible = false;
        LevelUPConfirmButton.Visible = false;
        descriptionPanel.Visible = false;
        GlobalEvents.Instance.OnSkillContainerChanged += OnSkillContainerChanged;
        GlobalEvents.Instance.OnLevelUp += OnPlayerLevelUp;
        LevelUPConfirmButton.Pressed += LevelUPConfirmButtonOnPressed;
        SelectedSkillSlot.itemButton.Pressed += SelectSkillOnPressed;
        SelectOKButton.Pressed += SelectOKButtonOnPressed;
        LevelUpButton.Pressed += LevelUpButtonOnPressed;
    }
    private void OnPlayerLevelUp(object sender, EventArgs e)
    {
        canvasLayer.Visible = true;
        LevelUpButton.Visible = true;
        levelUpPanel.Visible = false;
        SelectableSkillPanel.Visible = false;
        GainedStatsLabel.Text = player.Stats.GetLvlUpStats();
    }
    private void LevelUpButtonOnPressed()
    {
        LevelUpButton.Visible = false;
        levelUpPanel.Visible = true;
        GlobalEvents.Instance.EmitOnSkillContainerChanged(player.selectableAbilities.ToList());
    }

    private void SelectSkillOnPressed()
    {
        SelectableSkillPanel.Visible = !SelectableSkillPanel.Visible;
        GlobalEvents.Instance.EmitOnSkillContainerChanged(player.selectableAbilities.ToList());
    }
    private void SelectOKButtonOnPressed()
    {
        SelectableSkillPanel.Visible = false;
        LevelUPConfirmButton.Visible = true;
    }
    private void LevelUPConfirmButtonOnPressed()
    {
        player.Stats.OnLevelUp();
        GlobalEvents.Instance.EmitOnSkillBarChanged(player.allAbilities);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(player);
        canvasLayer.Visible = false;
    }
    public void ClearGridContainer()
    {
        while (SelectableSkillsContainer.GetChildCount() > 0)
        {
            var child = SelectableSkillsContainer.GetChild(0);
            SelectableSkillsContainer.RemoveChild(child);
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
    private void OnSkillContainerChanged(List<WeaponBaseAction> skill)
    {
        ClearGridContainer();
        foreach (WeaponBaseAction ability in skill)
        {
            var scene = slotScene.Instantiate();
            
            SelectableSkillsContainer.AddChild(scene);
            
            Slot slot = scene as Slot;
            
            slot.SetCustomMinimumSize(new Vector2(64,64));
            slot.itemQuantityLabel.Visible = false;
            
            if (ability!= null)
            {
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
        var abilityToChoose = slot.currentAbilityAction;
        slot.itemQuantityLabel.Visible = false;
        SelectedSkillSlot.itemQuantityLabel.Visible = false;
        if (SelectedSkillSlot.currentAbilityAction != null)
        {
            player.selectableAbilities.Add(SelectedSkillSlot.currentAbilityAction);
            player.allAbilities.Remove(SelectedSkillSlot.currentAbilityAction);
            changeSelectedSlotAbility(abilityToChoose);
        }
        else
        {
            changeSelectedSlotAbility(abilityToChoose);
        }
        SelectOKButton.Visible = true;
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
            descriptionPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-60, 60);
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

    public void changeSelectedSlotAbility(WeaponBaseAction abilityToChoose)
    {
        SelectedSkillSlot.SetAbility(abilityToChoose);
            
        player.allAbilities.Add(abilityToChoose);
        player.selectableAbilities.Remove(abilityToChoose);
            
        GlobalEvents.Instance.EmitOnSkillContainerChanged(player.selectableAbilities.ToList());
    }

    public override void _Process(double delta)
    {
    }
}