using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class PartyManager : Node
{
	[Export] public Node ActorParty;
	public delegate void ListChangedDelegate(List<Player> previous, List<Player> current);

	public event ListChangedDelegate OnPlayerPartyChanged;
	
	public delegate void ActionDelegate(Player previous, Player current);
	
	public  event ActionDelegate OnMainPlayerChanged;
	
	private static List<Player> _playerParty = new();
	
	public static PartyManager Instance;
	
	public static Dictionary<Player, int> FollowPlayerDict = new();

	/// <summary>
	/// Custom add method, don't forget 
	/// </summary>
	public List<Player> PlayerParty
	{
		get => _playerParty;
		set
		{
			OnPlayerPartyChanged?.Invoke(_playerParty, value);
			_playerParty = value;
		}
	}

	private  Player _mainPlayer;
	public Player MainPlayer
	{
		get => _mainPlayer;
		set
		{
			OnMainPlayerChanged?.Invoke(_mainPlayer, value);
			if (_mainPlayer != null)
			{
				_mainPlayer.isControllable = false;
				_mainPlayer.Velocity = Vector2.Zero;
				_mainPlayer.direction = Vector2.Zero;
			}
			var previousPlayer = _mainPlayer;
			var newPlayerIndex = PlayerParty.IndexOf(value);
			PlayerParty[newPlayerIndex] = previousPlayer;
			
			_mainPlayer = value;
			
			PlayerParty[0] = _mainPlayer;
			_mainPlayer.isControllable = true;
			SortFollowIndexes();
			SetPlayerParty();
			_mainPlayer.SetPlayerState(State.Idle);
			previousPlayer?.SetPlayerState(State.FollowParty);
		}
	}
	private void SortFollowIndexes()
	{
		FollowPlayerDict.Clear();
		for (int i = 1; i < PlayerParty.Count; i++)
		{
			var path =
				Pathfinding.Instance.FindPath(_mainPlayer.GlobalPosition, PlayerParty[i].GlobalPosition).Length;
			FollowPlayerDict.Add(PlayerParty[i], path);
		}

		FollowPlayerDict.OrderBy(x => x.Value);

		_mainPlayer.FollowIndex = 0;

		var keyList = FollowPlayerDict.Keys;

		for (int i = 0; i < FollowPlayerDict.Values.Count; i++)
		{
			keyList.ElementAt(i).FollowIndex = i + 1;
		}
	}

	public override void _Ready()
	{
		Instance = this;
		SetPlayerParty();
		SortFollowIndexes();
	}
	
	private  void AddToPlayerParty(Player player)
	{
		var tempList = PlayerParty;
		if (!tempList.Contains(player))
		{
			tempList.Add(player);
		}

		PlayerParty = tempList;
	}

	private void RemoveFromPlayerParty(Player player)
	{
		var tempList = PlayerParty;
		if (tempList.Contains(player))
		{
			tempList.Remove(player);
		}

		PlayerParty = tempList;
	}

	private void SetPlayerParty()
	{
		PlayerParty.Clear();
		var allPlayers = ActorParty.GetChildren();
		foreach (var player in allPlayers)
		{
			AddToPlayerParty(player as Player);
		}
		
		_mainPlayer ??= PlayerParty[0];
	}
}
