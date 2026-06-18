using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class TalentLibrary : Node
{
	public static TalentLibrary Instance { get; private set; }

	[Export] public BaseSkill PyroMainTalent;
	[Export] public BaseSkill AlchemyMainTalent;
	[Export] public BaseSkill AeromancyMainTalent;
	[Export] public BaseSkill GeomancyMainTalent;
	[Export] public BaseSkill HemomancyMainTalent;
	[Export] public BaseSkill NecromancyMainTalent;
	[Export] public BaseSkill FightingMainTalent;
	[Export] public BaseSkill ArcheryMainTalent;
	[Export] public BaseSkill ThiefMainTalent;
	[Export] public BaseSkill DivineMainTalent;
	[Export] public BaseSkill HydromancyMainTalent;
	[Export] public BaseSkill SummonerMainTalent;

	public List<BaseSkill> AllMainTalents = new();
	//
	
	[Export] public Array<BaseSkill> PyroPassives;
	[Export] public Array<BaseSkill> AlchemyPassives;
	[Export] public Array<BaseSkill> AeromancyPassives;
	[Export] public Array<BaseSkill> GeomancyPassives;
	[Export] public Array<BaseSkill> HemomancyPassives;
	[Export] public Array<BaseSkill> NecromancyPassives;
	[Export] public Array<BaseSkill> FightingPassives;
	[Export] public Array<BaseSkill> ArcheryPassives;
	[Export] public Array<BaseSkill> ThiefPassives;
	[Export] public Array<BaseSkill> DivinePassives;
	[Export] public Array<BaseSkill> HydromancyPassives;
	[Export] public Array<BaseSkill> SummonerPassives;
	
	public List<Array<BaseSkill>> AllPassives = new();	
	
	
	public override void _Ready()
	{
		Instance = this;

		AllMainTalents = new()
		{
			PyroMainTalent,
			AlchemyMainTalent,
			AeromancyMainTalent,
			GeomancyMainTalent,
			HemomancyMainTalent,
			NecromancyMainTalent,
			FightingMainTalent,
			ArcheryMainTalent,
			ThiefMainTalent,
			DivineMainTalent,
			HydromancyMainTalent,
			SummonerMainTalent,
		};
		
		AllPassives = new ()
		{
			PyroPassives,
			AlchemyPassives,
			AeromancyPassives,
			GeomancyPassives,
			HemomancyPassives,
			NecromancyPassives,
			FightingPassives,
			ArcheryPassives,
			ThiefPassives,
			DivinePassives,
			HydromancyPassives,
			SummonerPassives,
		};
	}
}
