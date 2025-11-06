using Godot;
using System;

public partial class Main : Node
{
	private ColorRect blackout;
	private Timer blackoutTimer;

	public override void _Ready()
	{
		blackout = GetNode<ColorRect>("CanvasLayer/ColorRect");
		blackoutTimer = GetNode<Timer>("Timer");
		blackoutTimer.Timeout += OnBlackoutTimeout;
	}

	public override void _Process(double delta)
	{
		// Press B to start blackout
		if (Input.IsActionJustPressed("toggle_blackout"))
		{
			blackout.Visible = true;       // show black screen
			blackoutTimer.Start();         // start 5 second timer
		}
	}

	private void OnBlackoutTimeout()
	{
		blackout.Visible = false;          // hide black screen
	}
}
