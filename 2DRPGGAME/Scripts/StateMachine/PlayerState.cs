using Godot;
using System;

public abstract class PlayerState 
{
	public Player Player; //thumbsup

	public abstract void PhysicsProcess(double delta);

	public abstract void EnterState();

	public abstract void ExitState();

	protected abstract void HandleInput();

	public void HandleInputs()
	{
		if(!Player.isControllable)
		 return; 

		HandleInput();

	}
	
}
