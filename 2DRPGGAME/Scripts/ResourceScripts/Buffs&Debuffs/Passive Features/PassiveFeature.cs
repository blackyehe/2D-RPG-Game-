using Godot;
using System;

public abstract partial class PassiveFeature : Resource
{
    [Export] public string FeatureName;
    [Export] public Texture2D PassiveFeatureSprite;
    [Export] public string FeatureDescription;
    [Export] public StatusEffectBase AppliedStatusEffect;

    public void GetPassiveFeature(CombatActor user)
    {
        if (user.PassiveFeatures.Contains(this)) return;
        
        user.PassiveFeatures.Add(this);
    }
    public void RemovePassiveFeature(CombatActor user)
    {
        user.PassiveFeatures.Remove(this);
    }
    
    public abstract bool IsFeatureEffectLegal(CombatActor user, DamageWithType damage, CombatActor target);
    public abstract void PassiveFeatureEffect(CombatActor user, DamageWithType damage, CombatActor target);
    
}
