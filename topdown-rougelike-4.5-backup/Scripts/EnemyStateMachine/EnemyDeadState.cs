using Godot;
using System;

public class EnemyDeadState : EnemyState
{
	public override void PhysicsProcess(double delta)
	{
		
	}
	public override void EnterState()
	{
		Enemy.animationPlayer.Play(AnimTags.Death);
	}
    
	public override void ExitState()
	{
	}
}
