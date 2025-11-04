using Godot;
using System;

public partial class Player_movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = 400.0f; // made positive for easier flipping logic

	private bool gravityFlipped = false; // Tracks whether gravity is flipped

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Toggle gravity flip on key press (not hold)
		if (Input.IsActionJustPressed("p1_flip"))
		{
			gravityFlipped = !gravityFlipped;

			// Optional: visually flip the player
			Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
		}

		// Apply gravity (always)
		Vector2 gravity = GetGravity();
		if (gravityFlipped)
			gravity *= -1;

		velocity += gravity * (float)delta;

		// Handle Jump (direction depends on flip state)
		if (Input.IsActionJustPressed("p1_jump") && (IsOnFloor() | IsOnCeiling()))
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}
		

		// Horizontal movement
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;

			// Sprinting
			if (Input.IsActionPressed("p1_sprint"))
				velocity.X *= 3;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
