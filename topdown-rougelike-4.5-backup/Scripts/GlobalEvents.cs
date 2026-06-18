using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class GlobalEvents : Node
{
	public static GlobalEvents Instance {private set; get;}
	
	
	public delegate void InventoryDelegate(List<Item> items);
	public event InventoryDelegate InventoryChanged;
	public void EmitInventoryChanged(List<Item> items) => InventoryChanged?.Invoke(items);
	
	public event EventHandler OnInteract;
	public void EmitOnInteract() => OnInteract?.Invoke(this, EventArgs.Empty);

	
	public delegate void EquipSlotDelegate(Player player,EquipableItem item);
	public event EquipSlotDelegate EquipSlotChanged;
	public void EmitEquipSlotChanged(Player player,EquipableItem item) => EquipSlotChanged?.Invoke(player,item);
	
	
	public delegate void GetExperienceDelegate(double experience);
	public event GetExperienceDelegate GetExperience;
	public void EmitGetExperience(double experience) => GetExperience?.Invoke(experience);
	
	
	public event EventHandler OnLevelUp;
	public void EmitOnLevelUp() => OnLevelUp?.Invoke(this, EventArgs.Empty);
	
	
	public delegate void OnSkillBarChangedDelegate(List<WeaponBaseAction> abilities, Player current);
	public event OnSkillBarChangedDelegate OnSkillBarChanged;
	public void EmitOnSkillBarChanged(List<WeaponBaseAction> abilities, Player current)  => OnSkillBarChanged?.Invoke(abilities,current);

	public delegate void OnSkillContainerChangedDelegate(List<WeaponBaseAction> skills);
	public event OnSkillContainerChangedDelegate OnSkillContainerChanged;
	public void EmitOnSkillContainerChanged(List<WeaponBaseAction> skills) => OnSkillContainerChanged?.Invoke(skills);

	public delegate void OnInventoryStatsUpgradedDelegate(Player player);
	public event OnInventoryStatsUpgradedDelegate OnInventoryStatsUpgraded;
	public void EmitInventoryStatsUpgraded(Player player)  => OnInventoryStatsUpgraded?.Invoke(player);
	
	public delegate void OnTalentLearnedDelegate(BaseSkill Talent);
	public event OnTalentLearnedDelegate OnTalentLearned;
	public void EmitOnTalentLearned(BaseSkill Talent) => OnTalentLearned?.Invoke(Talent);
	
	public delegate void SlotDropped(Slot droppedSlot, Slot slotAtPosition);
	public event SlotDropped OnSlotDropped;
	public void EmitOnSlotDropped(Slot droppedSlot, Slot slotAtPosition) => OnSlotDropped?.Invoke(droppedSlot, slotAtPosition);
	
	public override void _Ready()
	{
		Instance = this;
	}
}
