using Godot;
using System;

public abstract class CombatAction 
{
	public CombatActor actor;
	public bool isActionFinished;
	public abstract bool IsLegal();

	public abstract bool DoAction(double delta);
	
	
}
