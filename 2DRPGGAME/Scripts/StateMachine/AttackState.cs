using Godot;
using System;

public class AttackState : PlayerState
{
    public override void PhysicsProcess(double delta)
    {
        if(Player.animationPlayer.IsPlaying() == false)
        {
            AnimationPlayer_AnimationFinished(null);
        }
    }

    public override void EnterState()
    {
        Player.Velocity = Vector2.Zero;
        Player.direction = Vector2.Zero;
        GD.Print(Player.ActiveState);
        Player.animationPlayer.Play(AnimTags.LightAttack);
        Player.animationPlayer.AnimationFinished += AnimationPlayer_AnimationFinished;
        
    }

    private void AnimationPlayer_AnimationFinished(StringName animName)
    {
        Player.SetPlayerState(State.Idle);

    }

    public override void ExitState()
    {
        Player.animationPlayer.AnimationFinished -= AnimationPlayer_AnimationFinished;
    }
    protected override void HandleInput()
    {
    }
}
