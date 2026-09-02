using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class BaseDescription : Control
{
    public string stringData;
    public Texture2D textureData;
    public List<StringWithColor> stringsWithColor = new();

    public BaseDescription(string stringData)
    {
        this.stringData = stringData;
    }

    public BaseDescription(Texture2D textureData)
    {
        this.textureData = textureData;
    }
    
    public BaseDescription(Godot.Collections.Dictionary<DamageTypes, int> damageDistributions)
    {
        if (damageDistributions == null)
        {
            stringData = "URES A DISTRIBUTION (NINCSEN)";
            return;
        }

        stringsWithColor = damageDistributions.Select(x =>
            new StringWithColor()
            {
                ColorData = ColorLibrary.GetColorByDamageType(x.Key), 
                TextData = $"\t{x.Value.ToString()} {x.Key}"
            }).ToList();
    }

    public BaseDescription(List<StringWithColor> stringsWithColor)
    {
        this.stringsWithColor = stringsWithColor;
    }

    public BaseDescription(List<(StringsThatNeedColor color, string stringData)> stringList)
    {
        stringsWithColor = stringList.Select(x =>
            new StringWithColor
            {
                ColorData = x.color, 
                TextData = $"{x.stringData.ToString()}"
            }).ToList();
    }

    public override void _Ready()
    {
    }
}

public struct StringWithColor()
{
    public string TextData;
    public StringsThatNeedColor ColorData;
    
}