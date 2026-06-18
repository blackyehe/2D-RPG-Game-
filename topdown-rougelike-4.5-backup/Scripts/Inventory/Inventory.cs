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

    public delegate void SlotItemUnequipped(EquipableItem item);

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

    public void UnequipItem(EquipableItem item)
    {
        player.inventory.AddItem(item);
        item.OnUnequip(player);
        OnSlotItemUnequipped?.Invoke(item);
    }

    public void EquipItem(EquipableItem itemToEquip)
    {
        var playerInventory = player.inventory;
        var playerEquippedItems = player.EquippedItems;
        var itemToEquipSlot = 
            playerEquippedItems.FirstOrDefault(x => x.Key == itemToEquip.ItemResource.equipSlot);
        
        if (playerInventory.GetItemList().Contains(itemToEquip))
        {
            playerInventory.RemoveItem(itemToEquip);
        }

        if (itemToEquipSlot.Value != null && itemToEquip.ItemResource.equipSlot2 is EquipSlot.Empty)
        {
            var itemToUnequip = itemToEquipSlot.Value;
            playerInventory.UnequipItem(itemToUnequip);
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