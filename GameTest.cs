using Godot;
using System;

public partial class GameTest : Node2D
{
	private ColorRect blackout;
	private Timer blackoutTimer;
	private Timer durationTimer;
	private Random random = new Random();

	public override void _Ready()
	{
		GD.Print("✅ GameTest.cs started!");
		
		blackout = GetNode<ColorRect>("CanvasLayer/Blackout");
		blackoutTimer = GetNode<Timer>("BlackoutTimer");
		durationTimer = GetNode<Timer>("DurationTimer");

		// Safety check
		if (blackout == null || blackoutTimer == null || durationTimer == null)
		{
			GD.PrintErr("❌ Make sure Blackout, BlackoutTimer, and DurationTimer nodes exist!");
			return;
		}
		GD.Print($"Blackout: {GetNode("Blackout")}");
		GD.Print($"BlackoutTimer: {GetNode("BlackoutTimer")}");
		GD.Print($"DurationTimer: {GetNode("DurationTimer")}");
		
		// Make sure blackout is initially hidden
		
		blackout.Visible = false;

		// Configure timers
		blackoutTimer.OneShot = true;
		durationTimer.OneShot = true;

		// Connect timer signals via code
		blackoutTimer.Timeout += OnBlackoutTimerTimeout;
		durationTimer.Timeout += OnDurationTimerTimeout;

		// Start the first random blackout
		StartRandomBlackoutTimer();
	}

	private void StartRandomBlackoutTimer()
	{
		float waitTime = (float)(random.NextDouble() * 5.0 + 5.0);
		blackoutTimer.WaitTime = waitTime;
		blackoutTimer.Start();

		GD.Print($"⏳ Next blackout in {waitTime:F1} seconds");
		GD.Print($"BlackoutTimer.IsStopped = {blackoutTimer.IsStopped()}");

	}

	private void OnBlackoutTimerTimeout()
	{
		// Show the blackout
		blackout.Visible = true;

		// Duration of blackout = 5 seconds
		durationTimer.WaitTime = 5.0f;
		durationTimer.Start();

		GD.Print("🌑 BLACKOUT started!");
	}

	private void OnDurationTimerTimeout()
	{
		// Hide blackout
		blackout.Visible = false;
		GD.Print("☀️ Blackout ended.");

		// Start the next random blackout
		StartRandomBlackoutTimer();
	}

	public override void _Process(double delta)
	{
		// Optional: toggle blackout manually for testing
		if (Input.IsActionJustPressed("ui_accept"))
		{
			blackout.Visible = !blackout.Visible;
			GD.Print($"Toggled blackout manually: {blackout.Visible}");
		}
	}
}
