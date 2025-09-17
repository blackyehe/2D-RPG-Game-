using Godot;
using System;
using static Godot.TextServer;

public partial class IdleState : PlayerState
{
    public override void PhysicsProcess(double delta)
    {
        if (Player.direction != Vector2.Zero && Player.Velocity == Vector2.Zero)
        {
            Player.SetPlayerState(State.Walk);
            return;
        }
    }

    public override void EnterState()
    {
        Player.animationPlayer.Play(AnimTags.Idle);
        

    }

    public override void ExitState()
    {

    }
    protected override void HandleInput()
    {
        if (Input.IsActionPressed(InputTags.LeftClick))
        {
            Player.SetPlayerState(State.Attack);
            return;
        }
        if (Input.IsActionJustPressed("Interact"))
        {
            Player._interactable?.Interact();
        }
        Player.direction = Input.GetVector(InputTags.Left, InputTags.Right, InputTags.Up, InputTags.Down);
    }
}
