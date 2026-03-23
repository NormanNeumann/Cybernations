using Godot;
using System;
using System.Collections.Generic;
using Cybernations.Application.ViewModels;

namespace Cybernations.Scripts.Main.Components;

public partial class EnvisionPhasePanelView : PanelContainer
{
    [Signal] public delegate void ShiftPowerPressedEventHandler();
    [Signal] public delegate void ComeTogetherPressedEventHandler();
    [Signal] public delegate void ConnectPressedEventHandler();
    [Signal] public delegate void SetCoursePressedEventHandler();
    [Signal] public delegate void PreparePressedEventHandler();
    [Signal] public delegate void SteerPressedEventHandler();
    [Signal] public delegate void PassPressedEventHandler();

    private Label _phaseTitleLabel = null!;
    private Label _activePlayerLabel = null!;
    private Label _phaseHintLabel = null!;
    private Label _actionCostValueLabel = null!;
    private Label _statusMessageLabel = null!;
    private Label _actionDescriptionLabel = null!;

    private Button _shiftPowerButton = null!;
    private Button _comeTogetherButton = null!;
    private Button _connectButton = null!;
    private Button _setCourseButton = null!;
    private Button _prepareButton = null!;
    private Button _steerButton = null!;
    private Button _passButton = null!;

    private readonly List<Label> _feedbackSlotLabels = new();

    public override void _Ready()
    {
        _phaseTitleLabel = GetNode<Label>("MarginContainer/RootVBox/HeaderRow/PhaseTitleLabel");
        _activePlayerLabel = GetNode<Label>("MarginContainer/RootVBox/HeaderRow/ActivePlayerLabel");
        _phaseHintLabel = GetNode<Label>("MarginContainer/RootVBox/HeaderRow/PhaseHintLabel");

        _actionCostValueLabel = GetNode<Label>("MarginContainer/RootVBox/ActionCostPanel/HBoxContainer/ActionCostValueLabel");
        _statusMessageLabel = GetNode<Label>("MarginContainer/RootVBox/StatusPanel/VBoxContainer/StatusMessageLabel");
        _actionDescriptionLabel = GetNode<Label>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionDescriptionLabel");

        _shiftPowerButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/ShiftPowerButton");
        _comeTogetherButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/ComeTogetherButton");
        _connectButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/ConnectButton");
        _setCourseButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/SetCourseButton");
        _prepareButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/PrepareButton");
        _steerButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/SteerButton");
        _passButton = GetNode<Button>("MarginContainer/RootVBox/ActionsPanel/VBoxContainer/ActionButtonsGrid/PassButton");

        BindFeedbackSlots();
        BindButtons();
        SetupButtonText();
    }

    private void BindFeedbackSlots()
    {
        var row = GetNode<HBoxContainer>("MarginContainer/RootVBox/FeedbackTrackPanel/VBoxContainer/FeedbackSlotsRow");
        for (int i = 1; i <= 11; i++)
        {
            var label = row.GetNode<Label>($"FeedbackSlot{i}/FeedbackSlot{i}Label");
            _feedbackSlotLabels.Add(label);
        }
    }

    private void BindButtons()
    {
        _shiftPowerButton.Pressed += () => EmitSignal(SignalName.ShiftPowerPressed);
        _comeTogetherButton.Pressed += () => EmitSignal(SignalName.ComeTogetherPressed);
        _connectButton.Pressed += () => EmitSignal(SignalName.ConnectPressed);
        _setCourseButton.Pressed += () => EmitSignal(SignalName.SetCoursePressed);
        _prepareButton.Pressed += () => EmitSignal(SignalName.PreparePressed);
        _steerButton.Pressed += () => EmitSignal(SignalName.SteerPressed);
        _passButton.Pressed += () => EmitSignal(SignalName.PassPressed);

        _shiftPowerButton.MouseEntered += () => SetActionDescription("SHIFT POWER\nCost: 1 People Relationship\nEffect: Give the First Player token to any player.");
        _comeTogetherButton.MouseEntered += () => SetActionDescription("COME TOGETHER\nCost: 1 Environment Relationship\nEffect: Gain 1 Cohesion.");
        _connectButton.MouseEntered += () => SetActionDescription("CONNECT\nCost: 2 of the same Relationship\nEffect: Gain 1 Relationship of your choice.");
        _setCourseButton.MouseEntered += () => SetActionDescription("SET COURSE\nCost: 2 Technology Relationship\nEffect: Move 1 People token or rotate 1 Stack.");
        _prepareButton.MouseEntered += () => SetActionDescription("PREPARE\nCost: 2 People Relationship\nEffect: Gain 1 Cybernation level.");
        _steerButton.MouseEntered += () => SetActionDescription("STEER\nCost: 2 Environment Relationship\nEffect: Add or manipulate Feedback tokens.");
        _passButton.MouseEntered += () => SetActionDescription("PASS\nCost: Free\nEffect: Take no further action this Envision Phase.");
    }

    private void SetupButtonText()
    {
        _shiftPowerButton.Text = "Shift Power";
        _comeTogetherButton.Text = "Come Together";
        _connectButton.Text = "Connect";
        _setCourseButton.Text = "Set Course";
        _prepareButton.Text = "Prepare";
        _steerButton.Text = "Steer";
        _passButton.Text = "Pass";
    }

    private void SetActionDescription(string text)
    {
        _actionDescriptionLabel.Text = text;
    }

    public void Configure(EnvisionPhaseVm vm)
    {
        Visible = vm.IsVisible;

        _phaseTitleLabel.Text = vm.PhaseTitle;
        _activePlayerLabel.Text = vm.ActivePlayerText;
        _phaseHintLabel.Text = vm.PhaseHintText;
        _actionCostValueLabel.Text = vm.ActionCostText;
        _statusMessageLabel.Text = vm.StatusMessage;

        for (int i = 0; i < _feedbackSlotLabels.Count; i++)
        {
            _feedbackSlotLabels[i].Text =
                i < vm.FeedbackSlots.Count ? vm.FeedbackSlots[i] : "-";
        }

        _shiftPowerButton.Disabled = !vm.CanShiftPower;
        _comeTogetherButton.Disabled = !vm.CanComeTogether;
        _connectButton.Disabled = !vm.CanConnect;
        _setCourseButton.Disabled = !vm.CanSetCourse;
        _prepareButton.Disabled = !vm.CanPrepare;
        _steerButton.Disabled = !vm.CanSteer;
        _passButton.Disabled = !vm.CanPass;
    }
}