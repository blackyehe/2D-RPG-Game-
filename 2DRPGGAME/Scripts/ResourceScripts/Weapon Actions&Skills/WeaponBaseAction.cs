using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.NativeInterop;

public abstract partial class WeaponBaseAction : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public bool HasProjectileOrSecondaryAnimation;
    [Export] public bool IsWeaponAction;
    [Export] public bool IsMelee;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public float SkillDamage;
    [Export] public Godot.Collections.Dictionary<DamageTypes, int> DamageDistribution;
    [Export] public int SkillLevel;
    [Export] public DamageTypes SkillDamageType;
    [Export] public ActionTypes ActionType;
    [Export] public AttackProperty Action;
    [Export] public AttackProperty Range;
    [Export] public AttackProperty ActionArea;
    [Export] public Godot.Collections.Dictionary<actionCostType, int> ActionCosts;
    [Export] public StatusEffectBase StatusEffect;
    [Export] public double ActionRange;
    [Export] public double CoolDown;
    [Export] public PackedScene ProjectileScene;
    public StringName SkillAnimation;
    public double AnimationDuration;
    public abstract void DoAction(CombatActor user, CombatActor target);
    public AbilityProjectile SpawnProjectile(CombatActor user, CombatActor target)
    {
        if (ProjectileScene == null) return null;
        
        var projectileScene = ProjectileScene.Instantiate<AbilityProjectile>();
        AbilityProjectile Projectile = projectileScene;
        Projectile.Visible = true;
        Projectile.SpawnPos = user.GlobalPosition;
        Projectile.Target  = target;
        Projectile.User = user;
        ProjectileInstance.Instance.AddChild(Projectile);
        Projectile.Scale = new Vector2(0.6f, 0.6f);
        Projectile.LookAt(target.GlobalPosition);
        return Projectile;
    }
    public float GetOverallSkillDamage()
    {
        SkillDamage = 0;
        var values = DamageDistribution.Values.ToArray();
        for (int i = 0; i < DamageDistribution.Values.Count; i++)
        {
            SkillDamage += values[i];
        }
        return SkillDamage;
    }
    public List<DamageWithType> SkillDamageByDistribution()
    {
        List<DamageWithType> damageValues = [];
        for (int i = 0; i < DamageDistribution.Values.Count; i++)
        {
            DamageWithType currentValue = new()
            {
                dmgType = DamageDistribution.ElementAt(i).Key,
                dmgNumber = DamageDistribution.ElementAt(i).Value
            };
            damageValues.Add(currentValue);
        }
        return damageValues;
    }

    public Godot.Collections.Dictionary<DescriptionPanel, BaseDescription> GetDescription(CombatActor user)
    {
        var description = new Godot.Collections.Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(Name);
        description[DescriptionPanel.MainSprite] = new(Sprite);
        if (IsWeaponAction && IsMelee)
        {
            description[DescriptionPanel.TypeAndRarity] = new($"Weapon Action");
            description[DescriptionPanel.Damage] =
                new($"{GetOverallSkillDamage() + user.GetWeaponDMGBySlotType(EquipSlot.MainHand)} Damage");
        }
        else if (IsWeaponAction && !IsMelee)
        {
            description[DescriptionPanel.TypeAndRarity] = new($"Weapon Action");
            description[DescriptionPanel.Damage] =
                new($"{GetOverallSkillDamage() + user.GetWeaponDMGBySlotType(EquipSlot.Ranged)} Damage");
        }
        else if(!IsWeaponAction)
        {
            description[DescriptionPanel.TypeAndRarity] =
                new($"Level {SkillLevel} {SkillDamageType} {ActionType}");
            description[DescriptionPanel.Damage] = new($"{GetOverallSkillDamage()} Damage");
        }
        description[DescriptionPanel.DamageDistribution] = new(DamageDistribution);
        description[DescriptionPanel.SkillDescription] = new(Description);
        if (StatusEffect != null)
        {
            description[DescriptionPanel.DebuffSprite] = new(StatusEffect.Sprite);
            description[DescriptionPanel.DebuffDurationText] =
                new($"{StatusEffect.Name}: {StatusEffect.Duration} Turns \n{StatusEffect.Description}");
        }
        description[DescriptionPanel.ActionTypeSprite] = new(Action.Sprite);
        description[DescriptionPanel.ActionTypeText] = new(Action.SpellText);
        description[DescriptionPanel.ActionRangeSprite] = new(Range.Sprite);
        description[DescriptionPanel.ActionRangeText] = new(Range.SpellText);
        description[DescriptionPanel.STorAOESprite] = new(ActionArea.Sprite);
        description[DescriptionPanel.STorAOEText] = new(ActionArea.SpellText);
        if (ActionCosts.TryGetValue(actionCostType.Action, out int value) && value > 0)
        {
            description[DescriptionPanel.ActionText] = new($"Action");
        }

        if (ActionCosts.TryGetValue(actionCostType.BonusAction, out int bonusValue) && bonusValue > 0)
        {
            description[DescriptionPanel.BonusActionText] = new($"Bonus Action");
        }

        if (ActionCosts.TryGetValue(actionCostType.Mana, out int manaValue) && manaValue > 0)
        {
            description[DescriptionPanel.ManaText] = new($"{ActionCosts[actionCostType.Mana]} Mana");
        }

        return description;
    }
}