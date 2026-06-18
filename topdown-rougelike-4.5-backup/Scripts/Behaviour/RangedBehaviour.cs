using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class RangedBehaviour : Behaviour
{
    private bool MoveChecked;
    private bool AttackChecked;
    private bool eventSubscribed;

    public override void DecideAction()
    {
        if (!eventSubscribed)
        {
            ownerEntity.OnActionFinished += checkRefresh;
            eventSubscribed = true;
        }
        
        void checkRefresh(object sender, EventArgs eventArgs)
        {
            MoveChecked = false;
            AttackChecked = false;
            eventSubscribed  = false;
            ownerEntity.OnActionFinished -= checkRefresh;
        }
        
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
        var activeRange = ownerEntity.currentAbility?.ActionRange;
        var allyActor =
            TurnManager.Instance.GetAllyTile(TurnManager.Instance.GetTilePosition(closestEntity.GlobalPosition));

        Pathfinding.Instance.SetTileSolid(closestEntity.GlobalPosition, false, allyActor);

        var pathArray = Pathfinding.Instance.FindPath(ownerEntity.GlobalPosition, closestEntity.GlobalPosition);
        pathArray = pathArray.SkipLast((int)activeRange).ToArray();

        Pathfinding.Instance.SetTileSolid(closestEntity.GlobalPosition, true, allyActor);


        if (ownerEntity.Stats.TileMovementCount + 1 >= pathArray.Length)
        {
            ownerEntity.MovementPath = pathArray;

            var closestEntityPos = TurnManager.Instance.GetTilePosition(closestEntity.GlobalPosition);

            var moveC = new MoveAction(pathArray, ownerEntity);
            moveC.findPath = ownerEntity.MovementPath;

            var attC = new AttackAction(ownerEntity, closestEntity, ownerEntity.currentAbility);
            
            if (moveC.IsLegal() && !MoveChecked)
            {
                ownerEntity.AddAction(moveC);
                MoveChecked = true;
                return;
            }

            if (attC.IsLegal() && !AttackChecked)
            {
                ownerEntity.AddAction(attC);
                AttackChecked = true;
                return;
            }
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