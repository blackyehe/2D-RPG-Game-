using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Timer = Godot.Timer;

public abstract partial class CombatActor : CharacterBody2D
{
    public string ActorName;
    public bool IsPlayer => this is Player;
    public bool IsAlive = true;
    public bool IsTurnActive = false;
    public bool HasActed = false;
    public bool InCombat = false;
    [Export] public bool facingLeft;

    public CombatAction currentAction;

    [Export] public Sprite2D Sprite;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public PlayerStats actorStats;
    [Export] public Area2D deadArea2D;
    [Export] public CollisionShape2D collisionShape2D;

    public RuntimeStats Stats;

    public System.Collections.Generic.Dictionary<EquipSlot, EquipableItem> EquippedItems = new();
    public List<WeaponBaseAction> runtimeAbilities = new();
    public List<StatusEffectBase> statusEffectList = new();
    public List<BaseSkill> LearnedTalents = new();
    public List<Debuff> debuffList = new();
    public List<Buff> buffList = new();
    public List<PassiveFeature> PassiveFeatures = new();
    public event EventHandler OnActionFinished;
    public event EventHandler OnActorDeath;
    public event EventHandler OnProjectileNeeded;

    public delegate void DamageTakenEvent(CombatActor actor, float damage);

    public event DamageTakenEvent OnDamageTaken;
    public void EmitOnProjectileNeeded() => OnProjectileNeeded?.Invoke(this, EventArgs.Empty);

    public WeaponBaseAction currentAbility;
    public BaseWeapon activeWeapon;
    public BaseWeapon activeOffhandWeapon;

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
        GlobalEvents.Instance.EmitOnActionCountChanged(Stats.remainingCost[actionCostType.Action],
                Stats.remainingCost[actionCostType.BonusAction],
                Stats.remainingCost[actionCostType.Mana]);
    }

    public float GetWeaponDMGBySlotType(EquipSlot slotType)
    {
        return GetItemBySlotType(slotType).weaponResource.GetOverallWeaponDamage();
    }

    public bool IsEntityInRange(CombatActor enemy)
    {
        if (enemy is null) return false;

        var thisPos = TurnManager.Instance.GetTilePosition(GlobalPosition);
        var actorPos = TurnManager.Instance.GetTilePosition(enemy.GlobalPosition);
        if (activeWeapon == null)
            return false;

        return IsPlayer
            ? thisPos.DistanceTo(actorPos) <= currentAbility.ActionRange
            : thisPos.DistanceTo(actorPos) <= currentAbility.ActionRange + Stats.TileMovementCount;
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

    public DamageWithType CheckEnemyResistance(CombatActor user, DamageWithType dmgWithType, CombatActor target)
    {
        if (target.actorStats.Resistances is null 
             || !target.actorStats.Resistances.TryGetValue(dmgWithType.dmgType, out var resistance)) return dmgWithType;
            
        if (resistance is Resistances.HasResistanceTo)
        {
            dmgWithType.dmgNumber /= 2;
            dmgWithType.dmgNumber = (float)Math.Round(dmgWithType.dmgNumber);
        }

        if (target.actorStats.Resistances[dmgWithType.dmgType] is Resistances.WeakTo)
        {
            dmgWithType.dmgNumber *= 2;
            dmgWithType.dmgNumber = (float)Math.Round(dmgWithType.dmgNumber);
        }

        return dmgWithType;
    }

    public List<DamageWithType> CheckUserPassives(CombatActor user, DamageWithType dmgWithType, CombatActor target)
    {
        var returnList = new List<DamageWithType>();
        returnList.Add(dmgWithType);

        if (user.LearnedTalents.Count <= 0) return returnList;

        List<DamageWithType> damageInstanceList = new List<DamageWithType>();

        foreach (var passive in user.LearnedTalents)
        {
            if (passive.PassiveIsLegal(user, dmgWithType, target))
            {
                damageInstanceList.Add(passive.PassiveSkillEffect(user, dmgWithType.dmgType, dmgWithType.dmgNumber,
                    target));
            }
        }

        return damageInstanceList.Count > 0 ? damageInstanceList : returnList;
    }

    public void CheckItemPassiveFeatures(CombatActor user, DamageWithType dmgWithType,
        CombatActor target)
    {
        if (user.PassiveFeatures.Count <= 0) return;

        foreach (var feature in user.PassiveFeatures)
        {
            if (feature.IsFeatureEffectLegal(user, dmgWithType, target))
            {
                feature.PassiveFeatureEffect(user, dmgWithType, target);
            }
        }
    }

    public List<DamageWithType> CheckStatusEffectPassives(CombatActor user, DamageWithType dmgWithType,
        CombatActor target)
    {
        var returnList = new List<DamageWithType>();
        returnList.Add(dmgWithType);

        if (user.statusEffectList.Count <= 0) return returnList;
        if (user.debuffList.Count <= 0 && user.buffList.Count <= 0) return returnList;

        List<DamageWithType> buffDamageInstanceList = new List<DamageWithType>();
        List<DamageWithType> debuffDamageInstanceList = new List<DamageWithType>();


        if (user.debuffList.Count >= 1)
        {
            for (int i = 0; i < user.debuffList.Count; i++)
            {
                if (user.debuffList[i].IsStatusPassiveLegal(user, dmgWithType, target))
                {
                    debuffDamageInstanceList.Add(user.debuffList[i].StatusPassive(user, dmgWithType, target));
                }
            }
        }

        debuffDamageInstanceList.Add(dmgWithType);

        if (user.buffList.Count < 1) return buffDamageInstanceList.Count > 0 ? buffDamageInstanceList : returnList;
        
        for (int i = 0; i < debuffDamageInstanceList.Count; i++)
        {
            var debuffDamageInstance = debuffDamageInstanceList[i];
            for (int j = 0; j < user.buffList.Count; j++)
            {
                if (user.buffList[j].IsStatusPassiveLegal(user, debuffDamageInstance, target))
                {
                    buffDamageInstanceList.Add(user.buffList[j].StatusPassive(user, debuffDamageInstance, target));
                }
            }
        }
        
        return buffDamageInstanceList.Count > 0 ? buffDamageInstanceList : returnList;
    }

    public List<DamageWithType> ComprehensiveDamageCalc(CombatActor user, DamageWithType dmgWithType,
        CombatActor target)
    {
        List<DamageWithType> userPassiveDmg = CheckUserPassives(user, dmgWithType, target);
        GD.Print(string.Join(userPassiveDmg.ToString()), "damage after user passive check");

        List<DamageWithType> userStatusEffectDmg = [];
        List<DamageWithType> enemyResistDmg = [];
        List<DamageWithType> enemyPassiveDmg = [];
        List<DamageWithType> enemyStatusEffectDmg = [];

        for (int i = 0; i < userPassiveDmg.Count; i++)
        {
            for (int j = 0; j < user.PassiveFeatures.Count; j++)
            {
                if (user.PassiveFeatures[j].IsFeatureEffectLegal(user, userPassiveDmg[i], target))
                {
                    user.PassiveFeatures[j].PassiveFeatureEffect(user, userPassiveDmg[i], target);
                    GD.Print(user.Name, " Gained the following buffs after attacking: ",
                        user.PassiveFeatures[j].AppliedStatusEffect.Name);
                }
            }
        }

        for (int i = 0; i < userPassiveDmg.Count; i++)
        {
            var list = CheckStatusEffectPassives(user, userPassiveDmg[i], target);
            userStatusEffectDmg.Add(list[i]);
            GD.Print(userStatusEffectDmg[i].dmgNumber, userStatusEffectDmg[i].dmgType,
                " Dmg after user status effect check");
        }

        for (int i = 0; i < userPassiveDmg.Count; i++)
        {
            for (int j = 0; j < target.PassiveFeatures.Count; j++)
            {
                if ( user.PassiveFeatures.Count > 0 && user.PassiveFeatures[j].IsFeatureEffectLegal(target, userPassiveDmg[i], user))
                {
                    user.PassiveFeatures[j].PassiveFeatureEffect(target, userPassiveDmg[i], user);
                    GD.Print(target.Name, " Gained the following buffs after attacking: ",
                        target.PassiveFeatures[j].AppliedStatusEffect.Name);
                }
            }
        }

        for (int i = 0; i < userStatusEffectDmg.Count; i++)
        {
            var list = CheckEnemyResistance(user, userStatusEffectDmg[i], target);
            enemyResistDmg.Add(list);
            GD.Print(enemyResistDmg[i].dmgNumber, enemyResistDmg[i].dmgType, " Damage after enemy resistance check");
        }

        for (int i = 0; i < enemyResistDmg.Count; i++)
        {
            var list = CheckUserPassives(target, enemyResistDmg[i], user);
            enemyPassiveDmg.Add(list[i]);
            GD.Print(enemyPassiveDmg[i].dmgNumber, enemyPassiveDmg[i].dmgType, " Damage after enemy passive check");
        }

        for (int i = 0; i < enemyPassiveDmg.Count; i++)
        {
            var list = CheckStatusEffectPassives(target, enemyPassiveDmg[i], user);
            enemyStatusEffectDmg.Add(list[i]);
            GD.Print(enemyStatusEffectDmg[i].dmgNumber, enemyStatusEffectDmg[i].dmgType,
                " damage after enemy status effect check");
        }

        return enemyStatusEffectDmg;
    }

    public void DamageToDealAfterCalc(List<DamageWithType> damageValues, CombatActor user, CombatActor target)
    {
        List<DamageWithType> damageToDeal = [];

        List<DamageWithType> overallDmgDealt = [];

        for (int i = 0; i < damageValues.Count; i++)
        {
            damageToDeal = user.ComprehensiveDamageCalc(user, damageValues[i], target);
            overallDmgDealt.Add(damageToDeal[0]);

            //Hülye fasz vagyok, és annak ellenére hogy több dmg source van a lista sose lesz 1-nél nagyobb, tehát már 
            //1-es int-nél jön az error, valszeg a methodot kéne újradolgozni, mert lehet felesleges a listát returnolni.
            // Csak annak a damageToDeal[0] margójára ^
        }

        foreach (var instance in overallDmgDealt)
        {
            if (instance.dmgNumber >= 1)
            {
                target.TakeDamage(instance.dmgNumber);
                GD.Print("Damage dealt: ", instance.dmgNumber);
                GD.Print("Damage Type: ", instance.dmgType);
            }
            else
            {
                target.TakeDamage(0);
                GD.Print("Damage was lower than 1, therefore it is 0");
            }
        }
    }

    public bool CheckForStatusEffects()
    {
        if (statusEffectList.Count == 0) return true;

        StatusEffectBase.RemoveStatus(this);

        for (int i = 0; i < statusEffectList.Count; i++)
        {
            (statusEffectList[i] as IStatusEffect)?.TriggerStatusEffect(this);
        }

        return true;
    }

    public void TakeDamage(float damage)
    {
        Stats.HP -= damage;
        OnDamageTaken?.Invoke(this, damage);
        if (Stats.HP < 1)
        {
            OnActorDeath?.Invoke(this, EventArgs.Empty);
            return;
        }

        animationPlayer?.AnimationSetNext(AnimTags.Hurt, AnimTags.Idle);
        animationPlayer?.Play(AnimTags.Hurt);
       
    }

    public void ResetActionPoints()
    {
        Stats.TileMovementCount = Stats.MaxTileMovementCount;
        Stats.remainingCost[actionCostType.Action] = Stats.MaxActionCount;
        Stats.remainingCost[actionCostType.BonusAction] = Stats.MaxBonusActionCount;
    }

    public void TriggerAnimation()
    {
        animationPlayer?.Play(AnimTags.Idle);
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

    public Godot.Collections.Dictionary<actionCostType, int> remainingCost = new();
    public Array<BaseSkill> StartingSkills;

    public RuntimeStats(PlayerStats actorStats)
    {
        Name = actorStats.CharacterName;
        StarterClass = actorStats.Class;
        CurrentClass = StarterClass;
        MaxHP = actorStats.BaseHP;
        MaxMP = actorStats.BaseMP;
        MaxDefense = actorStats.BaseDefense;
        MaxStrength = actorStats.BaseStrength;
        MaxAttack = MaxStrength / 4;
        MaxAgility = actorStats.BaseAgility;
        MaxCharisma = actorStats.BaseCharisma;
        MaxMagicPower = actorStats.BaseMagicPower;
        MaxLuck = actorStats.BaseLuck;
        MaxSpeed = actorStats.BaseSpeed;

        BaseLevel = actorStats.Level;
        BaseXP = actorStats.XP;
        RewardXP = actorStats.RewardXP;
        XPThresholds = actorStats.XPThresholds;

        MaxActionCount = actorStats.ActionCount;
        MaxBonusActionCount = actorStats.BonusActionCount;
        MaxTileMovementCount = (int)MaxAgility / 2;

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
        XPToNextLevel = actorStats.XPThresholds == null || XPThresholds.Count == 0
            ? 0
            : actorStats.XPThresholds[CurrentLevel - 1];

        remainingCost[actionCostType.Action] = MaxActionCount;
        remainingCost[actionCostType.BonusAction] = MaxBonusActionCount;
        remainingCost[actionCostType.Mana] = (int)MaxMP;

        StartingSkills = actorStats.StartingSkills;

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
        return $"Level: {CurrentLevel} => {CurrentLevel + 1}\nVitality: {MaxHP} + {MaxHP / 2} => {MaxHP + MaxHP / 2}";
    }
}