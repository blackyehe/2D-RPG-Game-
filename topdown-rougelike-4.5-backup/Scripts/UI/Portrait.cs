using Godot;
using System;

public partial class Portrait : PanelContainer
{
	[Export] public TextureRect PortraitTexture;
	[Export] public Button PortraitButton;
	[Export] public RichTextLabel HPLabel;
	public Player Player;
	public delegate void PortraitPressed(Portrait portrait);
	public event PortraitPressed OnPortraitPressed;
	public delegate void PortraitExited(Portrait portrait);
	public event PortraitExited OnPortraitExited;
	public delegate void PortraitEntered(Portrait portrait);
	public event PortraitEntered OnPortraitEntered;
	public override void _Ready()
	{
		PortraitButton.Pressed += PortraitButtonOnPressed;
		PortraitButton.MouseEntered += PortraitButtonOnMouseEntered;
		PortraitButton.MouseExited += PortraitButtonOnMouseExited;
	}

	public void SetPortrait(Player player)
	{
		Player = player;
		PortraitTexture = player.PlayerPortrait;
		HPLabel.Text = player.Stats.HP > 0 ? $"{player.Stats.HP} / {player.Stats.MaxHP}" : "Dead";
	}
	
	private void PortraitButtonOnPressed()
	{
		OnPortraitPressed?.Invoke(this);	
	}
	private void PortraitButtonOnMouseExited()
	{
		OnPortraitExited?.Invoke(this);
	}
	private void PortraitButtonOnMouseEntered()
	{
		OnPortraitEntered?.Invoke(this);
	}
	
}
