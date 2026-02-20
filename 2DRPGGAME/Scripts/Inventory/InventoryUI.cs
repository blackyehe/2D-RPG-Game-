using Godot;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;


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
    [Export] public InventoryHeaderUI InventoryHeader;

    private Slot selectedSlot;
    private Slot tempSlot = new();
    public bool isready;

    public override void _Ready()
    {
        LootUI.Instance.descriptionPanel = descriptionPanel;
        canvasLayer.Visible = true;
        inventoryPanel.Visible = false;
        wholeInventory.Visible = false;
        GlobalEvents.Instance.EquipSlotChanged += OnEquipSlotChanged;
        useButton.Pressed += UseButton_Pressed;
        unequipButton.Pressed += UnequipButtonOnPressed;
        GlobalEvents.Instance.OnTalentLearned += OnPlayerTalentLearned;
        GlobalEvents.Instance.OnInventoryStatsUpgraded += OnInventoryStatsUpgraded;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
        GlobalEvents.Instance.OnSlotDropped += OnSlotDropped;
        isready = true;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        PartyManager.Instance.MainPlayer.inventory.InventoryChanged += Inventory_InventoryChanged;
        HeaderStatAtStart();

        SetStartingSkills();
        
    }

    private void OnMainPlayerChanged(Player previous, Player current)
    {
        previous.inventory.InventoryChanged -= Inventory_InventoryChanged;
        current.inventory.InventoryChanged += Inventory_InventoryChanged;
        Inventory_InventoryChanged(current.inventory.GetItemList());
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(current);
        UpdateShownEquipSlots(current);
        GD.Print(previous.LearnedTalents);
        UpdateShownSkills(current);
    }

    public void OnPlayerTalentLearned(BaseSkill skill)
    {
        if (skill == null) return;

        for (int i = 0; i < 5; i++)
        {
            var container = skillContainer[i];
            var mainSkill = container.GetChild<Slot>(0);
            if (mainSkill != null)
            {
                if (skill.IsMain && mainSkill.skillSchool == skill.SkillSchool)
                {
                    mainSkill.currentTalent = skill;
                    return;
                }
            }

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
                return;
            }

            if (skillContainer[i].GetChildCount() < 1 || skill.IsMain != false ||
                skillContainer[i].GetChildCount() > 5) continue;

            if (mainSkill.skillSchool == skill.SkillSchool)
            {
                var scene = SlotScene.Instantiate();
                Slot skillSlot = scene as Slot;
                container.AddChild(skillSlot);
                skillSlot.SetHSizeFlags(SizeFlags.ExpandFill);
                skillSlot.SetVSizeFlags(SizeFlags.ExpandFill);
                skillSlot.itemButton.Visible = false;
                skillSlot.SetTalent(skill);
                return;
            }
        }
    }

    public void UpdateShownSkills(Player player)
    {
        for (int i = 0; i < skillContainer.Count; i++)
        {
            while (skillContainer[i].GetChildCount() > 0)
            {
                var child = skillContainer[i].GetChild(0);
                skillContainer[i].RemoveChild(child);
            }
        }

        for (int i = 0; i < player.LearnedTalents.Count; i++)
        {
            OnPlayerTalentLearned(player.LearnedTalents[i]);
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
        var slottestSlot = equipSlots.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot);
        slottestSlot.SetItem(item);
        slottestSlot.itemQuantityLabel.Visible = false;
        slottestSlot.Visible = true;
        slottestSlot.Texture = slottestSlot.EquippedSlotTexture;
        slottestSlot.itemIcon.Visible = true;
        if (slottestSlot.EventsSubbed) return;
        slottestSlot.OnSlotEntered += OnItemButtonMouseEntered;
        slottestSlot.OnSlotExited += OnItemButtonMouseExited;
        slottestSlot.OnSlotPressed += OnEquippedItemSlotPressed;
        slottestSlot.EventsSubbed = true;
        slottestSlot.CurrentlyEquipped = true;
    }

    public void UpdateShownEquipSlots(Player current)
    {
        foreach (var equipSlot in equipSlots)
        {
            if (equipSlot.currentItem != null)
            {
                equipSlot.Visible = false;
                equipSlot.OnSlotEntered -= OnItemButtonMouseEntered;
                equipSlot.OnSlotExited -= OnItemButtonMouseExited;
                equipSlot.OnSlotPressed -= OnEquippedItemSlotPressed;
                equipSlot.currentItem = null;
                equipSlot.EventsSubbed = false;
                equipSlot.CurrentlyEquipped = false;
            }
        }

        foreach (var equipSlot in current.EquippedItems)
        {
            if (equipSlot.Value != null)
            {
                OnEquipSlotChanged(current, equipSlot.Value);
                
            }
        }

        foreach (var equipSlot in equipSlots)
        {
            if (equipSlot.currentItem is null)
            {
                equipSlot.Visible = true;
                equipSlot.OnSlotEntered += OnItemButtonMouseEntered;
                equipSlot.OnSlotExited += OnItemButtonMouseExited;
                equipSlot.OnSlotPressed += OnEquippedItemSlotPressed;
                equipSlot.EventsSubbed = true;
                equipSlot.Texture = null;
                equipSlot.itemIcon.Visible = false;
                equipSlot.CurrentlyEquipped = false;
            }
        }
        
        GD.Print(current.EquippedItems[EquipSlot.Ranged]);
        
    }

    private void Inventory_InventoryChanged(List<Item> items)
    {
        ClearGridContainer();
        foreach (Item item in items)
        {
            var scene = SlotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            slot.SetCustomMinimumSize(new Vector2(77, 81));
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

        for (int i = 0; i < 30; i++)
        {
            var scene = SlotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            slot.SetCustomMinimumSize(new Vector2(77, 81));
            slot.SetSlotsEmpty();
            slot.OnSlotEntered += OnItemButtonMouseEntered;
            slot.OnSlotExited += OnItemButtonMouseExited;
            slot.OnSlotPressed += OnItemButtonPressed;
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
            HeaderStatVisibilty();
            //EquippedInventoryDragPanel.Visible = !EquippedInventoryDragPanel.Visible;
            GetTree().Paused = !GetTree().Paused;
            GD.Print("Paused");
        }
    }

    public void HeaderStatVisibilty()
    {
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        foreach (var stat in InventoryHeader.InventoryStats)
        {
            stat.Value.Visible = !stat.Value.Visible;
        }
    }

    private void HeaderStatAtStart()
    {
        foreach (var stat in InventoryHeader.InventoryStats)
        {
            stat.Value.Visible = false;
        }
    }

    public void OnItemButtonPressed(Slot slot)
    {
        if (slot.currentItem == null) return;

        selectedSlot = slot;

        useButton.Text = slot.currentItem is EquipableItem ? "Equip" : "Use";
        usagePanel.GlobalPosition = slot.GlobalPosition + new Vector2(-50, 60);
        usagePanel.Visible = !usagePanel.Visible;
        descriptionPanel.Visible = false;
    }

    private void OnEquippedItemSlotPressed(Slot slot)
    {
        if (slot.currentItem == null) return;

        selectedSlot = slot;
        equippedItemPanel.GlobalPosition = slot.GlobalPosition + new Vector2(-50, 60);
        equippedItemPanel.Visible = !equippedItemPanel.Visible;
        descriptionPanel.Visible = false;
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
        var equipSlotByItem = equipSlots.FirstOrDefault(x => x.equipSlot == currentlyEquipped.ItemResource.equipSlot);
        PartyManager.Instance.MainPlayer.EquippedItems[currentlyEquipped.ItemResource.equipSlot] = null;
        currentlyEquipped.OnUnequip(PartyManager.Instance.MainPlayer);
        if (equipSlotByItem is null) return;
        equipSlotByItem.SetSlotsEmpty();
        equipSlotByItem.OnSlotEntered -= OnItemButtonMouseEntered;
        equipSlotByItem.OnSlotExited -= OnItemButtonMouseExited;
        equipSlotByItem.OnSlotPressed -= OnEquippedItemSlotPressed;
        UpdateShownEquipSlots(PartyManager.Instance.MainPlayer);
        equipSlotByItem.CurrentlyEquipped = false;
        equippedItemPanel.Visible = false;
    }

    private void UseButton_Pressed()
    {
        if (selectedSlot?.currentItem == null)
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

        UpdateShownEquipSlots(PartyManager.Instance.MainPlayer);
        usagePanel.Visible = false;
    }

    public void OnItemButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null && usagePanel.Visible == false)
        {
            slot.GetDescription(descriptionPanel, slot, slot.currentItem.ItemResource, slot.GlobalPosition, 410, 70);
        }
    }

    public void OnItemButtonMouseExited(Slot slot)
    {
        descriptionPanel.Visible = false;
        if (Input.IsActionPressed(InputTags.LeftClick)) return;
    }

    private void OnSlotDropped(Slot droppedSlot, Slot slotAtPosition)
    {
        if (droppedSlot == slotAtPosition) return;
        if (slotAtPosition.currentTalent != null) return;
        if (droppedSlot.equipSlot != EquipSlot.Empty && slotAtPosition.equipSlot is EquipSlot.Empty)
        {
            selectedSlot = droppedSlot;
            UnequipButtonOnPressed();
        }

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot is EquipSlot.Empty && slotAtPosition.currentItem is null)
        {
            selectedSlot = droppedSlot;
            slotAtPosition.SetItem(selectedSlot.currentItem);
            selectedSlot.SetSlotsEmpty();
        }

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot is EquipSlot.Empty &&
            droppedSlot.currentItem != null && slotAtPosition.currentItem != null)
        {
            selectedSlot = droppedSlot;
            tempSlot.SetItem(selectedSlot.currentItem);
            selectedSlot.SetItem(slotAtPosition.currentItem);
            slotAtPosition.SetItem(tempSlot.currentItem);
            tempSlot.SetSlotsEmpty();
        }

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot != EquipSlot.Empty)
        {
            if (droppedSlot.currentItem?.ItemResource.equipSlot != slotAtPosition.equipSlot) return;
            selectedSlot = droppedSlot;
            UseButton_Pressed();    
            
        }
        
    }
    
    void OnInventoryStatsUpgraded(Player player)
    {
        InventoryHeader.PlayerNameText.Text = player.Stats.Name;
        InventoryHeader.LevelClassText.Text = $"Level  {player.Stats.CurrentLevel}   {player.Stats.CurrentClass}";
        foreach (var stat in InventoryHeader.InventoryStats)
        {
            stat.Value.Number.Text = stat.Key switch
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