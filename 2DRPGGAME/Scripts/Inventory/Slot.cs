using Godot;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Godot.Collections;

public partial class Slot : TextureRect
{
    [Export] public TextureRect itemIcon = new();
    [Export] public Button itemButton;
    [Export] public Label itemQuantityLabel = new();
    [Export] public EquipSlot equipSlot;
    [Export] public Texture2D EquippedSlotTexture;
    [Export] public EquipSlot BackUpEquipSlot; 
    public Item currentItem = null;
    public WeaponBaseAction currentAbilityAction = null;
    public BaseSkill currentTalent = null;
    public SkillSchools skillSchool;
    public bool EventsSubbed = false;
    public bool CurrentlyEquipped = false;

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
        Texture = null;
        itemQuantityLabel.Text = "";
    }
    
    public void SetItem(Item newItem)
    {
        if (newItem is null) return;
        currentItem = newItem;
        itemIcon.Texture = newItem.ItemResource.Texture;
        itemQuantityLabel.Text = newItem.ItemResource.ItemQuantity.ToString();

        itemQuantityLabel.Visible = false;
        Visible = true;
        if (equipSlot != EquipSlot.Empty)
        {
            Texture = EquippedSlotTexture;
        }
        itemIcon.Visible = true;
    }

    public bool DoesItemTypeMatch(Item item)
    {
        return item.ItemResource.equipSlot == equipSlot || item.ItemResource.equipSlot2 == equipSlot;
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

    public void GetDescription(DescriptionPanelUI descriptionPanel, Slot slot, Resource descType, Vector2 globPos,
        float x, float y)
    {
        Dictionary<DescriptionPanel, BaseDescription> currentDescription;
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

        descriptionPanel.GlobalPosition = globPos + new Vector2(x, y);
        descriptionPanel.AnimationPlayer.Play("Appear");
        descriptionPanel.Scale = new Vector2(0.9f, 0.9f);
        //GD.Print(descriptionPanel.ContainerToResize.Size);
    }
    
    public void SlotUnequip()
    {
        SetSlotsEmpty();
        CurrentlyEquipped = false;
    }

    public void SlotEquip(EquipableItem item)
    {
        SetItem(item);
        CurrentlyEquipped = true;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (itemIcon == null) return default;
        var preview = Duplicate() as Slot;
        var c = new Control();
        c.AddChild(preview);
        preview.Position -= new Vector2(25, 25);
        preview.SelfModulate = Colors.Transparent;

        c.Scale = new Vector2(1.6f, 1.6f);
        SetDragPreview(c);
        GD.Print("dragged? idk");
        return this;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        var dropData = data.AsGodotObject();
        Slot realData = dropData as Slot;
        if (realData == null) return false;
        return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        var dropData = data.AsGodotObject();
        Slot realData = dropData as Slot;
        GlobalEvents.Instance.EmitOnSlotDropped(realData, this);
    }
}