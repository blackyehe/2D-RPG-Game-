using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class CombatState : PlayerState
{
    public CombatActor enemyActor;
    public CombatSubState currentSubState;
    private int moveCost;

    public override void PhysicsProcess(double delta)
    {
        if (Player.IsTurnActive != true)
        {
            return;
        }

        switch (currentSubState)
        {
            case CombatSubState.WaitForInput:
                break;

            case CombatSubState.PerformAction:
                if (Player.currentAction.DoAction(delta) == true)
                {
                    currentSubState = CombatSubState.ActionResult;
                }

                break;

            case CombatSubState.ActionResult:
                Player.animationPlayer.Play(AnimTags.Idle);

                //wip
                currentSubState = CombatSubState.WaitForInput;
                break;

            case CombatSubState.TurnEnd:

                Player.animationPlayer.Play(AnimTags.Idle);
                Player.IsTurnActive = false;
                Player.EndTurn();

                break;
        }
    }

    public override void EnterState()
    {
        GD.Print(Player.ActiveState);
        GD.Print(Player.IsTurnActive);

        Player.isControllable = true;
        currentSubState = CombatSubState.WaitForInput;
    }

    public override void ExitState()
    {
    }

    public void DoAttackAction()
    {
        if (Player.currentAction.IsLegal())
        {
            currentSubState = CombatSubState.PerformAction;
            GD.Print("performing attack");
        }
    }

    public void DoMovementAction()
    {
        if (Player.currentAction.IsLegal())
        {
            GD.Print("Movement action is legal: ", Player.currentAction.IsLegal());
            GD.Print("Substate is now: ", currentSubState);

            currentSubState = CombatSubState.PerformAction;
        }
    }

    protected override void HandleInput()
    {
        if (currentSubState != CombatSubState.WaitForInput)
            return;

        if (Player.GetViewport().GuiGetHoveredControl() != null)
            return;

        if (!Input.IsActionJustPressed(InputTags.LeftClick))
            return;

        Vector2I startTile = TurnManager.Instance.GetTilePosition(Player.GlobalPosition);
        Vector2I targetTile = TurnManager.Instance.GetClickedTilePosition();
        enemyActor = TurnManager.Instance.GetEnemyTile(targetTile);

        Pathfinding.Instance.SetTileSolid(targetTile, false, enemyActor);
        Vector2[] tilePath = Pathfinding.Instance.FindPath(startTile, targetTile);
        Pathfinding.Instance.SetTileSolid(targetTile, true, enemyActor);

        moveCost = tilePath.Length - 1;

        if (enemyActor == null)
        {
            if (moveCost <= Player.Stats.TileMovementCount)
            {
                Player.currentAction = new MoveAction(tilePath, Player);
                DoMovementAction();
            }
            return;
        }
        
        if (Player.IsEntityInRange(enemyActor))
        {
            Player.currentAction = new AttackAction(Player, enemyActor,Player.currentAbility);
            DoAttackAction();
            return;
        }

        var nextToTile = tilePath.SkipLast((int)Player.currentAbility.ActionRange).ToArray();
        var goodPos = TurnManager.Instance.GetTilePosition(nextToTile[^1]);

        Player.currentAction = new MoveAction(nextToTile, Player);
        DoMovementAction();
    }
}