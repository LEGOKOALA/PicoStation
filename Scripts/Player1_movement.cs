using Godot;
using System;

public partial class Player1_movement : CharacterBody2D
{
	public const float Speed = 300f;
	public const float JumpVelocity = 400f;

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

		if (flipTimer > 0)
			flipTimer -= (float)delta;

		// Horizontal movement
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");
		velocity.X = direction.X * Speed;

		// Sprint
		if (Input.IsActionPressed("p1_sprint"))
			velocity.X *= 3;

		// Flip sprite
		if (direction.X < 0) sprite.FlipH = true;
		else if (direction.X > 0) sprite.FlipH = false;

		// Walk animation
		if (direction != Vector2.Zero)
		{
			if (sprite.Animation != "walk" || !sprite.IsPlaying())
				sprite.Play("walk");
		}
		else
		{
			if (sprite.IsPlaying())
				sprite.Stop();
		}

		// Gravity flip
		if (Input.IsActionJustPressed("p1_flip") && flipTimer <= 0)
			FlipGravity();

		// Jump
		if (Input.IsActionJustPressed("p1_jump") && (IsOnFloor() || IsOnCeiling()))
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;

		// Apply gravity
		Vector2 gravity = GetGravity();
		if (gravityFlipped) gravity *= -1;
		velocity += gravity * (float)delta;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		flipTimer = abilityCooldown;
		Scale = new Vector2(Scale.X, gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y));
	}
}
