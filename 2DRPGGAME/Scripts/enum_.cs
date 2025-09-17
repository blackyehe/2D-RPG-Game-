public enum ItemType
{
    Weapon,
    Armor,
    Enchantment,
    Consumable,
    Quest,
    Miscellaneous,
    Currency,
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    FirstEdSadowlessHolographicCharizardKindaRare,
}
public enum WeaponType
{
    Melee,
    Ranged,
}

public enum Weapons
{
    None,
    Shortsword,
    Sword,
    Longsword,
    Greatsword,
    Dagger,
    Mace,
    Spear,
    Axe,
    Hammer,
    Shortbow,
    Bow,
    Longbow,
    Crossbow,
}
public enum WeaponProperties
{
    Finesse,
    Versatile,
    Twohanded,
    Enchanted,
    Heavy,
}
public enum ArmorTypes
{
    Heavy,
    Medium,
    Light,
    Shield,
}
public enum State
{
    Intro,
    Idle,
    Walk,
    Attack,
    Death,
    Combat,
}
public enum StateEnemy
{
    EnemyIdle,
    Alert,
    EnemyAttack,
    EnemyCombat,
}
public enum CombatSubState
{
    WaitForInput,
    PerformAction,
    ActionResult,
    TurnEnd,
    DecideAction,
}
public enum EquipSlot
{
    MainHand,
    OffHand,
    Helmet,
    Chest,
    Boots,
    Gloves,
    Cloak,
    Ranged,
    RangedTwo,
    RingOne,
    RingTwo,
    TrinketOne,
    TrinketTwo,
}
public enum actionCostType
{
    Action,
    BonusAction,
    Mana,
    Free,
}
public enum DamageTypes
{
    Slashing,
    Bludgeoning,
    Piercing,
    
    Fire,
    Ice,
    Water,
    Lightning,
    Necrotic,
    Poison,
    Wind,
}

public enum ActionTypes
{
    Action,
    Spell,
}
public enum debuffTypes
{
    None,
    Poison,
    Burn,
    Frozen,
    Bleed,
    Blindness,
    Silence,
    Rot,
    Frostbite,
}

public enum buffTypes
{
    Regeneration,
    ElementalInfusion,
    StatBoost,
    DebuffRemoval,
    
}
public enum characterStats
{
    Vitality,
    Defense,
    Mana,
    MagicPower,
    Agility,
    MovementCount,
    Strength,
    AttackPower,
    Luck,
    Charisma,
    ExperiencePoints,
}

public enum classTypes
{
    Warrior,
    Mage,
    Rogue,
    Druid,
    Paladin,
    Warlock,
    Ranger,
    ArcaneFighter,
}

public enum DescriptionPanel
{
    MainSprite,
    Name,
    TypeAndRarity,
    
    Damage,
    DamageDistribution,
    
    SkillDescription,
    ItemEffectSprite,
    ItemEffect,
    ItemEffectSprite2,
    ItemEffect2,
    GainableSkillSprite,
    GainableSkillDescription,
    
    DebuffSprite,
    DebuffDurationText,
    
    ActionTypeSprite,
    ActionTypeText,
    ActionRangeSprite,
    ActionRangeText,
    STorAOESprite,
    STorAOEText,
    
    FlavourText,
    
    ItemTypeSprite,
    ItemTypeText,
    ItemType2Sprite,
    ItemType2Text,
    ItemType3Sprite,
    ItemType3Text,
    
    ActionSprite,
    ActionText,
    BonusActionSprite,
    BonusActionText,
    ManaSprite,
    ManaText,
    
    ItemPriceSprite,
    ItemPriceText,
}
