using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class DescriptionPanelUI : VBoxContainer
{
    [Export] public Godot.Collections.Dictionary<DescriptionPanel, Control> DescriptionPanel = new();
    [Export] public Array<Container> ContainersToDisable = new();
    [Export] public AnimationPlayer AnimationPlayer;
    [Export] public PanelContainer ContainerToResize;
    private bool descriptionBool;
    public Slot currentlyHoveredSlot;

    public EventHandler OnDescriptionAppeared;
    public void DescriptionAppeared() => OnDescriptionAppeared?.Invoke(this, EventArgs.Empty);

    public void AnimationPlayerFinished()
    {
        currentlyHoveredSlot = null;
    }

    public void SetDescriptionPanel(Godot.Collections.Dictionary<DescriptionPanel, BaseDescription> descriptions)
    {
        Visible = true;
        foreach (var control in DescriptionPanel.Values)
        {
            control.Visible = false;
            switch (control)
            {
                case RichTextLabel rLabel:
                    rLabel.Text = "";
                    break;
                case TextureRect tRect:
                    tRect.Texture = null;
                    break;
            }

            var parent = control.GetParent() as Control;

            if (parent == null) continue;
            parent.Visible = false;
        }

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

        var containersToDisable = ContainersToDisable.ToList();
        for (int i = 0; i < containersToDisable.Count; i++)
        {
            ToggleContainerVisibility(containersToDisable[i]);
        }
    }

    public void ToggleContainerVisibility(CanvasItem container)
    {
        var containerGetChildren = container.GetChildren().ToList();
        
        List<Container> containerChildren = new();
        
        for (int i = 0; i < containerGetChildren.Count; i++)
        {
            if (!containerChildren.Contains(containerGetChildren[i]))
            {
                containerChildren.Add(containerGetChildren[i] as Container);
            }
        }

        container.Visible = containerChildren.Any(x => x.Visible);
    }

    public override void _Ready()
    {
        Visible = false;
    }
}