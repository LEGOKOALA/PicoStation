using Godot;
using System;

public partial class Player_movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = 400.0f; // made positive for easier flipping logic

	private bool gravityFlipped = false; // Tracks whether gravity is flipped
	private float gravity;
	
	private int gravityDirection = 1; // 1 = normal, -1 = flipped
	
	public override void _Ready()
	{
		gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		
		if (Input.IsActionJustPressed("p1_flip"))
		{
			gravityFlipped = !gravityFlipped;

			// Optional: visually flip the player
			Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
		}

		velocity.Y += (gravityFlipped ? -gravity : gravity) * (float)delta;

		bool isGrounded = gravityFlipped ? IsOnCeiling() : IsOnFloor();
		
		if (Input.IsActionJustPressed("p1_jump") && isGrounded)
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
