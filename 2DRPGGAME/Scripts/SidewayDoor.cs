using Godot;
using System;

public partial class SidewayDoor : StaticBody2D, IInteractable
{
    [Export] public Area2D area2D;
    [Export] public Sprite2D sprite2D;
    [Export] public CollisionShape2D collisionShape2D;
    [Export] public AnimatedSprite2D animatedSprite2D;
    public bool openDoor;
    private Player _player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        area2D.BodyEntered += Area2D_BodyEntered;
        area2D.BodyExited += Area2D_BodyExited;
    }

    private void Area2D_BodyEntered(Node2D body)
    {
        switch (body)
        {
            case Player player:
                _player = player;
                player.SelectInteractable(this);
                break;
        }
    }
    private void Area2D_BodyExited(Node2D body)
    {
        switch (body)
        {
            case Player player:
                player.DeselectInteractable(this);
                break;
        }
    }
    public void Interact()
    {
        openDoor = CollisionLayer == 0;
        CollisionLayer = openDoor ? (uint)1 : 0;
        if (openDoor == false)
        {
            animatedSprite2D.Play("doorOpen");
            animatedSprite2D.FlipH = _player.Position.X > Position.X ? true : false;
        }
        if (openDoor)
        {
            animatedSprite2D.PlayBackwards("doorOpen");
        }

    }
}
