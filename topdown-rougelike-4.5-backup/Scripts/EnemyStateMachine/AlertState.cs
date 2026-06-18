using Godot;
using System;
using static Godot.TextServer;

public partial class AlertState : EnemyState
{
    

    public override void PhysicsProcess(double delta)
    {
       /* Enemy.Position += (Enemy._player.Position - Enemy.Position).Normalized() / Enemy.Speed;
        if (Enemy.Position.DistanceSquaredTo(Enemy._player.Position) < 230)
        {
            Enemy.SetEnemyState(StateEnemy.EnemyAttack);
        }
        Vector2 justNameIt = (Enemy._player.Position - Enemy.Position).Normalized();
        if (justNameIt != Vector2.Zero)
        {
            Enemy.enemyFacingLeft = justNameIt < Vector2.Zero;
        }
        Enemy.animatedSprite.FlipH = justNameIt < Vector2.Zero || Enemy.facingLeft == true;
        Enemy.animatedSprite.FlipV = false;*/
    }

    public override void EnterState()
    {
        
        GD.Print((object)Enemy.ActiveState);
    }

    public override void ExitState()
    {
    }
}
