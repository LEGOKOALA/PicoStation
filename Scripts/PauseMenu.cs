using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private Button resumeButton;
	private Button optionsButton;
	private Button exitButton;
	private VBoxContainer vbox;

	public override void _Ready()
	{
		vbox = GetNode<VBoxContainer>("VBoxContainer");
		vbox.MouseFilter = Control.MouseFilterEnum.Stop;

		resumeButton = GetNode<Button>("VBoxContainer/Resume");
		optionsButton = GetNode<Button>("VBoxContainer/Options");
		exitButton = GetNode<Button>("VBoxContainer/Exit");

		resumeButton.Pressed += OnResumePressed;
		optionsButton.Pressed += OnOptionsPressed;
		exitButton.Pressed += OnExitPressed;

		Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause"))
		{
			TogglePause();
		}
	}

	private void TogglePause()
	{
		Visible = !Visible;
		GetTree().Paused = Visible;

		if (Visible)
			resumeButton.GrabFocus();
	}

	private void OnResumePressed()
	{
		GD.Print("resumeButton = " + resumeButton);
		GD.Print(Visible);
		TogglePause();
		GD.Print(Visible);
	}

	private void OnOptionsPressed()
	{
		GD.Print("Options clicked!");
	}

	private void OnExitPressed()
	{
		GetTree().Paused = false; // unpause before changing scene
		GetTree().ChangeSceneToFile("res://Scenes/StartMenu.tscn");
	}
}
