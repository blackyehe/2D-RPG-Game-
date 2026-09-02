using Godot;
using System;
using Godot.Collections;
using Godot.NativeInterop;

public partial class ColorLibrary : Node
{
    public static ColorLibrary Instance { get; private set; }
    public static Dictionary<StringsThatNeedColor, Color> colorDict = new()
    {
        { StringsThatNeedColor.BasicColor, Colors.White },
        { StringsThatNeedColor.Pyromancer, Colors.Red },
        { StringsThatNeedColor.Summoner, Colors.DarkBlue },
        { StringsThatNeedColor.Necromancer, Colors.DarkGreen },
        { StringsThatNeedColor.Aeromancer, Colors.Blue },
        { StringsThatNeedColor.Hemomancer, Colors.Crimson },
        { StringsThatNeedColor.Hydromancer, Colors.AliceBlue },
        { StringsThatNeedColor.Geomancer, Colors.Brown },
        { StringsThatNeedColor.Divine, Colors.Yellow },
        { StringsThatNeedColor.Archery, Colors.RosyBrown },
        { StringsThatNeedColor.Fighting, Colors.RosyBrown },
        { StringsThatNeedColor.Thief, Colors.DarkGray },
        { StringsThatNeedColor.Alchemist, Colors.ForestGreen },
        { StringsThatNeedColor.Slashing, Colors.Gray },
        { StringsThatNeedColor.Bludgeoning, Colors.Gray },
        { StringsThatNeedColor.Piercing, Colors.Gray },
        { StringsThatNeedColor.Fire, Colors.Red },
        { StringsThatNeedColor.Ice, Colors.DarkBlue },
        { StringsThatNeedColor.Water, Colors.AliceBlue },
        { StringsThatNeedColor.Lightning, Colors.CadetBlue },
        { StringsThatNeedColor.Necrotic, Colors.DarkGreen },
        { StringsThatNeedColor.Poison, Colors.ForestGreen },
        { StringsThatNeedColor.Wind, Colors.Blue },
        { StringsThatNeedColor.Common, Colors.Gray },
        { StringsThatNeedColor.Uncommon, Colors.Green },
        { StringsThatNeedColor.Rare, Colors.Blue },
        { StringsThatNeedColor.Epic, Colors.Purple },
        { StringsThatNeedColor.Legendary, Colors.Gold }
    };
    
    public override void _Ready()
    {
        Instance = this;
    }


    public static string GetStringColor(string textData, DamageTypes damageType)
    {
        return DescriptionPanelUI.ColorRichText(colorDict[GetColorByDamageType(damageType)], textData);
    }
    
    public static string GetStringColor(string textData, Rarity rarity)
    {
        return DescriptionPanelUI.ColorRichText(colorDict[GetColorByRarity(rarity)], textData);
    }
    
    public static string GetStringColor(string textData, SkillSchools school)
    {
        return DescriptionPanelUI.ColorRichText(colorDict[GetColorBySchool(school)], textData);
    }
    
    
    public static StringsThatNeedColor GetColorBySchool(SkillSchools skillSchool)
    {
        switch (skillSchool)
        {
            case SkillSchools.Aeromancer:
                return StringsThatNeedColor.Aeromancer;
            
            case SkillSchools.Pyromancer:
                return StringsThatNeedColor.Pyromancer;
            
            case SkillSchools.Summoner:
                return StringsThatNeedColor.Summoner;
            
            case SkillSchools.Necromancer:
                return StringsThatNeedColor.Necromancer;
            
            case SkillSchools.Archery:
                return StringsThatNeedColor.Archery;
            
            case SkillSchools.Fighting:
                return StringsThatNeedColor.Fighting;
            
            case SkillSchools.Geomancer:
                return StringsThatNeedColor.Geomancer;
            
            case SkillSchools.Alchemist:
                return StringsThatNeedColor.Alchemist;
            
            case SkillSchools.Hemomancer:
                return StringsThatNeedColor.Hemomancer;
            
            case SkillSchools.Hydromancer:
                return StringsThatNeedColor.Hydromancer;
            
            case SkillSchools.Thief:
                return StringsThatNeedColor.Thief;
            
            case SkillSchools.Divine:
                return StringsThatNeedColor.Divine;
        }
        return new StringsThatNeedColor();
    }

    public static StringsThatNeedColor GetColorByDamageType(DamageTypes damageType)
    {
        switch (damageType)
        {
            case DamageTypes.Fire:
                return StringsThatNeedColor.Fire;
            
            case DamageTypes.Ice:
                return StringsThatNeedColor.Ice;
            
            case DamageTypes.Water:
                return StringsThatNeedColor.Water;
            
            case DamageTypes.Bludgeoning:
                return StringsThatNeedColor.Bludgeoning;
            
            case DamageTypes.Lightning:
                return StringsThatNeedColor.Lightning;
            
            case DamageTypes.Necrotic:
                return StringsThatNeedColor.Necrotic;
            
            case DamageTypes.Poison:
                return StringsThatNeedColor.Poison;
            
            case DamageTypes.Wind:
                return StringsThatNeedColor.Wind;
            
            case DamageTypes.Slashing:
                return StringsThatNeedColor.Slashing;
            
            case DamageTypes.Piercing:
                return StringsThatNeedColor.Piercing;
        }
        return new StringsThatNeedColor();
    }

    public static StringsThatNeedColor GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common :
                return StringsThatNeedColor.Common;
            
            case Rarity.Rare :
                return StringsThatNeedColor.Rare;
            
            case Rarity.Epic  :
                return StringsThatNeedColor.Epic;
            
            case Rarity.Legendary:
                return StringsThatNeedColor.Legendary;
            
        }
        return new StringsThatNeedColor();
    }
}

public static partial class EnumToString
{
    public static Dictionary<StringsThatNeedColor, string> enumToStringDict = new()
    {
        { StringsThatNeedColor.BasicColor, "Basic Color"},
        { StringsThatNeedColor.Pyromancer,  "Pyromancer"},
        { StringsThatNeedColor.Summoner, "Summoner"},
        { StringsThatNeedColor.Necromancer, "Necromancer" },
        { StringsThatNeedColor.Aeromancer, "Aeromancer" },
        { StringsThatNeedColor.Hemomancer, "Hemomancer" },
        { StringsThatNeedColor.Hydromancer,"Hydromancer" },
        { StringsThatNeedColor.Geomancer, "Geomancer" },
        { StringsThatNeedColor.Divine, "Divine" },
        { StringsThatNeedColor.Archery, "Archery" },
        { StringsThatNeedColor.Fighting, "Fighting" },
        { StringsThatNeedColor.Thief, "Thief" },
        { StringsThatNeedColor.Alchemist, "Alchemist" },
        { StringsThatNeedColor.Slashing, "Slashing" },
        { StringsThatNeedColor.Bludgeoning, "Bludgeoning" },
        { StringsThatNeedColor.Piercing, "Piercing" },
        { StringsThatNeedColor.Fire, "Fire"},
        { StringsThatNeedColor.Ice, "Ice" },
        { StringsThatNeedColor.Water, "Water" },
        { StringsThatNeedColor.Lightning, "Lightning" },
        { StringsThatNeedColor.Necrotic, "Necrotic" },
        { StringsThatNeedColor.Poison, "Poison" },
        { StringsThatNeedColor.Wind, "Wind" },
        { StringsThatNeedColor.Common, "Common" },
        { StringsThatNeedColor.Uncommon, "Uncommon" },
        { StringsThatNeedColor.Rare, "Rare" },
        { StringsThatNeedColor.Epic, "Epic" },
        { StringsThatNeedColor.Legendary, "legendary" }
    };

    
}