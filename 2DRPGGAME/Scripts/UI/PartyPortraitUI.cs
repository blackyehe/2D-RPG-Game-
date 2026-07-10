using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class PartyPortraitUI : Control
{
    [Export] private PackedScene PortraitScene;
    [Export] private VBoxContainer PartyPortraitContainer;
    [Export] private CanvasLayer CanvasLayer;

    public override void _Ready()
    {
        CanvasLayer.Visible = true;
        PartyManager.Instance.OnPlayerPartyChanged += OnPlayerPartyChanged;

        if (PartyManager.Instance.PlayerParty.Count > 1)
        {
            GetPartyPortraits();
        }
    }

    private void OnPlayerPartyChanged(List<Player> previous, List<Player> current)
    {
        GetPartyPortraits();
    }

    private void GetPartyPortraits()
    {
        ClearContainer(PartyPortraitContainer);
        for (int i = 0; i < PartyManager.Instance.PlayerParty.Count; i++)
        {
            var scene = PortraitScene.Instantiate();

            PartyPortraitContainer.AddChild(scene);
            Portrait portrait = scene as Portrait;

            portrait.Visible = true;
            portrait.SetCustomMinimumSize(new Vector2(140, 160));
            portrait.SetPortrait(PartyManager.Instance.PlayerParty[i]);

            PartyManager.Instance.PlayerParty[i].OnDamageTaken += OnDamageTaken;
            PartyManager.Instance.PlayerParty[i].OnActorDeath += OnActorDeath;

            portrait.OnPortraitPressed += OnPortraitPressed;
            portrait.OnPortraitExited += OnPortraitExited;
            portrait.OnPortraitEntered += OnPortraitEntered;
        }
    }

    private void OnActorDeath(object sender, EventArgs e)
    {
        GetPartyPortraits();
    }

    private void OnDamageTaken(CombatActor actor, float damage)
    {
        GetPartyPortraits();
    }

    public void ClearContainer(VBoxContainer container)
    {
        while (container.GetChildCount() > 0)
        {
            var child = container.GetChild(0);
            container.RemoveChild(child);

            if (child is Portrait portrait)
            {
                portrait.OnPortraitPressed -= OnPortraitPressed;
                portrait.OnPortraitExited -= OnPortraitExited;
                portrait.OnPortraitEntered -= OnPortraitEntered;
            }

            for (int i = 0; i < container.GetChildCount(); i++)
            {
                PartyManager.Instance.PlayerParty[i].OnDamageTaken -= OnDamageTaken;
                PartyManager.Instance.PlayerParty[i].OnActorDeath -= OnActorDeath;
            }

            child.QueueFree();
        }
    }

    private void OnPortraitEntered(Portrait portrait)
    {
        //throw new NotImplementedException();
    }

    private void OnPortraitExited(Portrait portrait)
    {
        //throw new NotImplementedException();
    }

    private void OnPortraitPressed(Portrait portrait)
    {
        if (portrait.Player == PartyManager.Instance.MainPlayer) return;
        PartyManager.Instance.MainPlayer = portrait.Player;
    }
    
    
    
    
}