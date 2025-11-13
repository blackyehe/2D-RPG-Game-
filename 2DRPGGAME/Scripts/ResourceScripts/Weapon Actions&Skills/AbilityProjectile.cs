using Godot;
using System;

public partial class AbilityProjectile : Area2D
{
	[Export] public Sprite2D Sprite;
	[Export] public CollisionShape2D CollisionShape;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public float ProjectileSpeed;
	public Vector2 SpawnPos;
	public CombatActor User;
	public CombatActor Target;
	
	public override void _Ready()
	{
		GlobalPosition = SpawnPos;
	}

	public override void _Process(double delta)
	{
		GlobalPosition += (Target.GlobalPosition - GlobalPosition).Normalized() * ProjectileSpeed;
	}
	
}
