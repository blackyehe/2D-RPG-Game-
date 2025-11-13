using Godot;
using System;
using System.Linq;

public class FollowPartyState : PlayerState
{
    private Player PlayerToFollow;
    private int SwitchNumber = 0;
    private const float reachThreshold = 2f;
    private int _index = 0;
    private int timerInt = 7;
    private Vector2[] movementPath;

    public override void PhysicsProcess(double delta)
    {
        switch (SwitchNumber)
        {
            case 0:
                if (4 > CalcPath().Length)
                {
                    Player.animationPlayer.Play(AnimTags.Idle);
                }
                else
                {
                    SwitchNumber++;
                }

                break;
            case 1:
            {
                if (timerInt != 7) timerInt++;
                if (timerInt == 7)
                {
                    timerInt = 0;
                    movementPath = CalcPath();
                }

                if (DoMovement(delta, Player, movementPath))
                {
                    SwitchNumber = 0;
                }
            }
                break;
        }
    }

    private Vector2[] CalcPath()
    {
        var initialPathArray = Pathfinding.Instance.FindPath(Player.GlobalPosition, PlayerToFollow.GlobalPosition);
        var pathArray = initialPathArray.SkipLast(1).ToArray();
        pathArray = pathArray.Skip(1).ToArray();
        if (_index > 0) _index = 0;
        return pathArray;
    }

    private bool DoMovement(double delta, CombatActor actor, Vector2[] findPath)
    {
        if (_index >= findPath.Length)
        {
            return true;
        }
        
        var timer = timerInt;
        if (actor.Position.DistanceTo(findPath[_index]) < reachThreshold)
        {
            _index++;

            if (_index >= findPath.Length)
            {
                actor.SnapToClosestTile(actor);
                _index = 0;

                return false;
            }

            Vector2 direction
                = _index + 1 < findPath.Length
                    ? findPath[_index + 1] - findPath[_index]
                    : findPath[_index] - findPath[_index - 1];

            bool newFacingLeft = direction.X < 0;

            if (newFacingLeft != actor.facingLeft)
            {
                GD.Print("facing left bozo?: ", actor.facingLeft);
                actor.facingLeft = newFacingLeft;
                actor.animationPlayer.Play(actor.facingLeft ? AnimTags.TurnLeft : AnimTags.TurnRight);
                actor.animationPlayer.Seek(0, true);
                return false;
            }
        }

        actor.animationPlayer.Play(AnimTags.Walk);

        float speed = 90f;

        actor.Position += (findPath[_index] - actor.Position).Normalized() * speed * (float)delta;

        return false;
    }

    public override void EnterState()
    {
        Player.animationPlayer.Play(AnimTags.Idle);
        PlayerToFollow = PartyManager.Instance.PlayerParty.Find(x => x.FollowIndex == Player.FollowIndex - 1);
        Player.collisionShape2D.Disabled = true;
    }

    public override void ExitState()
    {
        Player.collisionShape2D.Disabled = false;
    }
    protected override void HandleInput()
    {
    }
}