using Godot;
using System;

public partial class Game : Node
{
	private ColorRect blackout;
	private Timer blackoutTimer;
	private Timer randomTimer;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready()
	{
		GD.Print("READY: Script loaded");

		// Get node references
		blackout = GetNode<ColorRect>("CanvasLayer/ColorRect");
		blackoutTimer = GetNode<Timer>("BlackoutTimer");
		randomTimer = GetNode<Timer>("RandomTimer");

		// Debug node discovery
		GD.Print($"BlackoutTimer found? {blackoutTimer != null}");
		GD.Print($"RandomTimer found? {randomTimer != null}");
		GD.Print($"ColorRect found? {blackout != null}");

		// Make sure blackout rect is hidden initially
		blackout.Color = new Color(0, 0, 0, 1); // solid black
		blackout.Visible = false;

		// Configure blackout timer (how long the blackout lasts)
		blackoutTimer.WaitTime = 5.0f;
		blackoutTimer.OneShot = true;
		blackoutTimer.Timeout += OnBlackoutTimeout;

		// Configure random timer (how long until next blackout starts)
		randomTimer.OneShot = true;
		randomTimer.Timeout += OnRandomTimerTimeout;

		// Start the first random delay
		StartRandomCountdown();
	}

	private void StartRandomCountdown()
	{
		// Random delay between 10–30 seconds (adjust as you like)
		double randomDelay = rng.RandfRange(10.0f, 30.0f);
		GD.Print($"Starting random timer for {randomDelay:F1} seconds...");
		randomTimer.WaitTime = randomDelay;
		randomTimer.Start();
	}

	private void OnRandomTimerTimeout()
	{
		// When random timer expires → trigger blackout
		GD.Print("🕶️ RandomTimer timeout → starting blackout");
		blackout.Visible = true;
		blackoutTimer.Start();
	}

	private void OnBlackoutTimeout()
	{
		// When blackout ends → hide screen and start another countdown
		GD.Print("💡 BlackoutTimer timeout → ending blackout");
		blackout.Visible = false;
		StartRandomCountdown();
	}
}
