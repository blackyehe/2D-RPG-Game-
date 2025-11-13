using Godot;
using System;

public partial class ProjectileInstance : Node
{
	public static ProjectileInstance Instance {get; private set;}

	public override void _Ready()
	{
		Instance = this;
	}
}
