using Godot;
using System;
using Godot.Collections;

public partial class EquipableItemResource : ItemResource
{
	[Export] public  WeaponBaseAction GainSkill;
	[Export] public PassiveFeature PassiveFeature1;
	[Export] public PassiveFeature PassiveFeature2;
	
	public override Dictionary<DescriptionPanel, BaseDescription> GetDescription()
	{
		throw new NotImplementedException();
	}
}
