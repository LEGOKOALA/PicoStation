using Godot;
using System;

public partial class StartMenu : Control
{
	private const string GameScenePath = "res://Scenes/Game.tscn";
	private Button[] buttons;

	public override void _Ready()
	{
		// Get buttons
		buttons = new Button[]
		{
			GetNode<Button>("VBoxContainer/StartButton"),
			GetNode<Button>("VBoxContainer/OptionsButton"),
			GetNode<Button>("VBoxContainer/QuitButton")
		};

		// Make buttons focusable
		foreach (var btn in buttons)
			btn.FocusMode = FocusModeEnum.All;

		// Focus the first button
		buttons[0].GrabFocus();

		// Connect signals properly
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
			// Get currently focused control
			var focusedButton = this.GetFocusOwner() as Button;
			if (focusedButton != null)
				focusedButton.Pressed(); // manually trigger press
		}
	}

	private void MoveFocus(int direction)
	{
		var current = this.GetFocusOwner() as Button;
		int index = Array.IndexOf(buttons, current);

		if (index == -1)
			index = 0;
		else
			index = (index + direction + buttons.Length) % buttons.Length;

		buttons[index].GrabFocus();
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
