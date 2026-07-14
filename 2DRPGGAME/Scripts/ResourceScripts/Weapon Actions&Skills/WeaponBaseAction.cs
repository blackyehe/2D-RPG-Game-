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
    public float SkillDamage;
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
        Projectile.Target = target;
        Projectile.User = user;
        ProjectileInstance.Instance.AddChild(Projectile);
        Projectile.Scale = new Vector2(0.6f, 0.6f);
        Projectile.LookAt(target.GlobalPosition);
        return Projectile;
    }

    public float GetOverallSkillDamageForDisplay()
    {
        SkillDamage = 0;
        var values = DamageDistribution.Values.ToArray();
        for (int i = 0; i < DamageDistribution.Values.Count; i++)
        {
            SkillDamage += values[i];
        }

        return SkillDamage;
    }

    public Godot.Collections.Dictionary<DamageTypes, int> WeaponAndSkillDamageDistributionForDisplay(CombatActor user,
        EquipSlot desiredHand)
    {
        var list1 = SkillDamageByDistributionForDmgCalc();
        var list2 = user.GetActiveWeaponBySlotType(desiredHand).weaponResource.DamageByDistribution();
        var finalList = new List<DamageWithType>();
        DamageWithType number;
        var finalDict = new Godot.Collections.Dictionary<DamageTypes, int>();

        finalList.AddRange(list1);
        finalList.AddRange(list2);

        for (int i = 0; i < finalList.Count; i++)
        {
            number.dmgNumber = finalList[i].dmgNumber;
            number.dmgType = finalList[i].dmgType;

            if (!finalDict.ContainsKey(finalList[i].dmgType))
            {
                finalDict.Add(finalList[i].dmgType, (int)finalList[i].dmgNumber);
            }
            else
            {
                number.dmgNumber = number.dmgNumber += finalDict[finalList[i].dmgType];
                finalDict[finalList[i].dmgType] = (int)number.dmgNumber;
            }
        }

        return finalDict;
    }

    public List<DamageWithType> WeaponAndSkillDistributionForDmgCalc(CombatActor user, EquipSlot desiredHand)
    {
        var returnList = new List<DamageWithType>();
        var numbersDict = WeaponAndSkillDamageDistributionForDisplay(user, desiredHand);
        DamageWithType damageInstance;
        for (int i = 0; i < numbersDict.Count; i++)
        {
            var keys = numbersDict.Keys.ToList();
            var values = numbersDict.Values.ToList();
            damageInstance.dmgType = keys[i];
            damageInstance.dmgNumber = values[i];
            returnList.Add(damageInstance);
        }

        return returnList;
    }

    public List<DamageWithType> SkillDamageByDistributionForDmgCalc()
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

    public Godot.Collections.Dictionary<DamageTypes, int> SkillDmgDistributionForDisplay()
    {
        var newDict = new Godot.Collections.Dictionary<DamageTypes, int>();
        var dmgList = SkillDamageByDistributionForDmgCalc();
        for (int i = 0; i < dmgList.Count; i++)
        {
            newDict.Add(dmgList[i].dmgType, (int)dmgList[i].dmgNumber);
        }

        return newDict;
    }

    public Godot.Collections.Dictionary<DescriptionPanel, BaseDescription> GetDescription(CombatActor user)
    {
        var description = new Godot.Collections.Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(Name);
        description[DescriptionPanel.MainSprite] = new(Sprite);

        switch (IsWeaponAction)
        {
            case true when IsMelee:
                description[DescriptionPanel.TypeAndRarity] = new("Weapon Action");
                description[DescriptionPanel.Damage] =
                    new(
                        $"{GetOverallSkillDamageForDisplay() + user.GetWeaponDMGBySlotType(EquipSlot.MainHand)} Damage");
                description[DescriptionPanel.DamageDistribution] =
                    new(WeaponAndSkillDamageDistributionForDisplay(user, EquipSlot.MainHand));
                break;
            
            case true when !IsMelee:
                description[DescriptionPanel.TypeAndRarity] = new("Weapon Action");
                description[DescriptionPanel.Damage] =
                    new($"{GetOverallSkillDamageForDisplay() + user.GetWeaponDMGBySlotType(EquipSlot.Ranged)} Damage");
                description[DescriptionPanel.DamageDistribution] =
                    new(WeaponAndSkillDamageDistributionForDisplay(user, EquipSlot.Ranged));
                break;

            case false: // case: !IsWeaponAction -> sima skill ami nem függ fegyvertől.
                description[DescriptionPanel.TypeAndRarity] =
                    new($"Level {SkillLevel} {SkillDamageType} {ActionType}");
                description[DescriptionPanel.Damage] = new($"{GetOverallSkillDamageForDisplay()} Damage");
                description[DescriptionPanel.DamageDistribution] = new(SkillDmgDistributionForDisplay());
                break;
        }

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
            description[DescriptionPanel.BonusActionText] = new("Bonus Action");
        }

        if (ActionCosts.TryGetValue(actionCostType.Mana, out int manaValue) && manaValue > 0)
        {
            description[DescriptionPanel.ManaText] = new($"{ActionCosts[actionCostType.Mana]} Mana");
        }

        return description;
    }
}