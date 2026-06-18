using Godot;
using System;

public partial class GlobalCamera : Camera2D
{
	public override void _Ready()
	{
		PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
	}
	private void OnMainPlayerChanged(Player previous, Player current)
	{
		Reparent(current);
		Position = Vector2.Zero;
	}
}
