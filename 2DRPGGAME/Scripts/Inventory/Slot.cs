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
    public WeaponBaseAction  currentAbilityAction = null;
    
    public void SetSlotsEmpty()
    {
        itemIcon = null;
        itemQuantityLabel.Text = "";
    }
    public void SetItem(Item newItem)
    {
        currentItem = newItem;
        itemIcon.Texture = newItem.ItemResource.Texture;
        itemQuantityLabel.Text  = newItem.ItemResource.ItemQuantity.ToString();
    }

    public void SetAbility(WeaponBaseAction ability)
    {
       itemIcon.Texture = ability.Sprite;
       currentAbilityAction = ability;
    }
    public override void _Ready()
    {
        itemButton.MouseEntered += ItemButton_MouseEntered;
        itemButton.MouseExited += ItemButton_MouseExited;
        itemButton.Pressed += ItemButton_Pressed;
    }

    public delegate void SlotPressed(Slot slot);
    public event SlotPressed OnSlotPressed;

    public delegate void SlotExited(Slot slot);
    public event SlotExited OnSlotExited;

    public delegate void SlotEntered(Slot slot);
    public event SlotEntered OnSlotEntered;
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
}
