using Godot;

public partial class EnemyCombatState : EnemyState
{
    public CombatAction currentAction;
    public CombatSubState currentSubState;
    private int moveCost;
    private double actionTimer;
    private double actionDuration;
    private int actionCounter = 0;

    public override void PhysicsProcess(double delta)
    {
        if (Enemy.IsTurnActive != true)
        {
            return;
        }

        switch (currentSubState)
        {
            case CombatSubState.DecideAction:
                Enemy.behaviour.DecideAction();
                currentSubState = CombatSubState.PerformAction;
                break;

            case CombatSubState.PerformAction:
                if (actionCounter < Enemy.combatActions.Count)
                {
                    if (Enemy.combatActions[actionCounter].DoAction(delta))
                    {
                        actionCounter++;
                    }
                    return;
                }

                actionCounter = 0;
                Enemy.combatActions.Clear();
                currentSubState = CombatSubState.ActionResult;
                break;

            case CombatSubState.ActionResult:

                Enemy.animationPlayer.Play(AnimTags.Idle);

                //wip
                currentSubState = CombatSubState.TurnEnd;
                break;

            case CombatSubState.TurnEnd:

                Enemy.animationPlayer.Play(AnimTags.Idle);
                Enemy.IsTurnActive = false;
                Enemy.EndTurn();
                break;
        }
    }

    public override void EnterState()
    {
        currentSubState = CombatSubState.DecideAction;
    }

    public override void ExitState()
    {
    }
}