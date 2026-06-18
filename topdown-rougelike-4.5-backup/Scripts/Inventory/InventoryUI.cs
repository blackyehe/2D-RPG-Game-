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
    [Export] public Array<Slot> equipSlotsArray;

    [Export] public TextureRect equippedItemPanel;
    [Export] public Button unequipButton;
    [Export] public Button compareButton;
    [Export] public InventoryHeaderUI InventoryHeader;

    public System.Collections.Generic.Dictionary<EquipSlot, Slot> equipSlotsDict = new();
    private Slot selectedSlot;
    private Slot tempSlot = new();
    public bool isready;

    public override void _Ready()
    {
       
        LootUI.Instance.descriptionPanel = descriptionPanel;
        canvasLayer.Visible = true;
        inventoryPanel.Visible = false;
        wholeInventory.Visible = false;
        FillEquipSlotsDict();
        GlobalEvents.Instance.EquipSlotChanged += OnEquipSlotChanged;
        useButton.Pressed += UseButton_Pressed;
        unequipButton.Pressed += UnequipButtonPressed;
        GlobalEvents.Instance.OnTalentLearned += OnPlayerTalentLearned;
        GlobalEvents.Instance.OnInventoryStatsUpgraded += OnInventoryStatsUpgraded;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
        GlobalEvents.Instance.OnSlotDropped += OnSlotDropped;
        isready = true;
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        PartyManager.Instance.MainPlayer.inventory.InventoryChanged += Inventory_InventoryChanged;
        PartyManager.Instance.MainPlayer.inventory.OnSlotItemEquipped += ItemInSlotEquip;
        PartyManager.Instance.MainPlayer.inventory.OnSlotItemUnequipped += ItemInSlotUnequip;
        HeaderStatAtStart();
        SetStartingSkills();
        SubToEquippedSlotEvents();
    }

    private void FillEquipSlotsDict()
    {
        for (int i = 0; i < equipSlotsArray.Count; i++)
        {
            var equipSlotEnum = equipSlotsArray[i].equipSlot;
            equipSlotsDict.Add(equipSlotEnum, equipSlotsArray[i]);
        }
    }
    private void OnMainPlayerChanged(Player previous, Player current)
    {
        previous.inventory.InventoryChanged -= Inventory_InventoryChanged;
        previous.inventory.OnSlotItemEquipped -= ItemInSlotEquip;
        previous.inventory.OnSlotItemUnequipped -= ItemInSlotUnequip;
        
        current.inventory.OnSlotItemEquipped += ItemInSlotEquip;
        current.inventory.OnSlotItemUnequipped += ItemInSlotUnequip;
        current.inventory.InventoryChanged += Inventory_InventoryChanged;
        
        Inventory_InventoryChanged(current.inventory.GetItemList());
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(current);
        GD.Print(previous.LearnedTalents);
        UpdateShownSkills(current);
        UpdateShownEquipSlots(current);
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

    private void SubToEquipButtonEvents(Slot slot)
    {
        slot.OnSlotEntered += OnItemButtonMouseEntered;
        slot.OnSlotExited += OnItemButtonMouseExited;
        slot.OnSlotPressed += OnEquippedItemSlotPressed;
        slot.EventsSubbed = true;
    }
    private void UnSubToEquipButtonEvents(Slot slot)
    {
        slot.OnSlotEntered -= OnItemButtonMouseEntered;
        slot.OnSlotExited -= OnItemButtonMouseExited;
        slot.OnSlotPressed -= OnEquippedItemSlotPressed;
        slot.EventsSubbed = false;
    }

    private void SubToBaseButtonEvents(Slot slot)
    {
        slot.OnSlotEntered += OnItemButtonMouseEntered;
        slot.OnSlotExited += OnItemButtonMouseExited;
        slot.OnSlotPressed += OnItemButtonPressed;
        slot.EventsSubbed = true;
    }
    private void UnSubToBaseButtonEvents(Slot slot)
    {
        slot.OnSlotEntered -= OnItemButtonMouseEntered;
        slot.OnSlotExited -= OnItemButtonMouseExited;
        slot.OnSlotPressed -= OnItemButtonPressed;
        slot.EventsSubbed = false;
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
    private void SubToEquippedSlotEvents()
    {
        foreach (var equipSlot in equipSlotsArray)
        {
            SubToEquipButtonEvents(equipSlot);
        }
    }
    private void OnEquipSlotChanged(Player player, EquipableItem item)
    {
        SwapEquipSlot(item);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(player);
    }
    //[Benchmark]
    public void SwapEquipSlot(EquipableItem item)
    {
        var slotChangedTo = equipSlotsArray.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot);
        if (slotChangedTo is null) return;
        
       var slotFromMethod = slotChangedTo.GetExactSlot(item);
       
       var finalSlot = equipSlotsArray.FirstOrDefault(x => x.equipSlot == slotFromMethod.equipSlot);
       
       if (finalSlot is null) return;
       
       finalSlot.SlotEquip(item);
      
    }

    public void UpdateShownEquipSlots(Player current)
    {
        foreach (var equipSlot in current.EquippedItems)
        {
            var slotToSet = equipSlotsArray.FirstOrDefault(x => x.equipSlot == equipSlot.Key);
            if (equipSlot.Value is null)
            { 
               slotToSet?.SlotUnequip();
            }
            else
            {
                slotToSet?.SlotEquip(equipSlot.Value);
            }
        }
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(current);
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

            SubToBaseButtonEvents(slot);
        }

        for (int i = 0; i < 30; i++)
        {
            var scene = SlotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            slot?.SetCustomMinimumSize(new Vector2(77, 81));
            slot?.SetSlotsEmpty();
            SubToBaseButtonEvents(slot);
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
               UnSubToBaseButtonEvents(slot);
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
    private void ItemInSlotUnequip(EquipableItem item)
    {
        var equipSlotByItem = equipSlotsArray.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot);
        var backupSlotItem = equipSlotsArray.FirstOrDefault(x => x.equipSlot == item.ItemResource.equipSlot2);
        if (equipSlotByItem is null) return;
        Slot backupSlot = new();
        if (item.ItemResource.equipSlot2 != EquipSlot.Empty && backupSlotItem.CurrentlyEquipped)
        {
            switch (equipSlotByItem.equipSlot)
            {
                case EquipSlot.MainHand:
                    backupSlot.equipSlot = EquipSlot.OffHand;
                    break;
                case EquipSlot.TrinketOne:
                    backupSlot.equipSlot = EquipSlot.TrinketTwo;
                    break;
                case EquipSlot.RingOne:
                    backupSlot.equipSlot = EquipSlot.RingTwo;
                    break;
            }
            var finalSlot = equipSlotsArray.FirstOrDefault(x => x.equipSlot == backupSlot.equipSlot);
            if (finalSlot is null) return;
            finalSlot.SlotUnequip();
            return;
        }
        equipSlotByItem?.SlotUnequip();
    }
    private void UnequipButtonPressed()
    {
        if (selectedSlot == null || selectedSlot.currentItem == null)
            return;

        if (!(selectedSlot.currentItem is EquipableItem currentlyEquipped))
        {
            return;
        }
        var equipSlotByItem = equipSlotsArray.FirstOrDefault(x => x.equipSlot == currentlyEquipped.ItemResource.equipSlot);
        if (equipSlotByItem is null) return;
       
        PartyManager.Instance.MainPlayer.inventory.UnequipItem(currentlyEquipped);
        
        //UnSubToEquipButtonEvents(equipSlotByItem);
        equippedItemPanel.Visible = false;
    }
// refaktorolni az egészet hogy a slotok kezeljék saját magukat.
    private void ItemInSlotEquip(EquipableItem itemToEquip)
    {
        if (itemToEquip is null) return;
        SwapEquipSlot(itemToEquip);
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
        PartyManager.Instance.MainPlayer.inventory.EquipItem(itemToEquip);
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
            UnequipButtonPressed();
        }
        //Item levétel inventoryba ^

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot is EquipSlot.Empty &&
            slotAtPosition.currentItem is null)
        {
            selectedSlot = droppedSlot;
            slotAtPosition.SetItem(selectedSlot.currentItem);
            selectedSlot.SetSlotsEmpty();
        }
        //Item pakolás inventoryn belül, üres slotra ^

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot is EquipSlot.Empty &&
            droppedSlot.currentItem != null && slotAtPosition.currentItem != null)
        {
            selectedSlot = droppedSlot;
            tempSlot.SetItem(selectedSlot.currentItem);
            selectedSlot.SetItem(slotAtPosition.currentItem);
            slotAtPosition.SetItem(tempSlot.currentItem);
            tempSlot.SetSlotsEmpty();
        }
        //Item pakolás inventoryn belül, olyan slotra ami foglalt ^

        if (droppedSlot.equipSlot is EquipSlot.Empty && slotAtPosition.equipSlot != EquipSlot.Empty)
        {
            if (droppedSlot.currentItem?.ItemResource.equipSlot != slotAtPosition.equipSlot &&
                droppedSlot.currentItem?.ItemResource.equipSlot2 != slotAtPosition.equipSlot) return;

            selectedSlot = droppedSlot;
            UseButton_Pressed();
            
        }
        //Item equip inventoryból közvetlen
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