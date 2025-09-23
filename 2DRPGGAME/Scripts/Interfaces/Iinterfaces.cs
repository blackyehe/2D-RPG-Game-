using Godot;
using System;
public interface IInteractable
{
	public void Interact();
}
public interface IApplicable 
{
	public void Apply(CombatActor target);
}

public interface IStatusEffect
{
	public bool StatusEffect(CombatActor target);
}
