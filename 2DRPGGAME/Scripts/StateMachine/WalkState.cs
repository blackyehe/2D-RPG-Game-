using Godot;
using System;
using static Godot.TextServer;

public partial class WalkState : PlayerState
{
    public override void PhysicsProcess(double delta)
    {
        Vector2 velocity = Player.Velocity;

        if (Player.direction.X != 0)
        {
            Player.facingLeft = Player.direction.X < 0;
        }
        Player.Sprite.FlipH = velocity.X < 0 || Player.facingLeft == true;

        if (Player.facingLeft == true)
        {
            Player.swingArea2D.RotationDegrees = 180;
        }
        else { Player.swingArea2D.RotationDegrees = 0; }
        if (Player.direction != Vector2.Zero)
        {
            velocity.X = Player.direction.X * Player.WalkSpeed;
            velocity.Y = Player.direction.Y * Player.WalkSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Player.Velocity.X, 0, Player.WalkSpeed);
            velocity.Y = Mathf.MoveToward(Player.Velocity.Y, 0, Player.WalkSpeed);
        }
        Player.Velocity = velocity; 
        if (Player.direction.X == 0 && Player.direction.Y == 0)
        {
            Player.SetPlayerState(State.Idle);
            return;
        }
    }

    public override void EnterState()
    {
        Player.animationPlayer.Play(AnimTags.Walk);
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
