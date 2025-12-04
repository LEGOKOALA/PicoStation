using Godot;
using System;

public partial class StartMenu : Control
{
	private Button startButton;
	private Button optionsButton;
	private Button quitButton;

	private const string GameScenePath = "res://Scenes/Game.tscn";

	public override void _Ready()
	{
		startButton = GetNode<Button>("VBoxContainer/StartButton");
		optionsButton = GetNode<Button>("VBoxContainer/OptionsButton");
		quitButton = GetNode<Button>("VBoxContainer/QuitButton");

		// Enable focus navigation
		startButton.FocusMode = FocusModeEnum.All;
		optionsButton.FocusMode = FocusModeEnum.All;
		quitButton.FocusMode = FocusModeEnum.All;

		// Focus the first button
		startButton.GrabFocus();

		// Connect signals
		startButton.Pressed += OnStartButtonPressed;
		optionsButton.Pressed += OnOptionsButtonPressed;
		quitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnStartButtonPressed()
	{
		var gameScene = ResourceLoader.Load<PackedScene>(GameScenePath);
		if (gameScene != null)
			GetTree().ChangeSceneToPacked(gameScene);
		else
			GD.Print("Error: Game scene not found at " + GameScenePath);
	}

	private void OnOptionsButtonPressed()
	{
		GD.Print("Options clicked!");
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
