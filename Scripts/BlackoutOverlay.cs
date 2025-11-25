using Godot;
using System;

public partial class BlackoutOverlay : CanvasLayer
{
	[Export] private float blackoutDuration = 4f; // seconds screen stays black
	[Export] private float minWait = 5f;          // minimum seconds between blackouts
	[Export] private float maxWait = 10f;         // maximum seconds between blackouts

	private Random _random = new Random();
	private ColorRect _colorRect;
	private Timer _timer;

	public override void _Ready()
	{
		// Get nodes
		_colorRect = GetNode<ColorRect>("ColorRect");
		_timer = GetNode<Timer>("Timer");

		// Make sure ColorRect covers the viewport
		_colorRect.Size = GetViewport().GetVisibleRect().Size;
		_colorRect.Visible = false;

		// Connect Timer signal
		_timer.Timeout += OnTimerTimeout;

		// Schedule the first blackout
		ScheduleNextBlackout();
	}

	private void ScheduleNextBlackout()
	{
		float waitTime = (float)(_random.NextDouble() * (maxWait - minWait) + minWait);
		_timer.WaitTime = waitTime;
		_timer.OneShot = true;
		_timer.Start();
	}

	private async void OnTimerTimeout()
	{
		// Show blackout
		_colorRect.Visible = true;

		// Wait for blackout duration
		await ToSignal(GetTree().CreateTimer(blackoutDuration), "timeout");

		// Hide blackout
		_colorRect.Visible = false;

		// Schedule the next blackout
		ScheduleNextBlackout();
	}
}
