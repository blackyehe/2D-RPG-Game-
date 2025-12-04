using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Slot : TextureRect
{
    [Export] public TextureRect itemIcon;
    [Export] public Button itemButton;
    [Export] public Label itemQuantityLabel;
    [Export] public EquipSlot equipSlot;
    public Item currentItem = null;
    public WeaponBaseAction currentAbilityAction = null;
    public BaseSkill currentTalent = null;
    public SkillSchools skillSchool;
    public bool EventsSubbed = false;
    public bool Dragged = false;
    public bool CurrentlyEquipped = false;
    private int HoverTimerInt = 0;

    public delegate void SlotPressed(Slot slot);

    public event SlotPressed OnSlotPressed;

    public delegate void SlotExited(Slot slot);

    public event SlotExited OnSlotExited;

    public delegate void SlotEntered(Slot slot);

    public event SlotEntered OnSlotEntered;

    public void SetSlotsEmpty()
    {
        itemIcon.Texture = null;
        currentItem = null;
        currentTalent = null;
        currentAbilityAction = null;
        itemQuantityLabel.Text = "";
    }

    public void SetItem(Item newItem)
    {
        currentItem = newItem;
        itemIcon.Texture = newItem.ItemResource.Texture;
        itemQuantityLabel.Text = newItem.ItemResource.ItemQuantity.ToString();
    }

    public void SetAbility(WeaponBaseAction ability)
    {
        itemIcon.Texture = ability.Sprite;
        currentAbilityAction = ability;
    }

    public void SetTalent(BaseSkill skill)
    {
        itemIcon.Texture = skill.Sprite;
        skillSchool = skill.SkillSchool;
        currentTalent = skill;
    }

    public override void _Ready()
    {
        itemButton.MouseEntered += ItemButton_MouseEntered;
        itemButton.MouseExited += ItemButton_MouseExited;
        itemButton.Pressed += ItemButton_Pressed;
    }

    private void ItemButton_Pressed()
    {
        OnSlotPressed?.Invoke(this);
    }

    private void ItemButton_MouseExited()
    {
        OnSlotExited?.Invoke(this);
    }

    private void ItemButton_MouseEntered()
    {
        OnSlotEntered?.Invoke(this);
    }

    public void GetDescription(DescriptionPanelUI descriptionPanel,Slot slot, Resource descType, Vector2 globPos, float x, float y)
    {
        descriptionPanel.HideAllControlsInDescriptionPanel();
        Godot.Collections.Dictionary<DescriptionPanel, BaseDescription> currentDescription;
        switch (descType)
        {
            case BaseSkill:
                currentDescription = slot.currentTalent.GetDescription(PartyManager.Instance.MainPlayer);
                descriptionPanel.SetDescriptionPanel(currentDescription);
                break;

            case ItemResource:
                currentDescription = slot.currentItem.ItemResource.GetDescription();
                descriptionPanel.SetDescriptionPanel(currentDescription);
                break;

            case WeaponBaseAction:
                currentDescription = slot.currentAbilityAction.GetDescription(PartyManager.Instance.MainPlayer);
                descriptionPanel.SetDescriptionPanel(currentDescription);
                break;
        }

        IsMouseInsideSlot(slot,descriptionPanel);
        //descriptionPanel.SetSize(new Vector2(373,485));
        descriptionPanel.GlobalPosition = globPos + new Vector2(x, y);
    }

    private void IsMouseInsideSlot(Slot slot,DescriptionPanelUI  descriptionPanel)
    {
            descriptionPanel.AnimationPlayer.Play("Appear");
            //descriptionPanel.Visible = true;
            GD.Print(slot);
    }
}