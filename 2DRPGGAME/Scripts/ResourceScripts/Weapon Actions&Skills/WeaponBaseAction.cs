using Godot;
using System;
using Godot.Collections;
using Godot.NativeInterop;

public abstract partial class WeaponBaseAction : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public bool IsWeaponAction;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public double SkillDamage;
    [Export] public int SkillLevel;
    [Export] public DamageTypes SkillDamageType;
    [Export] public ActionTypes ActionType;
    [Export] public AttackProperty Action;
    [Export] public AttackProperty Range;
    [Export] public AttackProperty ActionArea;
    [Export] public Dictionary<DamageTypes, int> DamageDistribution;
    [Export] public Dictionary<actionCostType, int> ActionCosts;
    [Export] public StatusEffectBase StatusEffect;
    [Export] public double ActionRange;
    [Export] public double CoolDown;
    public StringName SkillAnimation;
    public double AnimationDuration;
    public abstract void DoAction(CombatActor user, CombatActor target);
    public abstract double GetAnimDuration(CombatActor user);

    public Dictionary<DescriptionPanel, BaseDescription> GetDescription()
    {
        var description = new Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(Name);
        description[DescriptionPanel.MainSprite] = new(Sprite);
        if (IsWeaponAction)
        {
            description[DescriptionPanel.TypeAndRarity] = new($"Weapon Action");
        }
        else
        {
            description[DescriptionPanel.TypeAndRarity] = new($"Level {SkillLevel} {SkillDamageType} {ActionType}");
        }

        description[DescriptionPanel.Damage] = new($"{SkillDamage} Damage");
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