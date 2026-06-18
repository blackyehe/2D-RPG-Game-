using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    List<Item> _items = new();
    public int inventorySize = 50;
    public Player player;

    public bool AddItem(Item item)
    {
        if (_items.Count == inventorySize)
        {
            return false;
        }

        _items.Add(item);
        InventoryChanged?.Invoke(_items);

        return true;
    }

    public delegate void InventoryDelegate(List<Item> items);

    public event InventoryDelegate InventoryChanged;

    public delegate void SlotItemUnequipped(Slot slot);

    public event SlotItemUnequipped OnSlotItemUnequipped;

    public delegate void SlotItemEquipped(EquipableItem itemToEquip);

    public event SlotItemEquipped OnSlotItemEquipped;

    public int TestItemCount => _items.Count;

    public void RemoveItem(Item item)
    {
        GD.Print(_items.FirstOrDefault()?.ItemResource.ItemName);
        _items.Remove(item);
        InventoryChanged?.Invoke(_items);
    }

    public List<Item> GetItemList() => _items;

    public void UnequipItem(Slot slot)
    {
        EquipableItem item = (EquipableItem)slot.currentItem;
        player.inventory.AddItem(item);
        item.OnUnequip(player);
        OnSlotItemUnequipped?.Invoke(slot);
    }

    public void EquipItem(EquipableItem itemToEquip)
    {
        var playerInventory = player.inventory;
        
        if (playerInventory.GetItemList().Contains(itemToEquip))
        {
            playerInventory.RemoveItem(itemToEquip);
        }

        itemToEquip.OnEquip(player);
        OnSlotItemEquipped?.Invoke(itemToEquip);
    }

    public void InvSizeChange(double strength)
    {
        int plusInventorySize = (int)(strength / 2);
        inventorySize += plusInventorySize;
    }
}