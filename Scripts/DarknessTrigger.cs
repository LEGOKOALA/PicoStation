using Godot;
using System;

public partial class DarknessTrigger : Area2D
{
	[Signal]
	public delegate void DarknessEnteredEventHandler(bool isDark);

	public override void _Ready()
	{
		AddToGroup("darkness_triggers");
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("players"))
		{
			GD.Print("Player entered DARKNESS AREA.");
			EmitSignal(SignalName.DarknessEntered, true);
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("players"))
		{
			GD.Print("Player LEFT DARKNESS AREA.");
			EmitSignal(SignalName.DarknessEntered, false);
		}
	}
}
