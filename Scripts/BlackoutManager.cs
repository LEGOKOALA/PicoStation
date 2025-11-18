using Godot;
using System;

public partial class BlackoutManager : Node
{
	private ColorRect _blackout;      // black overlay
	private Timer _blackoutTimer;     // random delay timer
	private Timer _durationTimer;     // blackout duration timer
	private Random _rand = new Random();

	public override void _Ready()
	{
		// --- Step 1: Make this node persist across all scenes ---
		if (GetParent() != GetTree().Root)
		{
			GetTree().Root.AddChild(this);
			this.Owner = null; // detach from current scene so it won't get deleted
		}

		// --- Step 2: Create the ColorRect dynamically ---
		_blackout = new ColorRect
		{
			Color = Colors.Black,
			Size = GetViewport().GetVisibleRect().Size,
			Visible = false
		};
		_blackout.PauseMode = PauseMode.Process; // allows blackout to appear even if game is paused
		GetTree().Root.AddChild(_blackout);      // always on top of current scene

		// --- Step 3: Create timers dynamically ---
		_blackoutTimer = new Timer { OneShot = true };
		AddChild(_blackoutTimer);
		_blackoutTimer.Timeout += OnBlackoutTimerTimeout;

		_durationTimer = new Timer { OneShot = true };
		AddChild(_durationTimer);
		_durationTimer.Timeout += OnDurationTimerTimeout;

		// Start the first random blackout
		StartBlackoutTimer();
	}

	// Start the next blackout timer (random delay 3–10 seconds)
	private void StartBlackoutTimer()
	{
		float delay = (float)(_rand.NextDouble() * 7.0 + 3.0); // 3-10 seconds
		_blackoutTimer.WaitTime = delay;
		_blackoutTimer.Start();
		GD.Print("Next blackout in ", delay, " seconds.");
	}

	// Called when the blackout timer finishes — show blackout
	private void OnBlackoutTimerTimeout()
	{
		_blackout.Visible = true;
		_durationTimer.WaitTime = 5f; // blackout duration in seconds
		_durationTimer.Start();
		GD.Print("Blackout started!");
	}

	// Called when blackout duration ends — hide blackout
	private void OnDurationTimerTimeout()
	{
		_blackout.Visible = false;
		GD.Print("Blackout ended.");
		StartBlackoutTimer(); // schedule next random blackout
	}
}
