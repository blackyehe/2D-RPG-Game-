using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class MeleeBehaviour : Behaviour
{
    public Queue<CombatAction> ActionQueue = new ();
    
    public override void DecideAction() 
    {
        var targets = TurnManager.Instance.allyCombatants;

        CombatActor closestEntity = null;

        float distanceToTarget = 999;

        ownerEntity.currentAbility = ownerEntity.activeWeapon.weaponResource.Actions.FirstOrDefault();
        
        for (int i = 0; i < targets.Count; i++)
        {
            var currentDistance = ownerEntity.GetDistanceToTile(targets[i]);
            if (currentDistance < distanceToTarget)
            {
                distanceToTarget = currentDistance;
                closestEntity = targets[i];
            }
        }
        if (closestEntity == null) return;
        var allyActor =
            TurnManager.Instance.GetAllyTile(TurnManager.Instance.GetTilePosition(closestEntity.GlobalPosition));

        Pathfinding.Instance.SetTileSolid(closestEntity.GlobalPosition,false,allyActor);
        
        var pathArray = Pathfinding.Instance.FindPath(ownerEntity.GlobalPosition, closestEntity.GlobalPosition);

        Pathfinding.Instance.SetTileSolid(closestEntity.GlobalPosition,true,allyActor);
        
        if (ownerEntity.Stats.TileMovementCount + 1 >= pathArray.Length)
        {
            ownerEntity.MovementPath = pathArray.SkipLast(1).ToArray();

            var closestEntityPos = TurnManager.Instance.GetTilePosition(closestEntity.GlobalPosition);

            var moveC = new MoveAction(pathArray, ownerEntity);
            moveC.findPath = ownerEntity.MovementPath;
            
            var attC = new AttackAction(ownerEntity,closestEntity,ownerEntity.currentAbility);
              
            if (moveC.IsLegal())
            {
                ownerEntity.AddAction(moveC);
                return;
            }
            
            if (!attC.IsLegal())
                return;

            ownerEntity.AddAction(attC);
        }
        else
        {
            ownerEntity.MovementPath = pathArray.Take(ownerEntity.Stats.TileMovementCount).ToArray();

            var closestEntityPos = TurnManager.Instance.GetTilePosition(closestEntity.GlobalPosition);

            var moveC = new MoveAction(ownerEntity.MovementPath, ownerEntity);
            //moveC.findPath = ownerEntity.MovementPath;

            if (!moveC.IsLegal())
                return;

            ownerEntity.AddAction(moveC);
        }
    }
}
