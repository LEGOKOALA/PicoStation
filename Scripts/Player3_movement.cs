using Godot;
using System;
using System.Collections.Generic;

public partial class Player3_movement : CharacterBody2D
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
				K_3 = GetNode<Sprite2D>("K_3");
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
		if (Input.IsActionJustPressed("p3_flip") && Flip_timer <= 0 && brickMode == false)
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
		if (Input.IsActionJustPressed("p3_jump") && (IsOnFloor() || IsOnCeiling()) && brickMode == false)
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}

		// Horizontal movement
		Vector2 direction = Input.GetVector("p3_left", "p3_right", "p3_up", "p3_down");

		if (direction != Vector2.Zero && brickMode == false)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p3_sprint"))
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
		
		if (Input.IsActionJustPressed("p3_brick") && Brick_timer <= 0)
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
		SetCollisionLayerValue(5, true);
	}
	
	private void DisablePlayerInteraction()
	{
		SetCollisionLayerValue(1, false);
		SetCollisionLayerValue(3, false);
		SetCollisionLayerValue(5, false);
	}
	
	public void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		Flip_timer = Ability_cooldown;
		Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
	}
	
	// --- Key handling ---
private Sprite2D K_3;

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
	K_3.Visible = true;
	K_3.Modulate = new Color(1f, 1f, 1f, 1f);
	K_3.SelfModulate = new Color(1f, 1f, 1f, 1f);
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
		K_3.Modulate = new Color(1f, 1f, 1f, 0f);
		K_3.SelfModulate = new Color(1f, 1f, 1f, 0f);
	}
}}
