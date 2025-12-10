using Godot;
using System;
using System.Collections.Generic;

public partial class Player1_movement : CharacterBody2D
{
	public const float Speed = 300f;
	public const float JumpVelocity = 400f;

	private bool gravityFlipped = false; // Tracks whether gravity is flipped
	private AnimatedSprite2D sprite;     // Reference to the player's sprite
	
	private float Ability_cooldown = 1.0f;
	private float Flip_timer = 0.0f;
	private float Brick_timer = 0.0f;
	
	//does this instead of brickMode bool
	public enum BrickModeState
	{
		Normal,
		Brick
	}

	private BrickModeState brickMode = BrickModeState.Normal;

	// Tracks collected keys
	private HashSet<string> collectedKeys = new();


	public override void _Ready()
	{
		// Get the AnimatedSprite2D node
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite == null)
			GD.PrintErr("❌ Could not find AnimatedSprite2D node. Check node name!");
		keySprite = GetNode<Sprite2D>("K_1");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Safety check: stop code if sprite missing
		if (sprite == null)
			return;

		Vector2 velocity = Velocity;

		if (Flip_timer > 0)
			Flip_timer -= (float)delta;

		Brick_timer -= (float)delta;

		// Toggle gravity flip on key press w/ cooldown
		if (Input.IsActionJustPressed("p1_flip") && 
			Flip_timer <= 0 && 
			brickMode == BrickModeState.Normal)
		{
			FlipGravity();
		}

		// Apply gravity (flip if inverted)
		Vector2 gravity = GetGravity();
		if (gravityFlipped)
			gravity *= -1;

		velocity += gravity * (float)delta;

		// Handle jump
		if (Input.IsActionJustPressed("p1_jump") && 
			(IsOnFloor() || IsOnCeiling()) && 
			brickMode == BrickModeState.Normal)
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}

		// Horizontal movement
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");

		if (direction != Vector2.Zero && brickMode == BrickModeState.Normal)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p1_sprint"))
				velocity.X *= 1.75f;

			// Flip sprite direction
			if (direction.X < 0)
				sprite.FlipH = true;
			else if (direction.X > 0)
				sprite.FlipH = false;

			// Play walk animation
			if (sprite.Animation != "walk" || !sprite.IsPlaying())
				sprite.Play("walk");
		}
		else
		{
			// Slow to stop
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if (sprite.IsPlaying())
				sprite.Stop();
		}

		// Toggle brick mode
		if (Input.IsActionJustPressed("p1_brick") && Brick_timer <= 0)
		{
			if (brickMode == BrickModeState.Normal)
			{
				//checks brickmode enum
				brickMode = BrickModeState.Brick;
				ActivatePlayerInteraction();
				GD.Print("activate");
			}
			else
			{
				brickMode = BrickModeState.Normal;
				DisablePlayerInteraction();
				GD.Print("deactivate");
			}

			Brick_timer = Ability_cooldown;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ActivatePlayerInteraction()
	{
		SetCollisionLayerValue(3, true);
		SetCollisionLayerValue(4, true);
		SetCollisionLayerValue(5, true);
	}

	private void DisablePlayerInteraction()
	{
		SetCollisionLayerValue(3, false);
		SetCollisionLayerValue(4, false);
		SetCollisionLayerValue(5, false);
	}

	public void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		Flip_timer = Ability_cooldown;
		Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
	}

	// --- Key handling ---
private Sprite2D keySprite;

public void CollectKey(string keyId)
{
	if (collectedKeys.Add(keyId))
	{
		GD.Print($"Collected key: {keyId}");
	}
	else
	{
		GD.Print($"Already have key: {keyId}");
	}

	// Force key icon visible
	keySprite.Visible = true;
	keySprite.Modulate = new Color(1f, 1f, 1f, 1f);
	keySprite.SelfModulate = new Color(1f, 1f, 1f, 1f);
}

public bool HasKey(string keyId)
{
	return collectedKeys.Contains(keyId);
}

public void ConsumeKey(string keyId)
{
	if (collectedKeys.Remove(keyId))
	{
		GD.Print($"Used key: {keyId}");

		// Hide the key
		keySprite.Modulate = new Color(1f, 1f, 1f, 0f);
		keySprite.SelfModulate = new Color(1f, 1f, 1f, 0f);
	}
}}
