using Godot;
using System.Linq;

public class CombatState : PlayerState
{
    public CombatActor enemyActor;
    public CombatSubState currentSubState;
    private int moveCost;
    private bool isStatusAlreadyChecked;

    public override void PhysicsProcess(double delta)
    {
        if (Player.IsTurnActive != true)
        {
            return;
        }

        switch (currentSubState)
        {
            case CombatSubState.CheckStatusEffect:
                if (!isStatusAlreadyChecked)
                {
                    Player.CheckForStatusEffects();

                    isStatusAlreadyChecked = true;
                }

                if (!Player.animationPlayer.IsPlaying() || Player.animationPlayer.CurrentAnimation != AnimTags.Hurt)
                {
                    currentSubState = CombatSubState.WaitForInput;
                    Player.isControllable = true;
                }
                else
                {
                    currentSubState = CombatSubState.WaitForInput;
                }
                
                break;

            case CombatSubState.WaitForInput:
                break;

            case CombatSubState.PerformAction:
                if (Player.currentAction.DoAction(delta))
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
                isStatusAlreadyChecked = false;
                Player.EndTurn();

                break;
        }
    }

    public override void EnterState()
    {
        GD.Print(Player.ActiveState);
        GD.Print(Player.IsTurnActive);
        
        currentSubState = CombatSubState.CheckStatusEffect;
    }

    public override void ExitState()
    {
    }

    public void PerformAction()
    {
        if (Player.currentAction.IsLegal())
        {
            currentSubState = CombatSubState.PerformAction;
            GD.Print("performing attack");
        }
    }

    protected override void HandleInput()
    {
        if (currentSubState != CombatSubState.WaitForInput)
            return;

        if (Player.GetViewport().GuiGetHoveredControl() != null)
            return;

        if (Input.IsActionJustPressed(InputTags.RightClick))
        {
            Player.currentAbility = null;
            return;
        }
        
        if (!Input.IsActionJustPressed(InputTags.LeftClick))
            return;

        Vector2I startTile = TurnManager.Instance.GetTilePosition(Player.GlobalPosition);
        Vector2I targetTile = TurnManager.Instance.GetClickedTilePosition();
        
        var cursorTarget = Player.cursorTarget;
        var cursorTargetPos = TurnManager.Instance.GetTilePosition(cursorTarget.GlobalPosition);
        cursorTargetPos = TurnManager.Instance.GetClickedTilePosition();
        //Valahogy belekell integrálni a meglévő kódba, az aoe spell meg distance check a pos-tól. gl soldier o7
        
        enemyActor = TurnManager.Instance.GetEnemyTile(targetTile);

        Pathfinding.Instance.SetTileSolid(targetTile, false, enemyActor);
        Vector2[] tilePath = Pathfinding.Instance.FindPath(startTile, targetTile);
        Pathfinding.Instance.SetTileSolid(targetTile, true, enemyActor);

        moveCost = tilePath.Length - 1;

        if (enemyActor == null && Player.currentAbility == null)
        {
            if (moveCost <= Player.Stats.TileMovementCount)
            {
                Player.currentAction = new MoveAction(tilePath, Player);
                PerformAction();
            }

            return;
        }

        if (enemyActor == null && Player.currentAbility != null)
        {
           
            cursorTarget.GlobalPosition = TurnManager.Instance.GetTileToLocal(cursorTargetPos);
            if (startTile.DistanceTo(cursorTargetPos) <= Player.currentAbility.ActionRange)
            {
                Player.currentAction = new AttackAction(Player, cursorTarget, Player.currentAbility);
                PerformAction();
                return;
            } 
        }
        
        if (Player.IsEntityInRange(enemyActor))
        {
            Player.currentAction = new AttackAction(Player, enemyActor, Player.currentAbility);
            PerformAction();
            return;
        }

        var nextToTile = tilePath.SkipLast((int)Player.currentAbility.ActionRange).ToArray();
        var goodPos = TurnManager.Instance.GetTilePosition(nextToTile[^1]);

        Player.currentAction = new MoveAction(nextToTile, Player);
        PerformAction();
    }
}