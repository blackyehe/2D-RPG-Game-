using Godot;

public partial class EnemyCombatState : EnemyState
{
    public CombatAction currentAction;
    public CombatSubState currentSubState;
    private int moveCost;
    private double actionTimer;
    private double actionDuration;
    private int actionCounter = 0;
    private bool isStatusAlreadyChecked = false;
    private bool isActionAlreadyChecked = false;

    public override void PhysicsProcess(double delta)
    {
        if (Enemy.IsTurnActive != true)
        {
            return;
        }

        switch (currentSubState)
        {
            case CombatSubState.CheckStatusEffect:
                if (!isStatusAlreadyChecked)
                {
                    Enemy.CheckForStatusEffects();

                    isStatusAlreadyChecked = true;
                }

                if (!Enemy.animationPlayer.IsPlaying() || Enemy.animationPlayer.CurrentAnimation != AnimTags.Hurt)
                {
                    currentSubState = CombatSubState.DecideAction;
                }

                break;

            case CombatSubState.DecideAction:
                if (!isActionAlreadyChecked)
                {
                    Enemy.behaviour.DecideAction();
                    isActionAlreadyChecked = true;
                }

                if (Enemy.combatActions.Count != 0)
                {
                    currentAction = Enemy.combatActions.Dequeue();
                }

                currentSubState = CombatSubState.PerformAction;
                break;

            case CombatSubState.PerformAction:
                if (currentAction == null)
                {
                    currentSubState = CombatSubState.ActionResult;
                    break;
                }

                if (currentAction.DoAction(delta))
                {
                    currentSubState = Enemy.combatActions.Count == 0
                        ? CombatSubState.ActionResult
                        : CombatSubState.DecideAction;
                }

                break;

            case CombatSubState.ActionResult:

                Enemy.animationPlayer.Play(AnimTags.Idle);

                //wip
                currentSubState = CombatSubState.TurnEnd;
                break;

            case CombatSubState.TurnEnd:

                Enemy.animationPlayer.Play(AnimTags.Idle);
                Enemy.IsTurnActive = false;
                isStatusAlreadyChecked = false;
                isActionAlreadyChecked = false;
                currentAction = null;
                Enemy.EndTurn();
                break;
        }
    }

    public override void EnterState()
    {
        currentSubState = CombatSubState.CheckStatusEffect;
    }


    public override void ExitState()
    {
    }
}