using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class TurnManager : Node
{
    public CombatActor currentActor;
    public bool isCombatActive = false;
    public int roundNumber;
    public int turnNumber;
    public int currentTurnIndex = 0;
    [Export] public TileMap tileMap;

    public List<CombatActor> turnQueue = new();
    public List<CombatActor> allActors = new();
    public List<CombatActor> allyCombatants = new();
    public List<CombatActor> enemyCombatants = new();
    public List<CombatActor> deadActors = new();

    [Export] public AnimationPlayer TransitionAnimation;

    public event EventHandler OnCombatStarted;
    public event EventHandler OnCombatEnded;

    public static TurnManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public Vector2I GetTilePosition(Vector2 pos)
    {
        return tileMap.LocalToMap(pos);
    }

    public Vector2 GetTileToLocal(Vector2I pos)
    {
        return tileMap.MapToLocal(pos);
    }

    public Vector2 GetMousePosition()
    {
        return tileMap.GetGlobalMousePosition();
    }

    public Vector2I GetClickedTilePosition()
    {
        return GetTilePosition(GetMousePosition());
    }

    public CombatActor GetEnemyTile(Vector2I tile)
    {
        foreach (CombatActor entity in enemyCombatants)
        {
            if (GetTilePosition(entity.GlobalPosition) == tile)
            {
                return entity;
            }
        }

        return null;
    }

    public List<Vector2I> GetAllEnemiesPos()
    {
        var enemyList = new List<Vector2I>();
        foreach (CombatActor entity in enemyCombatants)
        {
            enemyList.Add(GetTilePosition(entity.GlobalPosition));
        }

        return enemyList;
    }

    public CombatActor GetAllyTile(Vector2I tile)
    {
        foreach (CombatActor ally in allyCombatants)
        {
            if (GetTilePosition(ally.GlobalPosition) == tile)
            {
                return ally;
            }
        }

        return null;
    }

    public void RegisterCombatant(CombatActor actor)
    {
        allActors.Add(actor);
        if (actor.IsPlayer)
        {
            allyCombatants.Add(actor);
        }
        else
        {
            enemyCombatants.Add(actor);
        }

        GD.Print("Combatants sorted");
    }

    public void CombatTransition()
    {
        PartyManager.Instance.PlayerParty.Find(x => x.isControllable).SetPlayerState(State.Idle);

        foreach (var player in PartyManager.Instance.PlayerParty)
        {
            player.Velocity = Vector2.Zero;
        }

        TransitionAnimation.Play("FadeIn");
    }

    public void StartCombat()
    {
        
        Pathfinding.Instance.SetupWalkableTiles();
        isCombatActive = true;
        OnCombatStarted?.Invoke(this, EventArgs.Empty);
        foreach (var actor in allActors)
        {
            actor.EnterCombat();
            actor.InCombat = true;
            actor.OnActorDeath += Actors_OnActorDeath;
        }

        NextCurrentActor();
    }

    public void EndCombat()
    {
        GlobalEvents.Instance.EmitGetExperience(GiveOutCombatXP());
        isCombatActive = false;
        OnCombatEnded?.Invoke(this, EventArgs.Empty);
        foreach (var actor in allActors)
        {
            actor.ExitCombat();
            actor.InCombat = false;
        }

        ResetCombatants();
    }

    public double GiveOutCombatXP()
    {
        double experience = 0;
        foreach (var actor in deadActors)
        {
            experience += actor.Stats.RewardXP;
        }

        return experience;
    }

    public void ResetCombatants()
    {
        turnQueue.Clear();
        allyCombatants.Clear();
        enemyCombatants.Clear();
        deadActors.Clear();
        allActors.Clear();
    }

    private void Actors_OnActorDeath(object sender, EventArgs e)
    {
        CombatActor actor = (CombatActor)sender;

        turnQueue.Remove(actor);
        actor.IsAlive = false;
        allActors.Remove(actor);
        enemyCombatants.Remove(actor);
        deadActors.Add(actor);
        actor.ActorDie();

        if (enemyCombatants.Count == 0)
        {
            EndCombat();
        }
    }

    public void NextCurrentActor()
    {
        if (turnQueue.Count == 0)
        {
            BuildTurnQueue();
        }
        
        currentActor = turnQueue[0];

        if (currentActor.IsPlayer)
        {
            PartyManager.Instance.MainPlayer = currentActor as Player;
        }
        
        turnQueue.RemoveAt(0);

        currentActor.StartTurn();

        currentActor.OnActionFinished += CurrentActor_OnActionFinished;
    }

    public void BuildTurnQueue()
    {
        foreach (var actor in allActors.OrderByDescending(x => x.Stats.Speed))
        {
            turnQueue.Add(actor);
        }
    }

    private void CurrentActor_OnActionFinished(object sender, EventArgs e)
    {
        currentActor.OnActionFinished -= CurrentActor_OnActionFinished;
        if (currentActor.IsPlayer)
        {
            if (currentActor is Player player) player.isControllable = false;
        }
        Pathfinding.Instance.SetupWalkableTiles();
        NextCurrentActor();
    }
}