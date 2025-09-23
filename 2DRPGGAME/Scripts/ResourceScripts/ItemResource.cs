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

    [Export] public WeaponBaseAction GainSkill;
    
    [Export] public string ItemFlavourDescription;
    
    [Export] public double ItemPriceText;
    
    [Export] public bool IsItemStackable;
    [Export] public int ItemQuantity;
    [Export] public EquipSlot equipSlot;

    public abstract Dictionary<DescriptionPanel,BaseDescription> GetDescription();
}