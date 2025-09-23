using Godot;
using System;

public partial class DeathState : PlayerState
{
    public override void PhysicsProcess(double delta)
    {

    }

    public override void EnterState()
    {
        GD.Print(Player.ActiveState);
        Player.animationPlayer.Play(AnimTags.Death);
    }

    public override void ExitState()
    {

    }
    protected override void HandleInput()
    {

    }
}
