using Godot;
using System.Threading.Tasks;

public partial class BlackoutCanvas : CanvasLayer
{
	// Exported variables visible in Inspector
	[Export] public ColorRect BlackoutRect;
	[Export] public float BlackoutDuration = 3.5f;
	[Export] public float MinTimeBetweenBlackouts = 2f;
	[Export] public float MaxTimeBetweenBlackouts = 4f;
	[Export] public float FadeDuration = 0.5f; // Duration of fade in/out

	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready()
	{
		rng.Randomize();

		// Start fully transparent so the screen is not dark initially
		if (BlackoutRect != null)
		{
			var color = BlackoutRect.Color;
			color.A = 0f;
			BlackoutRect.Color = color;
		}

		_ = RandomBlackoutLoop();
	}

	private async Task RandomBlackoutLoop()
	{
		while (true)
		{
			// Wait a random time between blackouts
			float wait = (float)rng.RandfRange(MinTimeBetweenBlackouts, MaxTimeBetweenBlackouts);
			await Task.Delay((int)(wait * 1000));

			await DoBlackout();
		}
	}

	private async Task DoBlackout()
	{
		if (BlackoutRect == null)
			return;

		// Fade to black
		for (float t = 0; t < FadeDuration; t += (float)GetProcessDeltaTime())
		{
			SetAlpha(Mathf.Lerp(0, 1, t / FadeDuration));
			await Task.Yield();
		}
		SetAlpha(1);

		// Wait while fully black
		await Task.Delay((int)(BlackoutDuration * 1000));

		// Fade back to transparent
		for (float t = 0; t < FadeDuration; t += (float)GetProcessDeltaTime())
		{
			SetAlpha(Mathf.Lerp(1, 0, t / FadeDuration));
			await Task.Yield();
		}
		SetAlpha(0);
	}

	private void SetAlpha(float a)
	{
		if (BlackoutRect != null)
		{
			float adjustedAlpha = a * 0.7f;
			var color = BlackoutRect.Color;
			color.A = adjustedAlpha;
			BlackoutRect.Color = color;
		}
	}
}
