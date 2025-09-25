using Godot;
using System;
using Godot.Collections;
using System.Linq;

public partial class BaseDescription : Control
{
	public string stringData;
	public Texture2D textureData;

	public BaseDescription(string stringData)
	{
		this.stringData = stringData;
	}
	public BaseDescription(Texture2D textureData)
	{
		this.textureData = textureData;
	}
	public BaseDescription(Dictionary<DamageTypes, int> damageDistributions)
	{
		if (damageDistributions == null)
		{
			stringData = "URES A DISTRIBUTION (NINCSEN)";
			return;
		}
		var dmgDist =damageDistributions.Select(x => $"\t{x.Key} {x.Value}");
		stringData = string.Join("\n", dmgDist);
	}
	
	public override void _Ready()
	{
	}
}
