using Godot;
using System;
using Godot.Collections;

public partial class DescriptionPanelUI : VBoxContainer
{
	[Export] public Dictionary<DescriptionPanel, Control> DescriptionPanel = new();

	public void HideAllControlsInDescriptionPanel()
	{
		foreach (var control in DescriptionPanel.Values)
		{
			control.Visible = false;
			
			var parent = control.GetParent() as Control; 
			
			if (parent == null) continue;
			
			parent.Visible = false;
		}
	}
	public void SetDescriptionPanel(Dictionary<DescriptionPanel, BaseDescription> descriptions)
	{
		foreach (var valuePair in descriptions)
		{
			if (valuePair.Value.textureData is null && string.IsNullOrEmpty(valuePair.Value.stringData))
				continue;
			
			var currentControl = DescriptionPanel[valuePair.Key];
			
			var parent = currentControl.GetParent() as Control;
			
			if (parent == null) continue;
			
			if (currentControl is RichTextLabel label)
			{
				label.Text = valuePair.Value.stringData;
				parent.Visible = true;
			}
			else if (currentControl is TextureRect texture)
			{
				texture.Texture = valuePair.Value.textureData;
				parent.Visible = true;
			}
			currentControl.Visible = true;
		}
	}
	
	public override void _Ready()
	{
	}
	
}
