using Godot;
using System;

public partial class Player_movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public float JumpVelocity = -400.0f;
	public float SprintMultiplier = 1.5f;

	private float gravity;
	private int gravityDirection = 1; // 1 = normal, -1 = flipped
	
	public override void _Ready()
	{
		gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

 // Handle gravity flipping input
		if (Input.IsActionJustPressed("p1_flip"))
		{
			gravityDirection = -1;
			JumpVelocity *= 1;
		}
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * gravityDirection * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("p1_jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			
			//sprinting
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
