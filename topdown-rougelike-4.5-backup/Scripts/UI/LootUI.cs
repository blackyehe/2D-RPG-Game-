using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LootUI : Node
{
    [Export] public TextureRect enemyLootPanel;
    [Export] public TextureRect playerInventory;
    [Export] public CanvasLayer canvasLayer;
    [Export] public GridContainer gridContainer;
    [Export] public TextureRect usagePanel;
    [Export] public MarginContainer detailsPanel;
    [Export] PackedScene slotScene;
    [Export] public RichTextLabel detailsLabel;
    [Export] public Button lootAllButton;
    [Export] public DescriptionPanelUI descriptionPanel;

    public Player Player;
    public Enemy enemy;
    private Slot selectedSlot;

    public static LootUI Instance {get; private set;}
    public override void _Ready()
    {
        Instance = this;
        enemyLootPanel.Visible = false;
        GlobalEvents.Instance.InventoryChanged += Instance_InventoryChanged;
        GlobalEvents.Instance.OnInteract += Instance_OnInteract;
        lootAllButton.Pressed += lootAllButton_Pressed;
    }

    private void Instance_OnInteract(object sender, EventArgs e)
    {
        LootUIOn();
    }

    private void Instance_InventoryChanged(List<Item> items)
    {
        ClearGridContainer();
        foreach (Item item in items)
        {
            var scene = slotScene.Instantiate();
            gridContainer.AddChild(scene);
            Slot slot = scene as Slot;
            if (item != null)
            {
                detailsPanel.Visible = false;
                usagePanel.Visible = false;
                slot.SetCustomMinimumSize(new Vector2(64,64));
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

    public override void _Process(double delta)
    {
    }

    public void LootUIOn()
    {
        if (!canvasLayer.Visible)
        {
            canvasLayer.Visible = true;
            enemyLootPanel.Visible = true;
            GetTree().Paused = !GetTree().Paused;
        }
        else
        {
            enemyLootPanel.Visible = !enemyLootPanel.Visible;
            GetTree().Paused = !GetTree().Paused;
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
            
            PartyManager.Instance.MainPlayer.inventory.AddItem(selectedSlot.currentItem);
            selectedSlot.QueueFree();
            detailsPanel.Visible = false;
        }
    }

    private void lootAllButton_Pressed()
    {
        foreach (Node child in gridContainer.GetChildren())
        {
            Slot slot = child as Slot;
            if (slot == null) return;
            selectedSlot = slot;
            PartyManager.Instance.MainPlayer.inventory.AddItem(selectedSlot.currentItem);
            gridContainer.RemoveChild(selectedSlot);
        }
        enemyLootPanel.Visible = false;
        GetTree().Paused = false;
    }

    public void OnItemButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null && usagePanel.Visible == false)
        {
            if (Input.IsActionPressed(InputTags.LeftClick)) return;
            slot.GetDescription(descriptionPanel,slot, slot.currentItem.ItemResource, slot.GlobalPosition, 80, -40);
        }
    }
    
    public void OnItemButtonMouseExited(Slot slot)
    {
        descriptionPanel.Visible = false;
    }

    private void Timer_Timeout()
    {
        detailsPanel.Visible = true;
    }
}