using Godot;
using System;
using System.Collections.Generic;

public class Inventory
{
	List<Item> _items = new List<Item>();
	public int inventorySize = 50;
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
    public int TestItemCount => _items.Count;

	public void RemoveItem(Item item)
	{
		_items.Remove(item);
        InventoryChanged?.Invoke(_items);

    }
	public List<Item> GetItemList() => _items;
	
	public void InvSizeChange(double strength)
	{
		int plusInventorySize = (int)(strength / 2);
		inventorySize += plusInventorySize; 
	}
}


