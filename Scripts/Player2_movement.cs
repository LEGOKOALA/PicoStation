using Godot;
using System;

public partial class Player2_movement : CharacterBody2D
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

		Vector2 direction = Input.GetVector("p2_left", "p2_right", "p2_up", "p2_down");
		velocity.X = direction.X * Speed;

		if (Input.IsActionPressed("p2_sprint"))
			velocity.X *= 3;

		if (direction.X < 0) sprite.FlipH = true;
		else if (direction.X > 0) sprite.FlipH = false;

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

		if (Input.IsActionJustPressed("p2_flip") && flipTimer <= 0)
			FlipGravity();

		if (Input.IsActionJustPressed("p2_jump") && (IsOnFloor() || IsOnCeiling()))
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;

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
