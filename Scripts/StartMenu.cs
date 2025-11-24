using Godot;
using System;

public partial class StartMenu : Control
{
	private const string GameScenePath = "res://Scenes/Game.tscn";
	private Button[] buttons;
	private int focusedIndex = 0;

	public override void _Ready()
	{
		buttons = new Button[]
		{
			GetNode<Button>("VBoxContainer/StartButton"),
			GetNode<Button>("VBoxContainer/OptionsButton"),
			GetNode<Button>("VBoxContainer/QuitButton")
		};

		foreach (var btn in buttons)
			btn.FocusMode = FocusModeEnum.All;

		// Focus the first button
		focusedIndex = 0;
		buttons[focusedIndex].GrabFocus();

		// Connect signals using Connect() method
		buttons[0].Connect("pressed", new Callable(this, nameof(OnStartButtonPressed)));
		buttons[1].Connect("pressed", new Callable(this, nameof(OnOptionsButtonPressed)));
		buttons[2].Connect("pressed", new Callable(this, nameof(OnQuitButtonPressed)));
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_down"))
			MoveFocus(1);
		else if (@event.IsActionPressed("ui_up"))
			MoveFocus(-1);
		else if (@event.IsActionPressed("ui_accept"))
		{
			// Correct way to trigger button press
			buttons[focusedIndex].EmitSignal("pressed");
		}
	}

	private void MoveFocus(int direction)
	{
		focusedIndex = (focusedIndex + direction + buttons.Length) % buttons.Length;
		buttons[focusedIndex].GrabFocus();
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
