using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class AbilityLibrary : Node
{
    public static AbilityLibrary Instance { get; private set; }

    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Pyromancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Alchemy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Aeromancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Geomancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Hemomancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Necromancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Fighting;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Archery;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Thief;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Divine;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Hydromancy;
    [Export] public Godot.Collections.Dictionary<int, AbilityArray> Summoner;
    public List<Godot.Collections.Dictionary<int, AbilityArray>> AllAbilities = new();

    public override void _Ready()
    {
        Instance = this;
        AllAbilities =  new List<Godot.Collections.Dictionary<int, AbilityArray>>
        {
            Pyromancy,
            Alchemy,
            Aeromancy,
            Geomancy,
            Hemomancy,
            Necromancy,
            Fighting,
            Archery,
            Thief,
            Divine,
            Hydromancy,
            Summoner,
        };
    }
}