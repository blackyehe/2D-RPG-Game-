using Godot;
using System;
using Godot.Collections;

public abstract partial class ItemResource : Resource
{
    [Export] public string ItemName;
    [Export] public Texture2D Texture;
    [Export] public Rarity ItemRarity;
    [Export] public ItemType ItemType;
    
    [Export] public Texture2D EffectTexture1;
    [Export] public string EffectDescription1;
    [Export] public Texture2D EffectTexture2;
    [Export] public string EffectDescription2;

    [Export] public Texture2D GainSkillSprite;
    [Export] public string GainSkillNameAndType;
    
    [Export] public string ItemFlavourDescription;
    
    [Export] public Texture2D ItemTypeTexture1;
    [Export] public Texture2D ItemPropertyTexture1;
    [Export] public Texture2D ItemPropertyTexture2;

    [Export] public Texture2D ItemPriceTexture;
    [Export] public double ItemPriceText;
    
    [Export] public bool IsItemStackable;
    [Export] public int ItemQuantity;
    [Export] public EquipSlot equipSlot;

    public abstract Dictionary<DescriptionPanel,BaseDescription> GetDescription();
}