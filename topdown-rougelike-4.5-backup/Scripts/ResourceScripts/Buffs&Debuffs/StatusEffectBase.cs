using Godot;
using System;

public partial class StatusEffectBase : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public double Duration;
    [Export] public bool IsStackable;
}

