using Godot;
using System;

public partial class Game : Node2D
{
	private ColorRect _blackout;      
	private Timer _blackoutTimer;      
	private Timer _durationTimer;      
	private Random _rand = new Random();

	public override void _Ready()
	{
		
		GD.Print("READY CHECK");

		// Get the Blackout ColorRect from CanvasLayer
		_blackout = GetNode<ColorRect>("CanvasLayer/Blackout");
		if (_blackout == null)
		{
			GD.PrintErr("❌ Blackout ColorRect not found! Check path.");
			return;
		}

		// Make sure blackout is initially hidden
		_blackout.Visible = false;

		// Create and configure timers
		_blackoutTimer = new Timer();
		_blackoutTimer.OneShot = true;
		AddChild(_blackoutTimer);
		_blackoutTimer.Timeout += OnBlackoutTimerTimeout;

		_durationTimer = new Timer();
		_durationTimer.OneShot = true;
		AddChild(_durationTimer);
		_durationTimer.Timeout += OnDurationTimerTimeout;

		// Start the first random blackout timer
		StartBlackoutTimer();
	}

	// Starts the blackout timer with a random delay (3-10 seconds)
	private void StartBlackoutTimer()
	{
		float delay = (float)(_rand.NextDouble() * 7.0 + 3.0); // random 3-10 sec
		_blackoutTimer.WaitTime = delay;
		_blackoutTimer.Start();
		GD.Print("Next blackout in ", delay, " seconds.");
	}

	// Called when blackout timer finishes — shows the blackout
	private void OnBlackoutTimerTimeout()
	{
		_blackout.Visible = true;
		_durationTimer.WaitTime = 5.0f; // blackout lasts 4 seconds
		_durationTimer.Start();
		GD.Print("Blackout started!");
	}

	// Called when blackout duration timer finishes — hides the blackout
	private void OnDurationTimerTimeout()
	{
		_blackout.Visible = false;
		GD.Print("Blackout ended.");
		StartBlackoutTimer(); // schedule the next random blackout
	}
}
