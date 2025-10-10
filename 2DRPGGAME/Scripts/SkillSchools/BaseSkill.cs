using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;


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
        if (!IsMain) return null;
        var currentAbilities = Abilities[SkillLevel];
        Array<WeaponBaseAction> skillArray = new();
        foreach (var skill in currentAbilities.skillArray.Where(x => x.SkillLevel <= SkillLevel))
        {
            skillArray.Add(skill); //Main skill level upnál figyelni kéne, hogy hamarabb történjen meg ^ emiatt kösz heló.
        }

        return skillArray;
    }

    public List<BaseSkill> GetPassivesBySkillSchool(BaseSkill mainTalent)
    {
        if (!IsMain) return null;
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
        }

        return passives;
    }

    public abstract void PassiveSkillEffect(CombatActor user);
}