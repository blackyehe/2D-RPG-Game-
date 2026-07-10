using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Array = Godot.Collections.Array;


public abstract partial class BaseSkill : Resource
{
    public bool PassivesAdded = false;
    [Export] public SkillSchools SkillSchool;
    [Export] public bool IsMain;
    [Export] public int SkillLevel;
    [Export] public string SkillName;
    [Export] public Texture2D Sprite;
    [Export] public string Description;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Abilities = new();

    public Godot.Collections.Dictionary<DescriptionPanel, BaseDescription> GetDescription(CombatActor user)
    {
        var description = new Godot.Collections.Dictionary<DescriptionPanel, BaseDescription>();
        description[DescriptionPanel.Name] = new(SkillName);
        description[DescriptionPanel.MainSprite] = new(Sprite);
        if (IsMain)
        {
            description[DescriptionPanel.TypeAndRarity] =
                new($"Level {SkillLevel} {SkillSchool} Primary Talent");
        }
        else
        {
            description[DescriptionPanel.TypeAndRarity] = new($"{SkillSchool} Passive Talent");
        }

        description[DescriptionPanel.SkillDescription] = new(Description);

        return description;
    }

    public Array<WeaponBaseAction> MainSkillEffect()
    {
        
        var currAbilities = Abilities;
        Array<WeaponBaseAction> skillArray2 = [];
        for (int i = 1; i <= SkillLevel; i++)
        {
            var skillsToAdd = currAbilities[i].skillArray;
            skillArray2.AddRange(skillsToAdd);
        }
        return skillArray2;
    }

    public List<BaseSkill> GetPassivesBySkillSchool(BaseSkill mainTalent)
    {
        if (!IsMain) return [];
        List<BaseSkill> passives = new();
        var allPassives = TalentLibrary.Instance.AllPassives;
        for (int i = 0; i < allPassives.Count; i++)
        {
            var currentPassiveArray = allPassives[i];
            var firstPassive = currentPassiveArray?.FirstOrDefault();
            
            if (firstPassive != null && firstPassive.SkillSchool == mainTalent.SkillSchool)
            {
                passives.AddRange(currentPassiveArray);
            }

            passives =
                passives.Where(x => !PartyManager.Instance.MainPlayer.LearnedTalents.Contains(x)).ToList();
        }

        return passives;
    }

    public abstract DamageWithType PassiveSkillEffect(CombatActor user, DamageTypes damageType,float damageNumber, CombatActor enemy);
    public abstract bool PassiveIsLegal(CombatActor user, DamageWithType dmgWithType, CombatActor enemy);
    
}

public struct DamageWithType
{
    public DamageTypes dmgType;
    public float dmgNumber;
    
    
}