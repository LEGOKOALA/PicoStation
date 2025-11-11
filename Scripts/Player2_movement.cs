using Godot;
using System;

public partial class Player2_movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = 400.0f;

	private bool gravityFlipped = false; // Tracks whether gravity is flipped
	private AnimatedSprite2D sprite;     // Reference to the player's sprite
	
	private float Ability_cooldown = 1.0f;
	private float Flip_timer = 0.0f;

	public override void _Ready()
	{
		// Gets the AnimatedSprite2D node
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (Flip_timer > 0)
		// needs to be a float to have same data type
			Flip_timer -= (float)delta;

		// Toggle gravity flip on key press
		if (Input.IsActionJustPressed("p2_flip") && Flip_timer <= 0)
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
		if (Input.IsActionJustPressed("p2_jump") && (IsOnFloor() || IsOnCeiling()))
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}

		// Horizontal movement
		Vector2 direction = Input.GetVector("p2_left", "p2_right", "p2_up", "p2_down");

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p2_sprint"))
				velocity.X *= 3;

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
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			// Stops the walking animation when not moving
			if (sprite.IsPlaying())
				sprite.Stop();
		}
		
		if (Input.IsActionJustPressed("p2_brick"))
		{
			this.SetCollisionLayerValue(9, true);
		}
		
		
		Velocity = velocity;
		MoveAndSlide();
	}
}
