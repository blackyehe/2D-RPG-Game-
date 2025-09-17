using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = System.Range;

public partial class InventoryUI : Control
{
    public Enemy enemy;
    [Export] CanvasLayer canvasLayer;
    [Export] GridContainer gridContainer;
    [Export] public PackedScene SlotScene;
    [Export] public DescriptionPanelUI descriptionPanel;
    
    [Export] public MarginContainer detailsPanel;
    [Export] public RichTextLabel detailsLabel;

    [Export] public TextureRect usagePanel;
    [Export] public Button useButton;
    [Export] public Button dropButton;
    
    [Export] public TextureRect inventoryPanel;
    [Export] public TextureRect wholeInventory;
    [Export] public Array<Slot> equipSlots;

    [Export] public TextureRect equippedItemPanel;
    [Export] public Button unequipButton;
    [Export] public Button compareButton;

    [Export] public Godot.Collections.Dictionary<characterStats,RichTextLabel> InventoryStats = new();
    
    [Export] public RichTextLabel PlayerNameText;
    [Export] public RichTextLabel LevelClassText;
    
    private Slot selectedSlot;
    private bool descriptionBool;
    public bool isready;
    public override void _Ready()
    {
        canvasLayer.Visible = true;
        inventoryPanel.Visible = false;
        wholeInventory.Visible = false;
        descriptionPanel.Visible = false;
        GlobalEvents.Instance.EquipSlotChanged += OnEquipSlotChanged;
        Player.Instance.inventory.InventoryChanged += Inventory_InventoryChanged;
        useButton.Pressed += UseButton_Pressed;
        unequipButton.Pressed += UnequipButtonOnPressed;
        GlobalEvents.Instance.OnInventoryStatsUpgraded += OnInventoryStatsUpgraded;
        isready = true;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(Player.Instance);
    }
    private void OnEquipSlotChanged(Item item)
    {
        SwapEquipSlot(item);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(Player.Instance);
    }

    public void SwapEquipSlot(Item item)
    {
        var SlottestSlot = equipSlots.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot);
        SlottestSlot.SetItem(item);
        SlottestSlot.itemQuantityLabel.Visible = false;
        SlottestSlot.OnSlotEntered += OnItemButtonMouseEntered;
        SlottestSlot.OnSlotExited += OnItemButtonMouseExited;
        SlottestSlot.OnSlotPressed += OnEquippedItemSlotPressed;
        SlottestSlot.Visible = true;
    }

    private void Inventory_InventoryChanged(List<Item> items)
    {
        ClearGridContainer();
        foreach (Item item in items)
        {
            var scene = SlotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            slot.SetCustomMinimumSize(new Vector2(86,91));
            if (item != null)
            {
                detailsPanel.Visible = false;
                usagePanel.Visible = false;
                slot.SetItem(item);
            }
            else
            {
                slot.SetSlotsEmpty();
            }
            slot.OnSlotEntered += OnItemButtonMouseEntered;
            slot.OnSlotExited += OnItemButtonMouseExited;
            slot.OnSlotPressed += OnItemButtonPressed;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UIOn();
    }

    public void UIOn()
    {
        if (Input.IsActionJustPressed("Inventory"))
        {
            inventoryPanel.Visible = !inventoryPanel.Visible;
            wholeInventory.Visible = !wholeInventory.Visible;
            GetTree().Paused = !GetTree().Paused;
            GD.Print("Paused");
        }
    }

    public void ClearGridContainer()
    {
        while (gridContainer.GetChildCount() > 0)
        {
            var child = gridContainer.GetChild(0);
            gridContainer.RemoveChild(child);
            Slot slot = child as Slot;
            if (slot != null)
            {
                slot.OnSlotEntered -= OnItemButtonMouseEntered;
                slot.OnSlotExited -= OnItemButtonMouseExited;
                slot.OnSlotPressed -= OnItemButtonPressed;
            }

            child.QueueFree();
        }
    }

    public void OnItemButtonPressed(Slot slot)
    {
        if (slot.currentItem != null)
        {
            selectedSlot = slot;
            
            useButton.Text = slot.currentItem is EquipableItem ? "Equip" : "Use";
            usagePanel.GlobalPosition = slot.GlobalPosition + new Vector2(-50, 60);
            usagePanel.Visible = !usagePanel.Visible;
            detailsPanel.Visible = false;
        }
    }

    private void OnEquippedItemSlotPressed(Slot slot)
    {
        if (slot.currentItem != null)
        {
            selectedSlot = slot;
            equippedItemPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-50, 60);
            equippedItemPanel.Visible = !equippedItemPanel.Visible;
            detailsPanel.Visible = false;
        }
    }

    private void UnequipButtonOnPressed()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null)
            return;

        if (!(selectedSlot.currentItem is EquipableItem currentlyEquipped))
        {
            return;
        }

        Player.Instance.inventory.AddItem(currentlyEquipped);
        Player.Instance.RemoveCurrentAbilities(currentlyEquipped.ItemResource.equipSlot);
        GlobalEvents.Instance.EmitOnSkillBarChanged(Player.Instance.allAbilities);
        var SlothSlot = equipSlots.FirstOrDefault(x => x.equipSlot == currentlyEquipped.ItemResource.equipSlot);
        Player.Instance.EquippedItems[currentlyEquipped.ItemResource.equipSlot] = null;
        currentlyEquipped.OnUnequip(Player.Instance);
        SlothSlot.Visible = false;
        SlothSlot.OnSlotEntered -= OnItemButtonMouseEntered;
        SlothSlot.OnSlotPressed -= OnEquippedItemSlotPressed;
        SlothSlot.OnSlotExited -= OnItemButtonMouseExited;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(Player.Instance);
        equippedItemPanel.Visible = false;
    }

    private void UseButton_Pressed()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null)
            return;

        if (!(selectedSlot.currentItem is EquipableItem itemToEquip))
        {
            GD.Print($" {selectedSlot.currentItem.GetType()} Not equipable");
            return;
        }

        EquipSlot slotType = itemToEquip.ItemResource.equipSlot;

        EquipableItem currentlyEquipped = Player.Instance.EquippedItems[slotType];

        Player.Instance.EquippedItems[slotType] = itemToEquip;

        if (currentlyEquipped != null)
        {
            Player.Instance.inventory.AddItem(currentlyEquipped);
            currentlyEquipped.OnUnequip(Player.Instance);
            GD.Print($"Swapped out {currentlyEquipped.ItemResource.ItemName} for {itemToEquip.ItemResource.ItemName}");
            var SlothSlot = equipSlots.FirstOrDefault(x => x.equipSlot == currentlyEquipped.ItemResource.equipSlot);
            SlothSlot.OnSlotEntered -= OnItemButtonMouseEntered;
            SlothSlot.OnSlotPressed -= OnEquippedItemSlotPressed;
            SlothSlot.OnSlotExited -= OnItemButtonMouseExited;
        }
        else
        {
            if (slotType is EquipSlot.MainHand)
            {
                Player.Instance.activeWeapon = (BaseWeapon)itemToEquip;
            }

            GD.Print($"Equipped {itemToEquip.ItemResource.ItemName} to {slotType}");
        }

        SwapEquipSlot(itemToEquip);
        
        Player.Instance.GetCurrentAbilityByItem(itemToEquip.ItemResource.equipSlot);
        
        GlobalEvents.Instance.EmitOnSkillBarChanged(Player.Instance.allAbilities);
        
        itemToEquip.OnEquip(Player.Instance);
        
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(Player.Instance);
        
        Player.Instance.inventory.RemoveItem(itemToEquip);

        usagePanel.Visible = false;
    }

    public void OnItemButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null && usagePanel.Visible == false)
        {
            //detailsPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-60, 60);
            //detailsLabel.Text = slot.currentItem.ItemResource.GetDescription();
            var currentDescription = slot.currentItem.ItemResource.GetDescription();
            descriptionPanel.HideAllControlsInDescriptionPanel();
            descriptionPanel.SetDescriptionPanel(currentDescription);
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
    public void OnItemButtonMouseExited(Slot slot)
    {
        descriptionBool = false;
        descriptionPanel.Visible = false;
    }
    void OnInventoryStatsUpgraded(Player player)
    {
        PlayerNameText.Text = player.Stats.Name;
        LevelClassText.Text = $"Level  {player.Stats.CurrentLevel}   {player.Stats.CurrentClass}";
        foreach (var stat in InventoryStats)
        {
            stat.Value.Text = stat.Key switch
            {
                characterStats.Vitality => player.Stats.MaxHP.ToString(),
                characterStats.Defense => player.Stats.MaxDefense.ToString(),
                characterStats.Mana => player.Stats.MaxMP.ToString(),
                characterStats.MagicPower => player.Stats.MaxMagicPower.ToString(),
                characterStats.Agility => player.Stats.MaxAgility.ToString(),
                characterStats.MovementCount => player.Stats.TileMovementCount.ToString(),
                characterStats.Strength => player.Stats.MaxStrength.ToString(),
                characterStats.AttackPower => player.Stats.MaxAttack.ToString(),
                characterStats.Luck => player.Stats.MaxLuck.ToString(),
                characterStats.Charisma => player.Stats.MaxCharisma.ToString(),
                characterStats.ExperiencePoints => player.Stats.XP.ToString(),
            };
        }
    }
}