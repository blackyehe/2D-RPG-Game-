using Godot;
using System;

public class IntroState : PlayerState
{

    public override void EnterState()
    {
        GD.Print(Player.ActiveState);

    }

    public override void ExitState()
    {

    }
    public override void PhysicsProcess(double delta)
    {
        if (Player.animationPlayer.IsPlaying() == false)
        {
            Player.SetPlayerState(State.Idle);
            return;
        }
    }
    protected override void HandleInput()
    {

    }
}
