using Godot;
using System;

public partial class DummyCursorTarget : CombatActor
{
	public override void _Ready()
	{
		Stats = new RuntimeStats(actorStats);
		
	}
	public override void EnterCombat()
	{
		throw new NotImplementedException();
	}

	public override void StartTurn()
	{
		throw new NotImplementedException();
	}

	public override void ExitCombat()
	{
		throw new NotImplementedException();
	}
}
