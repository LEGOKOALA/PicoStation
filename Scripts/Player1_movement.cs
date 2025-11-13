using Godot;
using System;
using System.Collections.Generic;

public partial class Player1_movement : CharacterBody2D
{
	public const float Speed = 300f;
	public const float JumpVelocity = 400f;

	private bool gravityFlipped = false;
	private AnimatedSprite2D sprite;

	private float abilityCooldown = 1.0f;
	private float flipTimer = 0f;

	// Tracks collected keys
	private HashSet<string> collectedKeys = new();

	// Tracks if the player is jumping
	public bool IsJumping { get; private set; } = false;

	public override void _Ready()
	{
		// Get the AnimatedSprite2D node
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite == null)
			GD.PrintErr("❌ Could not find AnimatedSprite2D node. Check node name!");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Safety check: stop code if sprite missing
		if (sprite == null)
			return;

		Vector2 velocity = Velocity;

		// Update cooldown timer
		if (flipTimer > 0)
			flipTimer -= (float)delta;

		// Gravity flip
		if (Input.IsActionJustPressed("p1_flip") && flipTimer <= 0)
			FlipGravity();

		// Apply gravity (flip if inverted)
		Vector2 gravity = GetGravity();
		if (gravityFlipped)
			gravity *= -1;
		velocity += gravity * (float)delta;

		// Jump
		if (Input.IsActionJustPressed("p1_jump") && (IsOnFloor() || IsOnCeiling()))
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
			IsJumping = true;
		}
		else if (IsOnFloor() || IsOnCeiling())
		{
			IsJumping = false;
		}

		// Horizontal movement
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p1_sprint"))
				velocity.X *= 3;

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
			// Slow down when not moving
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			// Stop walking animation
			if (sprite.IsPlaying())
				sprite.Stop();
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// --- Flip gravity ---
	private void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		flipTimer = abilityCooldown;
		Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
	}

	// --- Key handling ---
	public void CollectKey(string keyId)
	{
		if (collectedKeys.Add(keyId))
			GD.Print($"Collected key: {keyId}");
		else
			GD.Print($"Already have key: {keyId}");
	}

	public bool HasKey(string keyId)
	{
		return collectedKeys.Contains(keyId);
	}

	public void ConsumeKey(string keyId)
	{
		if (collectedKeys.Remove(keyId))
			GD.Print($"Used key: {keyId}");
	}
}
