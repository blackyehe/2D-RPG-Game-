using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class LevelUpUI : Control
{
    [Export] public Panel levelUpPanel;
    [Export] public RichTextLabel GainedStatsLabel;
    [Export] public TextureRect SelectedSkillBackGround;
    [Export] public Slot SelectedSkillSlot;
    [Export] public GridContainer SelectableSkillsContainer;
    [Export] public Panel SelectableSkillPanel;
    [Export] public Button SelectOKButton;
    [Export] public Button LevelUPConfirmButton;
    [Export] public CanvasLayer canvasLayer;
    [Export] public Button LevelUpButton;
    [Export] public PackedScene slotScene;

    [Export] public Slot SelectedTalentSlot;
    private TextureRect talentSlotTexture;
    [Export] public Panel SelectableTalentPanel;
    [Export] public GridContainer selectableTalentsContainer;
    [Export] public GridContainer SelectablePassivesContainer;
    [Export] public Button TalentConfirmButton;

    [Export] public DescriptionPanelUI descriptionPanel;

    //public Player player;
    private bool descriptionBool = false;

    public override void _Ready()
    {
        canvasLayer.Visible = false;
        LevelUpButton.Visible = false;
        SelectableSkillPanel.Visible = false;
        SelectedSkillBackGround.Visible = false;
        SelectOKButton.Visible = false;
        LevelUPConfirmButton.Visible = false;
        TalentConfirmButton.Visible = false;
        SelectableTalentPanel.Visible = false;
        GlobalEvents.Instance.OnSkillContainerChanged += OnSkillContainerChanged;
        GlobalEvents.Instance.OnLevelUp += OnPlayerLevelUp;
        LevelUPConfirmButton.Pressed += LevelUPConfirmButtonOnPressed;
        SelectedSkillSlot.OnSlotPressed += SelectSkillOnPressed;
        SelectedTalentSlot.OnSlotPressed += SelectedTalentOnSlotPressed;
        SelectOKButton.Pressed += SelectOKButtonOnPressed;
        LevelUpButton.Pressed += LevelUpButtonOnPressed;
        GlobalEvents.Instance.OnTalentLearned += OnPlayerTalentLearned;
        TalentConfirmButton.Pressed += TalentConfirmButtonOnPressed;
        PartyManager.Instance.OnMainPlayerChanged += OnMainPlayerChanged;
        talentSlotTexture = SelectedTalentSlot.itemIcon;
    }

    private void OnMainPlayerChanged(Player previous, Player current)
    {
        if (current.LevelUpAvailable)
        {
            canvasLayer.Visible = true;
            LevelUpButton.Visible = true;
            levelUpPanel.Visible = false;
            SelectableSkillPanel.Visible = false;
            GainedStatsLabel.Text = current.Stats.GetLvlUpStats();
        }
        SortLearnableTalents(current.selectableTalents,current);
        SelectedTalentSlot.SetSlotsEmpty();
    }
    private void OnPlayerTalentLearned(BaseSkill talent)
    {
        SortLearnableTalents(PartyManager.Instance.MainPlayer.selectableTalents,PartyManager.Instance.MainPlayer);
    }

    private void OnPlayerLevelUp(object sender, EventArgs e)
    {
        canvasLayer.Visible = true;
        LevelUpButton.Visible = true;
        levelUpPanel.Visible = false;
        SelectableSkillPanel.Visible = false;
        GainedStatsLabel.Text = PartyManager.Instance.MainPlayer.Stats.GetLvlUpStats();
    }

    private void LevelUpButtonOnPressed()
    {
        LevelUpButton.Visible = false;
        levelUpPanel.Visible = true;
        SelectedSkillBackGround.Visible = false;
        GetTree().Paused = true;
        GlobalEvents.Instance.EmitOnSkillContainerChanged(PartyManager.Instance.MainPlayer.selectableAbilities
            .ToList());
    }

    private void SelectSkillOnPressed(Slot slot)
    {
        SelectableSkillPanel.GlobalPosition = slot.GlobalPosition + new Vector2(370, -50);
        SelectableSkillPanel.Visible = !SelectableSkillPanel.Visible;
        GlobalEvents.Instance.EmitOnSkillContainerChanged(PartyManager.Instance.MainPlayer.selectableAbilities
            .ToList());
    }

    private void SelectOKButtonOnPressed()
    {
        SelectableSkillPanel.Visible = false;
        LevelUPConfirmButton.Visible = true;
    }

    private void LevelUPConfirmButtonOnPressed()
    {
        PartyManager.Instance.MainPlayer.Stats.OnLevelUp();
        GlobalEvents.Instance.EmitOnSkillBarChanged(PartyManager.Instance.MainPlayer.runtimeAbilities,
            PartyManager.Instance.MainPlayer);
        GlobalEvents.Instance.EmitInventoryStatsUpgraded(PartyManager.Instance.MainPlayer);
        GlobalEvents.Instance.EmitOnTalentLearned(SelectedTalentSlot.currentTalent);
        PartyManager.Instance.MainPlayer.LevelUpAvailable = false;
        canvasLayer.Visible = false;
        GetTree().Paused = false;
    }

    public void ClearGridContainer(GridContainer container)
    {
        while (container.GetChildCount() > 0)
        {
            var child = container.GetChild(0);
            container.RemoveChild(child);
            if (child is Slot slot)
            {
                slot.OnSlotEntered -= OnSkillButtonMouseEntered;
                slot.OnSlotExited -= OnSkillButtonMouseExited;
                slot.OnSlotPressed -= OnSkillButtonPressed;
                slot.OnSlotPressed -= OnTalentButtonPressed;
                slot.OnSlotPressed -= OnPassiveButtonPressed;
            }

            child.QueueFree();
        }
    }

    private void OnSkillContainerChanged(List<WeaponBaseAction> skill)
    {
        ClearGridContainer(SelectableSkillsContainer);
        foreach (WeaponBaseAction ability in skill)
        {
            var scene = slotScene.Instantiate();

            SelectableSkillsContainer.AddChild(scene);

            Slot slot = scene as Slot;

            slot.SetCustomMinimumSize(new Vector2(64, 64));
            slot.itemQuantityLabel.Visible = false;

            if (ability != null)
            {
                slot.SetAbility(ability);
            }
            else
            {
                slot.SetSlotsEmpty();
            }

            slot.OnSlotEntered += OnSkillButtonMouseEntered;
            slot.OnSlotExited += OnSkillButtonMouseExited;
            slot.OnSlotPressed += OnSkillButtonPressed;
        }
    }

    public void OnSkillButtonPressed(Slot slot)
    {
        var abilityToChoose = slot.currentAbilityAction;
        slot.itemQuantityLabel.Visible = false;
        SelectedSkillSlot.itemQuantityLabel.Visible = false;
        if (SelectedSkillSlot.currentAbilityAction != null)
        {
            PartyManager.Instance.MainPlayer.selectableAbilities.Add(SelectedSkillSlot.currentAbilityAction);
            PartyManager.Instance.MainPlayer.runtimeAbilities.Remove(SelectedSkillSlot.currentAbilityAction);
            ChangeSelectedSlotAbility(abilityToChoose);
        }
        else
        {
            ChangeSelectedSlotAbility(abilityToChoose);
        }

        SelectOKButton.Visible = true;
    }

    public void OnSkillButtonMouseEntered(Slot slot)
    {
        if (slot.currentItem != null)
        {
            slot.GetDescription(descriptionPanel,slot,slot.currentItem.ItemResource,selectableTalentsContainer.GlobalPosition,220,60);
            return;
        }

        if (slot.currentItem == null && slot.currentAbilityAction != null)
        {
            slot.GetDescription(descriptionPanel,slot,slot.currentAbilityAction,selectableTalentsContainer.GlobalPosition,220,60);
            return;
        }

        if (slot.currentItem == null && slot.currentTalent != null)
        {
            slot.GetDescription(descriptionPanel,slot,slot.currentTalent,selectableTalentsContainer.GlobalPosition,220,60);
        }
    }

    public void OnSkillButtonMouseExited(Slot slot)
    {
        
        descriptionPanel.Visible = false;
    }

    public void ChangeSelectedSlotAbility(WeaponBaseAction abilityToChoose)
    {
        SelectedSkillSlot.SetAbility(abilityToChoose);

        PartyManager.Instance.MainPlayer.runtimeAbilities.Add(abilityToChoose);
        PartyManager.Instance.MainPlayer.selectableAbilities.Remove(abilityToChoose);

        GlobalEvents.Instance.EmitOnSkillContainerChanged(PartyManager.Instance.MainPlayer.selectableAbilities
            .ToList());
    }

    private void SelectedTalentOnSlotPressed(Slot slot)
    {
        SelectableTalentPanel.Visible = !SelectableTalentPanel.Visible;
        SelectableTalentPanel.GlobalPosition = slot.GlobalPosition + new Vector2(370, -90);
    }

    private void SortLearnableTalents(List<BaseSkill> talents, Player currentPlayer)
    {
        ClearGridContainer(selectableTalentsContainer);
        ClearGridContainer(SelectablePassivesContainer);

        if (talents == null) return;
        foreach (var learnableTalent in talents.Where(x => x.IsMain))
        {
            var learntPassives = currentPlayer.LearnedTalents.Where(x =>
                !x.IsMain
                && x.SkillSchool == learnableTalent.SkillSchool
            ).ToList();

            if (learntPassives.Count < learnableTalent.SkillLevel) continue;

            var scene = slotScene.Instantiate();
            selectableTalentsContainer.AddChild(scene);
            scene.Reparent(selectableTalentsContainer);
            Slot slot = scene as Slot;

            slot.SetCustomMinimumSize(new Vector2(64, 64));
            slot.itemQuantityLabel.Visible = false;
            var DuplicateTalent = learnableTalent.Duplicate() as BaseSkill;
            DuplicateTalent.SkillLevel++;
            slot.SetTalent(DuplicateTalent);

            slot.OnSlotEntered += OnSkillButtonMouseEntered;
            slot.OnSlotExited += OnSkillButtonMouseExited;
            slot.OnSlotPressed += OnTalentButtonPressed;
        }

        foreach (var learnablePassive in talents.Where(x =>
                     !x.IsMain
                     && !currentPlayer.LearnedTalents.Contains(x)))
        {
            var scene = slotScene.Instantiate();
            SelectablePassivesContainer.AddChild(scene);
            Slot slot = scene as Slot;

            slot.SetCustomMinimumSize(new Vector2(64, 64));
            slot.itemQuantityLabel.Visible = false;
            slot.SetTalent(learnablePassive);

            slot.OnSlotEntered += OnSkillButtonMouseEntered;
            slot.OnSlotExited += OnSkillButtonMouseExited;
            slot.OnSlotPressed += OnPassiveButtonPressed;
        }
    }

    public void OnTalentButtonPressed(Slot slot)
    {
        var talentToChoose = slot.currentTalent;

        if (SelectedTalentSlot.currentTalent is { IsMain: true })
        {
            var currentTalent = SelectedTalentSlot.currentTalent;
            foreach (var spell in currentTalent.MainSkillEffect())
            {
                PartyManager.Instance.MainPlayer.selectableAbilities.Remove(spell);
            }

            SetSelectedTalentSlot(talentToChoose);
        }
        else if (SelectedTalentSlot.currentTalent is not { IsMain: true })
        {
            SetSelectedTalentSlot(talentToChoose);
        }

        SelectOKButton.Visible = true;
        TalentConfirmButton.Visible = true;
    }

    public void SetSelectedTalentSlot(BaseSkill NewTalent)
    {
        SelectedTalentSlot.SetTalent(NewTalent);
        if (!NewTalent.IsMain) return;

        var selectableAbilities = PartyManager.Instance.MainPlayer.selectableAbilities;
        var correspondingSpells =
            NewTalent.MainSkillEffect().Where(x => !selectableAbilities.Contains(x)).ToArray();
        Array<WeaponBaseAction> realCorrespondingSpells = [];
        for (int i = 0; i < correspondingSpells.Length; i++)
        {
            var spell = correspondingSpells[i];
            realCorrespondingSpells.Add(spell);
        }

        selectableAbilities.AddRange(realCorrespondingSpells);
        GlobalEvents.Instance.EmitOnSkillContainerChanged(selectableAbilities.ToList());
    }

    public void OnPassiveButtonPressed(Slot slot)
    {
        var talentToChoose = slot.currentTalent;
        if (SelectedTalentSlot.currentTalent is { IsMain: true })
        {
            var currentTalent = SelectedTalentSlot.currentTalent;
            foreach (var spell in currentTalent.MainSkillEffect())
            {
                PartyManager.Instance.MainPlayer.selectableAbilities.Remove(spell);
            }
        }

        SetSelectedTalentSlot(talentToChoose);

        SelectOKButton.Visible = true;
        TalentConfirmButton.Visible = true;
    }

    private void TalentConfirmButtonOnPressed()
    {
        SelectableTalentPanel.Visible = false;
        SelectedSkillBackGround.Visible = true;
    }

    public override void _Process(double delta)
    {
    }
}