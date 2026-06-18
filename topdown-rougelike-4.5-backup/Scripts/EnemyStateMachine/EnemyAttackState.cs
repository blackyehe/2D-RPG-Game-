using Godot;
using System;

public partial class EnemyAttackState : EnemyState
{

    public override void PhysicsProcess(double delta)
    {
        Vector2 justNameIt = (Enemy._player.Position - Enemy.Position).Normalized();
        float angle = justNameIt.Angle();
        Enemy.swingArea2D.GlobalPosition = Enemy.GlobalPosition + justNameIt * 13;
        Enemy.swingArea2D.Rotation = angle;
        
    }

    public override void EnterState()
    {
        Enemy.animationPlayer.AnimationFinished += AnimationPlayer_AnimationFinished;
        GD.Print(Enemy.ActiveState);
        Enemy.animationPlayer.Play(AnimTags.LightAttack);
    }

    private void AnimationPlayer_AnimationFinished(StringName animName)
    {
        Enemy.SetEnemyState(StateEnemy.Alert);

    }

    public override void ExitState()
    {
        Enemy.animationPlayer.AnimationFinished -= AnimationPlayer_AnimationFinished;
    }
}
