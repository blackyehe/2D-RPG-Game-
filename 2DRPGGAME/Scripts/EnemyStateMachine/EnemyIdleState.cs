using Godot;
using System;

public partial class EnemyIdleState : EnemyState
{
    public override void PhysicsProcess(double delta)
    {
        Enemy.ViewCircle();
    }
    public override void EnterState()
    {
        Enemy.animationPlayer.Play(AnimTags.Idle);
        GD.Print((object)Enemy.ActiveState);
    }
    
    public override void ExitState()
    {
    }
}
