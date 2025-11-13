using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Microsoft.VisualBasic.CompilerServices;
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
    [Export] public Array<GridContainer> skillContainer;
    [Export] public Array<Slot> equipSlots;

    [Export] public TextureRect equippedItemPanel;
    [Export] public Button unequipButton;
    [Export] public Button compareButton;

    [Export] public Godot.Collections.Dictionary<characterStats, RichTextLabel> InventoryStats = new();

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
        useButton.Pressed += UseButton_Pressed;
        unequipButton.Pressed += UnequipButtonOnPressed;
        GlobalEvents.Instance.OnTalentLearned += OnPlayerTalentLearned;
        GlobalEvents.Instance.OnInventoryStatsUpgraded += OnInventoryStatsUpgraded;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
        isready = true;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        PartyManager.Instance.MainPlayer.inventory.InventoryChanged += Inventory_InventoryChanged;
        
        SetStartingSkills();
    }

    private void OnMainPlayerChanged(Player previous, Player current)
    {
        previous.inventory.InventoryChanged -= Inventory_InventoryChanged;
        current.inventory.InventoryChanged += Inventory_InventoryChanged;
        Inventory_InventoryChanged(current.inventory.GetItemList());
        ChangeShownEquipSlots(current);
    }
    public void OnPlayerTalentLearned(BaseSkill skill)
    {
        if (skill == null) return;

        for (int i = 0; i < 5; i++)
        {
            if (skillContainer[i].GetChildCount() == 0 && skill.IsMain)
            {
                var scene = SlotScene.Instantiate();
                Slot skillSlot = scene as Slot;
                skillContainer[i].AddChild(skillSlot);
                skillSlot.SetHSizeFlags(SizeFlags.ExpandFill);
                skillSlot.SetVSizeFlags(SizeFlags.ExpandFill);
                skillSlot.itemButton.Visible = false;
                skillSlot.Visible = true;
                skillSlot.SetTalent(skill);
                PartyManager.Instance.MainPlayer.LearnedTalents.Add(skill);
                
                return;
            }

            if (skillContainer[i].GetChildCount() >= 1 && skill.IsMain == false &&
                skillContainer[i].GetChildCount() <= 5)
            {
                var container = skillContainer[i];
                var mainSkill = container.GetChild<Slot>(0);

                if (mainSkill.skillSchool == skill.SkillSchool)
                {
                    var scene = SlotScene.Instantiate();
                    Slot skillSlot = scene as Slot;
                    container.AddChild(skillSlot);
                    skillSlot.SetHSizeFlags(SizeFlags.ExpandFill);
                    skillSlot.SetVSizeFlags(SizeFlags.ExpandFill);
                    skillSlot.itemButton.Visible = false;
                    skillSlot.SetTalent(skill);
                    PartyManager.Instance.MainPlayer.LearnedTalents.Add(skill);
                    
                    return;
                }
            }
        }
    }

    public void SetStartingSkills()
    {
        foreach (var baseSkill in PartyManager.Instance.MainPlayer.Stats.StartingSkills)
        {
            GlobalEvents.Instance.EmitOnTalentLearned(baseSkill);
        }
    }

    private void OnEquipSlotChanged(Player player, Item item)
    {
        SwapEquipSlot(item);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(player);
    }

    public void SwapEquipSlot(Item item)
    {
        var SlottestSlot = equipSlots.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot);
        SlottestSlot.SetItem(item);
        SlottestSlot.itemQuantityLabel.Visible = false;
        SlottestSlot.Visible = true;
        if (SlottestSlot.EventsSubbed) return;
        SlottestSlot.OnSlotEntered += OnItemButtonMouseEntered;
        SlottestSlot.OnSlotExited += OnItemButtonMouseExited;
        SlottestSlot.OnSlotPressed += OnEquippedItemSlotPressed;
        SlottestSlot.EventsSubbed = true;
    }

    public void ChangeShownEquipSlots(Player current)
    {
        foreach (var equipSlot in equipSlots)
        {
            if (equipSlot.currentItem != null)
            {
                equipSlot.Visible = false;
                equipSlot.OnSlotEntered -= OnItemButtonMouseEntered;
                equipSlot.OnSlotExited -= OnItemButtonMouseExited;
                equipSlot.OnSlotPressed -= OnEquippedItemSlotPressed;
                equipSlot.EventsSubbed = false;
            }
        }

        foreach (var equipSlot in current.EquippedItems)
        {
            if (equipSlot.Value != null)
            {
                OnEquipSlotChanged(current,equipSlot.Value);
            }
        }
    }

    private void Inventory_InventoryChanged(List<Item> items)
    {
        ClearGridContainer();
        foreach (Item item in items)
        {
            var scene = SlotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            slot.SetCustomMinimumSize(new Vector2(86, 91));
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

        PartyManager.Instance.MainPlayer.inventory.AddItem(currentlyEquipped);
        var SlothSlot = equipSlots.FirstOrDefault(x => x.equipSlot == currentlyEquipped.ItemResource.equipSlot);
        PartyManager.Instance.MainPlayer.EquippedItems[currentlyEquipped.ItemResource.equipSlot] = null;
        currentlyEquipped.OnUnequip(PartyManager.Instance.MainPlayer);
        SlothSlot.Visible = false;
        SlothSlot.OnSlotEntered -= OnItemButtonMouseEntered;
        SlothSlot.OnSlotPressed -= OnEquippedItemSlotPressed;
        SlothSlot.OnSlotExited -= OnItemButtonMouseExited;
        SlothSlot.EventsSubbed = false;
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

        EquipableItem currentlyEquipped = PartyManager.Instance.MainPlayer.EquippedItems[slotType];

        PartyManager.Instance.MainPlayer.EquippedItems[slotType] = itemToEquip;

        if (currentlyEquipped != null)
        {
            PartyManager.Instance.MainPlayer.inventory.AddItem(currentlyEquipped);
            currentlyEquipped.OnUnequip(PartyManager.Instance.MainPlayer);
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
                PartyManager.Instance.MainPlayer.activeWeapon = (BaseWeapon)itemToEquip;
            }

            GD.Print($"Equipped {itemToEquip.ItemResource.ItemName} to {slotType}");
        }

        SwapEquipSlot(itemToEquip);
        
        itemToEquip.OnEquip(PartyManager.Instance.MainPlayer);


        PartyManager.Instance.MainPlayer.inventory.RemoveItem(itemToEquip);

        usagePanel.Visible = false;
    }

    public void OnItemButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null && usagePanel.Visible == false)
        {
            GetDescription(slot, slot.currentItem.ItemResource);
        }
    }
    public void GetDescription(Slot slot, Resource descType)
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

        descriptionPanel.Scale = new Vector2(1.1f, 1.1f);
        descriptionPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-380, -100);
        descriptionBool = true;
        GetTree().CreateTimer(0.8).Timeout += IsDescriptionBoolTrue;
    }

    public void IsDescriptionBoolTrue()
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