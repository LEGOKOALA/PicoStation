using Godot;
using System;

public partial class Key : Area2D
{
	[Export] public string KeyId = "default";

	public override void _Ready()
	{
		// Connect the signal in C#
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		Node player = null;

		if (body is Player1_movement p1) player = p1;
		else if (body is Player2_movement p2) player = p2;
		else if (body is Player3_movement p3) player = p3;
		else if (body is Player4_movement p4) player = p4;

		if (player == null)
			return;

		GD.Print($"Player collected key: {KeyId}");

		// Call CollectKey dynamically
		dynamic dynPlayer = player;
		if (dynPlayer.HasMethod("CollectKey"))
			dynPlayer.CollectKey(KeyId);

		QueueFree(); // Remove key from the world
	}
}
