using Godot;
using System;

public partial class InventoryHeaderUI : Control
{
	[Export] public Godot.Collections.Dictionary<characterStats, HeaderBGUI> InventoryStats = new();
	[Export] public RichTextLabel PlayerNameText;
	[Export] public RichTextLabel LevelClassText;
	public override void _Ready()
	{
	}

	
}
