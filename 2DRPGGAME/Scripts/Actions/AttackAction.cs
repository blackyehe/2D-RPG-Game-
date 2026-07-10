using System;
using Godot;
using NewGameProject.Scripts;

public class AttackAction : CombatAction
{
    private Vector2 _targetTilePos;
    private double _actionTimer = 0;
    private double _actionDuration;
    private BaseWeapon _weapon;
    private CombatActor _target;
    private StringName currentAnimation;
    private int valtozo;
    private bool alreadySubbed = false;
    private AbilityProjectile projectile;

    public override bool IsLegal()
    {
        if (!actor.IsEntityInRange(_target)) return false;
        foreach (var actionCost in (actor.currentAbility.ActionCosts))
        {
            if (actionCost.Value > actor.Stats.remainingCost[actionCost.Key]) return false;
        }

        actor.animationPlayer.AnimationFinished += currentAnimation => valtozo++;
        actor.OnProjectileNeeded += ActorOnOnProjectileNeeded;
        return true;
    }

    private void ActorOnOnProjectileNeeded(object sender, EventArgs e)
    {
        projectile = actor.currentAbility.SpawnProjectile(actor, _target);
        projectile.AnimationPlayer.Play(AnimTags.Shoot);
        projectile.BodyEntered += enemyArea =>
        {
            if (enemyArea == _target)
            {
                projectile.AnimationPlayer.Play(AnimTags.Disperse);
                projectile.AnimationPlayer.AnimationFinished += Disperse => projectile.QueueFree(); 
               // projectile.QueueFree();
                valtozo++;
            }
        };
    }

    public override bool DoAction(double delta)
    {
        switch (valtozo)
        {
            case 0:
            {
                bool newFacingLeft = _target.GlobalPosition.X < actor.GlobalPosition.X;
                actor.facingLeft = newFacingLeft;
                actor.animationPlayer.Play(actor.facingLeft ? AnimTags.TurnLeft : AnimTags.TurnRight);
                valtozo++;
                break;
            }
            case 1:
            {
                actor.animationPlayer.Play(currentAnimation);

                break;
            }
            case 2:
            {
                if (!actor.currentAbility.HasProjectileOrSecondaryAnimation ||
                    actor.currentAbility.ProjectileScene == null)

                    valtozo++;
                break;
            }
            case 3:
            {
                actor.currentAbility.DoAction(actor, _target);
                (actor.currentAbility.StatusEffect as IApplicable)?.Apply(_target);
                isActionFinished = true;
                actor.OnProjectileNeeded -= ActorOnOnProjectileNeeded;
                return true;
            }
        }

        return false;
    }

    public AttackAction(CombatActor user, CombatActor target, WeaponBaseAction ability)
    {
        _target = target;
        actor = user;
        user.currentAbility = ability;

        _actionTimer = 0;
        _actionDuration = ability.AnimationDuration;
        currentAnimation = ability.SkillAnimation;

        user.facingLeft = target.GlobalPosition.X < user.GlobalPosition.X;
        user.animationPlayer.Play(user.facingLeft ? AnimTags.TurnLeft : AnimTags.TurnRight);
        Vector2I userPos = TurnManager.Instance.GetTilePosition(user.GlobalPosition);
        Vector2I targetPos = TurnManager.Instance.GetTilePosition(target.GlobalPosition);
    }
}