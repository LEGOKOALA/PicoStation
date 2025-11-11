using Godot;
using System;
using System.Collections.Generic;

public partial class Player_movement : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	private HashSet<string> collectedKeys = new();

	// 🟢 Added: Track if the player is jumping
	public bool IsJumping { get; private set; } = false;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Apply gravity
		if (!IsOnFloor())
			velocity += GetGravity() * (float)delta;

		// --- Jump logic ---
		if (Input.IsActionJustPressed("p1_jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			IsJumping = true; // Mark as jumping
		}

		// --- Landing check ---
		if (IsOnFloor() && velocity.Y >= 0)
		{
			IsJumping = false; // Landed
		}

		// --- Movement ---
		Vector2 direction = Input.GetVector("p1_left", "p1_right", "p1_up", "p1_down");

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;

			if (Input.IsActionPressed("p1_sprint"))
				velocity.X *= 2;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
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
