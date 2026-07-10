using Godot;
using System;
using System.Collections.Generic;

public abstract partial class StatusEffectBase : Resource
{
    [Export] public Texture2D Sprite;
    [Export] public string Name;
    [Export] public string Description;
    [Export] public double Duration;
    [Export] public bool IsStackable;
    public double CurrentDuration;
    public abstract void RemoveFromList(CombatActor target);
    public static void RemoveStatus(CombatActor target)
    {
        for (int i = 0; i < target.statusEffectList.Count; i++)
        {
            if (!(target.statusEffectList[i].CurrentDuration <= 0)) continue;
            
            if (target.statusEffectList[i] is Debuff debuff) debuff.RemoveFromList(target);
            if (target.statusEffectList[i] is Buff buff) buff.RemoveFromList(target);
            
            target.statusEffectList.Remove(target.statusEffectList[i]);
            
            i--;
        }
    }
}

