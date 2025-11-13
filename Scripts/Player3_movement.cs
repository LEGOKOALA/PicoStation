using Godot;
using System;

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
	private bool gravityFlipped = false;
	private AnimatedSprite2D sprite;

	private float abilityCooldown = 1.0f;
	private float flipTimer = 0f;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (Flip_timer > 0)
		// needs to be a float to have same data type as timers
			Flip_timer -= (float)delta;
			Brick_timer -= (float)delta;

		// Toggle gravity flip on key press w/ cooldown
		if (Input.IsActionJustPressed("p3_flip") && Flip_timer <= 0)
		{
			gravityFlipped = !gravityFlipped;
			Flip_timer = Ability_cooldown;

			// Visually flip the player vertically
			Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
		}

		// Apply gravity
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

		if (flipTimer > 0)
			flipTimer -= (float)delta;

		Vector2 direction = Input.GetVector("p3_left", "p3_right", "p3_up", "p3_down");
		velocity.X = direction.X * Speed;

		if (Input.IsActionPressed("p3_sprint"))
			velocity.X *= 3;

		if (direction.X < 0) sprite.FlipH = true;
		else if (direction.X > 0) sprite.FlipH = false;

		if (direction != Vector2.Zero && brickMode == false)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p3_sprint"))
				velocity.X *= 1.75f;

			//  Flip the sprite horizontally based on movement direction
			if (direction.X < 0)
				sprite.FlipH = true;
			else if (direction.X > 0)
				sprite.FlipH = false;

			//  Play the walk animation if not already playing
			if (sprite.Animation != "walk" || !sprite.IsPlaying())
				sprite.Play("walk");
		}
		else
		{
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
		
		

		if (Input.IsActionJustPressed("p3_flip") && flipTimer <= 0)
			FlipGravity();

		if (Input.IsActionJustPressed("p3_jump") && (IsOnFloor() || IsOnCeiling()))
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;

		Vector2 gravity = GetGravity();
		if (gravityFlipped) gravity *= -1;
		velocity += gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ActivatePlayerInteraction()
	{
		SetCollisionMaskValue(3, true);
		SetCollisionMaskValue(4, true);
		SetCollisionMaskValue(5, true);
	}
	
	private void DisablePlayerInteraction()
	{
		SetCollisionMaskValue(3, false);
		SetCollisionMaskValue(4, false);
		SetCollisionMaskValue(5, false);
	}

	private void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		flipTimer = abilityCooldown;
		Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
	}
}
