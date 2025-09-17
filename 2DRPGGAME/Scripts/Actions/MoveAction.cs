using System.Linq;
using Godot;

public partial class MoveAction : CombatAction
{
    private Vector2 _targetTilePos;
    public Vector2[] findPath;
    private int _index = 0;
    private bool previousFacing;
    private Vector2I enemyPos;
    private const float reachThreshold = 1.5f;
    
    public override bool IsLegal()
    {
        if (findPath != null && findPath.Length > 1 && actor.Stats.TileMovementCount > 0)
        {
            return true;
        }

        return false;
    }

    public override bool DoAction(double delta)
    {
        if (_index >= findPath.Length)
        {
            return true;
        }
        
        if (actor.Position.DistanceTo(findPath[_index]) < reachThreshold)
        {
            
            _index++;
            
            if (_index >= findPath.Length)
            {
                var moveCost = findPath.Length - 1;
                actor.Stats.TileMovementCount -= moveCost;
                actor.SnapToClosestTile(actor);
                isActionFinished = true;
                return false;
            }

            Vector2 direction = _index + 1 < findPath.Length
                ? findPath[_index + 1] - findPath[_index]
                : findPath[_index] - findPath[_index - 1];

            bool newFacingLeft = direction.X < 0;

            if (newFacingLeft != actor.facingLeft)
            {
                GD.Print("facing left bozo?: ", actor.facingLeft);
                actor.facingLeft = newFacingLeft;
                actor.animationPlayer.Play(actor.facingLeft ? AnimTags.TurnLeft : AnimTags.TurnRight);
                actor.animationPlayer.Seek(0, true);
                isActionFinished = false;
                return false;
            }
        }

        actor.animationPlayer.Play(AnimTags.Walk);

        float speed = 70f;
        actor.Position += (findPath[_index] - actor.Position).Normalized() * speed * (float)delta;

        return false;
    }

    public MoveAction(Vector2[] pathArray, CombatActor actor)
    {
        this.actor = actor;

        var actorPos = TurnManager.Instance.GetTilePosition(actor.GlobalPosition);

        findPath = pathArray;

        GD.Print("findPath array: ", findPath[0]);
    }
}