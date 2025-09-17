using Godot;
using System;
using Godot.Collections;

public partial class PlayerStats : Resource
{
    [Export] public string CharacterName;
    [Export] public classTypes Class;
    [Export] public double BaseHP;
    [Export] public double BaseMP;
    [Export] public double BaseAttack;
    [Export] public double BaseDefense;
    [Export] public double BaseStrength;
    [Export] public double BaseAgility;
    [Export] public double BaseCharisma;
    [Export] public double BaseMagicPower;
    [Export] public double BaseLuck;
    [Export] public double BaseSpeed;
    
    [Export] public Dictionary<actionCostType, int> remainingCost;
    [Export] public int ActionCount;
    [Export] public int BonusActionCount;
    [Export] public double XP;
    [Export] public int Level;
    [Export] public double RewardXP;
    [Export] public int TileMovementCount;
    [Export] public double LightAttackAnimDuration;
    [Export] public double HeavyAttackAnimDuration;
    [Export] public double RangedShotAnimDuration;
    [Export] public Array<double> XPThresholds;
}
