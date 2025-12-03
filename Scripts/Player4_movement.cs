using Godot;
using System;
using System.Collections.Generic;

public partial class Player4_movement : CharacterBody2D
{
	public const float Speed = 300f;
	public const float JumpVelocity = 400f;

	private bool gravityFlipped = false; // Tracks whether gravity is flipped
	private AnimatedSprite2D sprite;     // Reference to the player's sprite
	
	private float Ability_cooldown = 1.0f;
	private float Flip_timer = 0.0f;
	private float Brick_timer = 0.0f;
	
	private bool brickMode = false; // tracks if brick is activated


	// Tracks collected keys
	private HashSet<string> collectedKeys = new();


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
		if (Flip_timer > 0)
		// needs to be a float to have same data type as timers
			Flip_timer -= (float)delta;
			Brick_timer -= (float)delta;

		// Toggle gravity flip on key press w/ cooldown
		if (Input.IsActionJustPressed("p4_flip") && Flip_timer <= 0 && brickMode == false)
		{
			FlipGravity();
		}

		// Update cooldown timer
		if (Flip_timer > 0)
			Flip_timer -= (float)delta;
			
			
		// Apply gravity (flip if inverted)
		Vector2 gravity = GetGravity();
		if (gravityFlipped)
			gravity *= -1;
		velocity += gravity * (float)delta;


		// Handle jump (works whether gravity is flipped or not)
		if (Input.IsActionJustPressed("p4_jump") && (IsOnFloor() || IsOnCeiling()) && brickMode == false)
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}

		// Horizontal movement
		Vector2 direction = Input.GetVector("p4_left", "p4_right", "p4_up", "p4_down");

		if (direction != Vector2.Zero && brickMode == false)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p4_sprint"))
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
			// Slow down when not moving
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			// Stop walking animation
			if (sprite.IsPlaying())
				sprite.Stop();
		}
		
		if (Input.IsActionJustPressed("p4_brick") && Brick_timer <= 0)
		{
			brickMode = !brickMode;
			Brick_timer = Ability_cooldown;
			if (brickMode)
			{
				ActivatePlayerInteraction();
				GD.Print("activate");
			}
			else
			{
				DisablePlayerInteraction();
				GD.Print("deactivate");
			}
		}
		
		
		Velocity = velocity;
		MoveAndSlide();
	}

	private void ActivatePlayerInteraction()
	{
		SetCollisionLayerValue(1, true);
		SetCollisionLayerValue(3, true);
		SetCollisionLayerValue(4, true);
	}
	
	private void DisablePlayerInteraction()
	{
		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(3, false);
		SetCollisionLayerValue(4, false);
	}
	
	public void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		Flip_timer = Ability_cooldown;
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
}}
