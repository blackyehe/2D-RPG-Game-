using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using Godot.Collections;
using Timer = Godot.Timer;

public abstract partial class CombatActor : CharacterBody2D
{
    public string ActorName;
    public bool IsPlayer => this is Player;
    public bool IsAlive = true;
    public bool IsTurnActive = false;
    public bool HasActed = false;
    [Export] public bool facingLeft;

    public CombatAction currentAction;
    
    [Export] public Sprite2D Sprite;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public PlayerStats actorStats;
    [Export] public Area2D deadArea2D;
    
    public RuntimeStats Stats;

    public System.Collections.Generic.Dictionary<EquipSlot, EquipableItem> EquippedItems = new();
    public List<WeaponBaseAction> allAbilities = new();
    public List<StatusEffectBase> statusEffectList = new();
    public event EventHandler OnActionFinished;
    public event EventHandler OnActorDeath;
    public WeaponBaseAction currentAbility;
    public BaseWeapon activeWeapon;

    public void EndTurn()
    {
        if (currentAction?.isActionFinished == false) return;
        ResetActionPoints();
        IsTurnActive = false;
        GetTree().CreateTimer(0.85).Timeout += () => OnActionFinished?.Invoke(this, EventArgs.Empty);
    }

    public abstract void EnterCombat();
    public abstract void StartTurn();
    public abstract void ExitCombat();
    
    public void SnapToClosestTile(CombatActor actor)
    {
        actor = this;

        Vector2I currentTile = TurnManager.Instance.GetTilePosition(actor.GlobalPosition);
        Vector2 snappedTile = TurnManager.Instance.GetTileToLocal(currentTile);
        actor.GlobalPosition = snappedTile;
    }

    public int GetWeaponRange()
    {
        if (activeWeapon is null)
        {
            return 0;
        }

        return activeWeapon.weaponResource.attackRange;
    }
    public BaseWeapon GetItemBySlotType(EquipSlot slotType)
    {
        return EquippedItems[slotType] as BaseWeapon;
    }

    public BaseWeapon GetActiveWeaponBySlotType(EquipSlot slotType)
    {
        activeWeapon = GetItemBySlotType(slotType);
        return activeWeapon;
    }

    public void RemoveCostAfterAction()
    {
        foreach (var actionCost in (currentAbility.ActionCosts))
        { 
            Stats.remainingCost[actionCost.Key] -= actionCost.Value;
        }
    }

    public double GetWeaponDMGBySlotType(EquipSlot slotType)
    {
       return GetItemBySlotType(slotType).weaponResource.WeaponDamage;
    }
    public bool IsEntityInRange(CombatActor enemy)
    {
        var thisPos = TurnManager.Instance.GetTilePosition(GlobalPosition);
        var actorPos = TurnManager.Instance.GetTilePosition(enemy.GlobalPosition);
        if (activeWeapon == null)
            return false;

        return thisPos.DistanceTo(actorPos) <= currentAbility.ActionRange;
    }
    public void SetActiveWeapon()
    {
        activeWeapon ??= EquippedItems[EquipSlot.MainHand] as BaseWeapon;
        activeWeapon ??= EquippedItems[EquipSlot.Ranged] as BaseWeapon;
        if (activeWeapon is null)
        {
            GD.Print(" nincs fegyo ocskosom :(  csinalj egy geci oklot");
        }
    }
    public void TakeDamage(double damage)
    {
        Stats.HP -= damage;
        if (Stats.HP < 1)
        {
            OnActorDeath?.Invoke(this, EventArgs.Empty);
            return;
        }
        animationPlayer.AnimationSetNext(AnimTags.Hurt, AnimTags.Idle);
        animationPlayer.Play(AnimTags.Hurt);
    }

    public void ResetActionPoints()
    {
        Stats.TileMovementCount = Stats.MaxTileMovementCount;
        Stats.remainingCost[actionCostType.Action] = Stats.MaxActionCount;
        Stats.remainingCost[actionCostType.BonusAction] = Stats.MaxBonusActionCount;
    }

    public bool CheckForStatusEffects()
    {
        for (int i = 0; i < statusEffectList.Count; i++)
        {
            if (statusEffectList[i].Duration <= 0)
            {
                statusEffectList.Remove(statusEffectList[i]);
                i--;
            }
        }
        
        if (statusEffectList.Count == 0) return true;
        
        for (int i = 0; i < statusEffectList.Count; i++)
        {
            (statusEffectList[i] as IStatusEffect)?.StatusEffect(this);
        }

        return true;
    }

    public void ActorDie()
    {
        switch (this)
        {
            case Player actor:
                actor.SetPlayerState(State.Death);
                break;
            case Enemy actor:
                actor.SetEnemyState(StateEnemy.EnemyDead);
                break;
        }
    }

    public void AfterDeath()
    {
        SetPhysicsProcess(false);
        Pathfinding.Instance.SetTileSolid(GlobalPosition, false, this);
        CollisionLayer = 0;
    }
}

public class RuntimeStats
{
    public string Name;
    public classTypes StarterClass;
    public classTypes CurrentClass;
    public double HP;
    public double Attack;
    public double Defense;
    public double Strength;
    public double Agility;
    public double Charisma;
    public double MagicPower;
    public double Luck;
    public double Speed;

    public double MaxHP;
    public double MaxMP;
    public double MaxAttack;
    public double MaxDefense;
    public double MaxStrength;
    public double MaxAgility;
    public double MaxCharisma;
    public double MaxMagicPower;
    public double MaxLuck;
    public double MaxSpeed;
    public int BaseLevel;
    public double BaseXP;
    
    public int TileMovementCount;
    public int CurrentLevel;
    public double XP;
    public double RewardXP;

    public int MaxActionCount;
    public int MaxBonusActionCount;
    public int MaxTileMovementCount;
    public double XPToNextLevel;
    public Array<double> XPThresholds;
    public double LightAttackAnimDuration;
    public double HeavyAttackAnimDuration;
    public double RangedShotAnimDuration;
    public Godot.Collections.Dictionary<actionCostType, int> remainingCost = new();

    public RuntimeStats(PlayerStats actorStats)
    {
        Name = actorStats.CharacterName;
        StarterClass = actorStats.Class;
        CurrentClass = StarterClass;
        MaxHP = actorStats.BaseHP;
        MaxMP = actorStats.BaseMP;
        MaxDefense = actorStats.BaseDefense;
        MaxStrength = actorStats.BaseStrength;
        MaxAttack = MaxStrength/4;
        MaxAgility = actorStats.BaseAgility;
        MaxCharisma = actorStats.BaseCharisma;
        MaxMagicPower = actorStats.BaseMagicPower;
        MaxLuck = actorStats.BaseLuck;
        MaxSpeed = actorStats.BaseSpeed;
        
        BaseLevel  = actorStats.Level;
        BaseXP  = actorStats.XP;
        RewardXP = actorStats.RewardXP;
        XPThresholds = actorStats.XPThresholds;

        MaxActionCount = actorStats.ActionCount;
        MaxBonusActionCount = actorStats.BonusActionCount;
        MaxTileMovementCount = (int)MaxAgility / 2;

        LightAttackAnimDuration = actorStats.LightAttackAnimDuration;
        HeavyAttackAnimDuration = actorStats.HeavyAttackAnimDuration;
        RangedShotAnimDuration = actorStats.RangedShotAnimDuration;

        HP = MaxHP;
        Attack = MaxAttack;
        Defense = MaxDefense;
        Strength = MaxStrength;
        Agility = MaxAgility;
        Charisma = MaxCharisma;
        MagicPower = MaxMagicPower;
        Luck = MaxLuck;
        Speed = MaxSpeed;
        
        CurrentLevel = BaseLevel;
        XP = BaseXP;
        XPToNextLevel = actorStats.XPThresholds == null || XPThresholds.Count == 0 ? 0 : actorStats.XPThresholds[CurrentLevel - 1]; 
        
        remainingCost[actionCostType.Action] = MaxActionCount;
        remainingCost[actionCostType.BonusAction] = MaxBonusActionCount;
        remainingCost[actionCostType.Mana] = (int)MaxMP;
        
        TileMovementCount = MaxTileMovementCount;
    }
    public void OnLevelUp()
    {
        XP -= XPToNextLevel;
        CurrentLevel += 1;
        XPToNextLevel = XPThresholds[CurrentLevel - 1];
        MaxHP += MaxHP / 2;
    }
    public string GetLvlUpStats()
    {
        return $"Level: {CurrentLevel} => {CurrentLevel + 1}\nVitality: {MaxHP} + {MaxHP/2} => {MaxHP + MaxHP/2}";
    }
}