using Godot;
using System;
using System.Collections.Generic;

public partial class Player2_movement : CharacterBody2D
{
	public const float Speed = 300f;
	public const float JumpVelocity = 400f;

	private bool gravityFlipped = false;
	private AnimatedSprite2D sprite;

	private float Ability_cooldown = 1.0f;
	private float Flip_timer = 0.0f;
	private float Brick_timer = 0.0f;

	private PointLight2D playerLight;

	public enum BrickModeState { Normal, Brick }
	private BrickModeState brickMode = BrickModeState.Normal;

	private HashSet<string> collectedKeys = new();

	// Key icon reference
	private Sprite2D K_2;

	public override void _Ready()
	{
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (sprite == null)
			GD.PrintErr("❌ Could not find AnimatedSprite2D node. Check node name!");

		K_2 = GetNode<Sprite2D>("K_2");

		playerLight = GetNodeOrNull<PointLight2D>("PointLight2D");
		if (playerLight == null)
			GD.PrintErr("❌ Could not find PointLight2D");

		if (playerLight != null)
			playerLight.Visible = false;

		AddToGroup("players");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (sprite == null)
			return;

		Vector2 velocity = Velocity;

		if (Flip_timer > 0)
			Flip_timer -= (float)delta;

		Brick_timer -= (float)delta;

		if (Input.IsActionJustPressed("p2_flip") &&
			Flip_timer <= 0 &&
			brickMode == BrickModeState.Normal)
		{
			FlipGravity();
		}

		Vector2 gravity = GetGravity();
		if (gravityFlipped)
			gravity *= -1;

		velocity += gravity * (float)delta;

		if (Input.IsActionJustPressed("p2_jump") &&
			(IsOnFloor() || IsOnCeiling()) &&
			brickMode == BrickModeState.Normal)
		{
			velocity.Y = gravityFlipped ? JumpVelocity : -JumpVelocity;
		}

		Vector2 direction = Input.GetVector("p2_left", "p2_right", "p2_up", "p2_down");

		if (direction != Vector2.Zero && brickMode == BrickModeState.Normal)
		{
			velocity.X = direction.X * Speed;

			if (Input.IsActionPressed("p2_sprint"))
				velocity.X *= 1.75f;

			if (direction.X < 0)
				sprite.FlipH = true;
			else if (direction.X > 0)
				sprite.FlipH = false;

			if (sprite.Animation != "walk" || !sprite.IsPlaying())
				sprite.Play("walk");
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if (sprite.IsPlaying())
				sprite.Stop();
		}

		if (Input.IsActionJustPressed("p2_brick") && Brick_timer <= 0)
		{
			if (brickMode == BrickModeState.Normal)
			{
				brickMode = BrickModeState.Brick;
				ActivatePlayerInteraction();
				GD.Print("activate");
			}
			else
			{
				brickMode = BrickModeState.Normal;
				DisablePlayerInteraction();
				GD.Print("deactivate");
			}

			Brick_timer = Ability_cooldown;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void ActivatePlayerInteraction()
	{
		SetCollisionLayerValue(3, true);
		SetCollisionLayerValue(4, true);
		SetCollisionLayerValue(5, true);
	}

	private void DisablePlayerInteraction()
	{
		SetCollisionLayerValue(3, false);
		SetCollisionLayerValue(4, false);
		SetCollisionLayerValue(5, false);
	}

	public void FlipGravity()
	{
		gravityFlipped = !gravityFlipped;
		Flip_timer = Ability_cooldown;

		Scale = new Vector2(
			Scale.X,
			gravityFlipped ? -Mathf.Abs(Scale.Y) : Mathf.Abs(Scale.Y)
		);
	}

	// ---------------------------------------
	// KEY HANDLING
	// ---------------------------------------
	public void CollectKey(string keyId)
	{
		if (collectedKeys.Add(keyId))
			GD.Print($"Collected key: {keyId}");
		else
			GD.Print($"Already have key: {keyId}");

		K_2.Visible = true;
		K_2.Modulate = Colors.White;
		K_2.SelfModulate = Colors.White;
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
			K_2.Modulate = new Color(1f, 1f, 1f, 0f);
			K_2.SelfModulate = new Color(1f, 1f, 1f, 0f);
		}
	}

	// ---------------------------------------
	// FLASHLIGHT
	// ---------------------------------------
	public void TurnOnFlashlight()
	{
		if (playerLight != null)
		{
			playerLight.Visible = true;
			GD.Print("Flashlight turned ON");
		}
	}

	public void TurnOffFlashlight()
	{
		if (playerLight != null)
		{
			playerLight.Visible = false;
			GD.Print("Flashlight turned OFF");
		}
	}
}
